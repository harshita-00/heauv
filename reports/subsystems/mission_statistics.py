"""
mission_statistics.py
---------------------
Builds the Mission Statistics page.

Data sources (both from .dat files in the run folder):
    Control file  → depth, lat, lon, nvel, evel, dvel
    MCS RTC file  → day, month, year, hour, minute, second (start/end time)

NOTE: No matplotlib import here — importing matplotlib in this module
      would reset the Agg backend set by pdf_builder.py and corrupt
      the X-axis formatting on all plot pages.
"""

import os
import sys
import math
from datetime import datetime

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", ".."))
if ROOT not in sys.path:
    sys.path.insert(0, ROOT)

import numpy as np
from reportlab.lib.pagesizes import A4, landscape
from reportlab.lib import colors
from reportlab.lib.units import cm
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.enums import TA_RIGHT, TA_LEFT
from reportlab.pdfgen import canvas as rl_canvas

from reports.utils.data_extractor import find_dat_file, load_dat_file

# ── Page constants ────────────────────────────────────────────────────────────
PAGE_W, PAGE_H = landscape(A4)
MARGIN         = 1.8 * cm

# ── Colour palette (matches pdf_builder.py exactly) ──────────────────────────
NAVY       = colors.HexColor("#0A1628")
STEEL_BLUE = colors.HexColor("#1B3A6B")
ACCENT     = colors.HexColor("#2E86C1")
LIGHT_BLUE = colors.HexColor("#AED6F1")
ROW_ALT    = colors.HexColor("#EAF4FB")
WHITE      = colors.white
BLACK      = colors.black

# ── Control column indices (0-based) ─────────────────────────────────────────
COL_DEPTH = 1
COL_LAT   = 9
COL_LON   = 10
COL_NVEL  = 27
COL_EVEL  = 28
COL_DVEL  = 29

# ── MCS RTC column indices ────────────────────────────────────────────────────
MCS_DAY   = 1
MCS_MONTH = 2
MCS_YEAR  = 3
MCS_HOUR  = 4
MCS_MIN   = 5
MCS_SEC   = 6


# ── Helpers ───────────────────────────────────────────────────────────────────

def _safe_col(data: np.ndarray, idx: int) -> np.ndarray:
    if idx < data.shape[1]:
        return data[:, idx]
    return np.full(data.shape[0], np.nan)


def _haversine(la1, lo1, la2, lo2) -> float:
    R = 6_371_000.0
    phi1, phi2 = math.radians(la1), math.radians(la2)
    dphi = math.radians(la2 - la1)
    dlam = math.radians(lo2 - lo1)
    a = (math.sin(dphi / 2) ** 2
         + math.cos(phi1) * math.cos(phi2) * math.sin(dlam / 2) ** 2)
    return R * 2 * math.atan2(math.sqrt(a), math.sqrt(1 - a))


def _fmt_lat(v) -> str:
    if v is None or math.isnan(v):
        return "N/A"
    hem = "N" if v >= 0 else "S"
    return f"{hem}{abs(v):.8f}°"


def _fmt_lon(v) -> str:
    if v is None or math.isnan(v):
        return "N/A"
    hem = "E" if v >= 0 else "W"
    return f"{hem}{abs(v):.8f}°"


def _find_mcs_file(run_folder: str) -> str | None:
    for fname in os.listdir(run_folder):
        clean = fname.lstrip("_").lower()
        if fname.endswith(".dat") and "mcs" in clean and "rtc" in clean:
            return os.path.join(run_folder, fname)
    return None


def _compute_statistics(run_folder: str) -> dict | None:
    """
    Load control + MCS files and compute mission statistics.
    Returns a dict of metrics, or None if control file missing.
    """
    ctrl_path = find_dat_file(run_folder, "control")
    if ctrl_path is None:
        return None

    ctrl = load_dat_file(ctrl_path)
    if ctrl is None:
        return None

    # ── Time from MCS RTC ─────────────────────────────────────────────────
    start_str = end_str = dur_str = "N/A"
    mcs_path = _find_mcs_file(run_folder)
    if mcs_path:
        mcs = load_dat_file(mcs_path)
        if mcs is not None and mcs.shape[0] >= 2:
            def get_dt(row):
                r = mcs[row]
                try:
                    yr = int(r[MCS_YEAR])
                    mo = int(r[MCS_MONTH])
                    dy = int(r[MCS_DAY])
                    hr = int(r[MCS_HOUR])
                    mn = int(r[MCS_MIN])
                    sc = int(r[MCS_SEC])
                    # Validate ranges before constructing datetime
                    if not (2000 <= yr <= 2100 and 1 <= mo <= 12 and
                            1 <= dy <= 31 and 0 <= hr <= 23 and
                            0 <= mn <= 59 and 0 <= sc <= 59):
                        return None
                    return datetime(yr, mo, dy, hr, mn, sc)
                except Exception:
                    return None

            t0 = get_dt(0)
            t1 = get_dt(-1)
            if t0 and t1:
                start_str = t0.strftime("%a %b %d %H:%M:%S IST %Y")
                end_str   = t1.strftime("%a %b %d %H:%M:%S IST %Y")
                secs      = int((t1 - t0).total_seconds())
                h, rem    = divmod(abs(secs), 3600)
                m, s      = divmod(rem, 60)
                dur_str   = f"{h:02d}h {m:02d}m {s:02d}s"

    # ── Depth ─────────────────────────────────────────────────────────────
    depths = _safe_col(ctrl, COL_DEPTH)
    # Only use positive depth values (ignore sensor noise near zero)
    vd = depths[(~np.isnan(depths)) & (depths > 0)]
    max_depth = f"{float(np.max(vd)):.2f} m"  if len(vd) else "N/A"
    avg_depth = f"{float(np.mean(vd)):.2f} m" if len(vd) else "N/A"

    # ── Distance ──────────────────────────────────────────────────────────
    lats = _safe_col(ctrl, COL_LAT)
    lons = _safe_col(ctrl, COL_LON)
    total_dist = 0.0

    # Only compute distance if lat/lon have real values (not zeros)
    valid_lats = lats[(~np.isnan(lats)) & (lats != 0)]
    if len(valid_lats) > 1:
        for i in range(1, len(lats)):
            if not any(math.isnan(v) or v == 0
                       for v in [lats[i], lats[i-1], lons[i], lons[i-1]]):
                total_dist += _haversine(lats[i-1], lons[i-1], lats[i], lons[i])
    dist_str = f"{total_dist:.2f} m"

    # ── Speed ─────────────────────────────────────────────────────────────
    nvel = _safe_col(ctrl, COL_NVEL)
    evel = _safe_col(ctrl, COL_EVEL)
    dvel = _safe_col(ctrl, COL_DVEL)
    res  = np.sqrt(nvel**2 + evel**2 + dvel**2)
    # Filter out unrealistic values (only keep reasonable speed range 0-20 m/s)
    vs = res[(~np.isnan(res)) & (res < 20) & (res >= 0)]
    mean_speed = f"{float(np.mean(vs)):.2f} m/s" if len(vs) else "N/A"

    # ── Home position ─────────────────────────────────────────────────────
    home_lat = next((v for v in lats if not math.isnan(v) and v != 0), None)
    home_lon = next((v for v in lons if not math.isnan(v) and v != 0), None)

    return {
        "vehicle":    "HEAUV",
        "start_time": start_str,
        "end_time":   end_str,
        "duration":   dur_str,
        "max_depth":  max_depth,
        "avg_depth":  avg_depth,
        "distance":   dist_str,
        "mean_speed": mean_speed,
        "home_lat":   _fmt_lat(home_lat),
        "home_lon":   _fmt_lon(home_lon),
    }


# ═════════════════════════════════════════════════════════════════════════════
#  Page builder
# ═════════════════════════════════════════════════════════════════════════════

def build_mission_statistics_page(c: rl_canvas.Canvas,
                                   run_folder: str,
                                   page_number: int,
                                   mission_info: dict) -> bool:
    """
    Draw the Mission Statistics page.
    Label : Value layout — no table, no image, no roll/pitch.
    Returns True if drawn, False if control file missing.
    """
    stats = _compute_statistics(run_folder)
    if stats is None:
        return False

    bin_name    = mission_info.get("bin_name", "")
    run_name    = mission_info.get("run_name", "")
    mission_str = f"Mission Report — {bin_name}  |  {run_name}"

    c.saveState()

    # ── White background ──────────────────────────────────────────────────
    c.setFillColor(WHITE)
    c.rect(0, 0, PAGE_W, PAGE_H, fill=1, stroke=0)

    # ── Header strip (identical to all other pages) ───────────────────────
    c.setFillColor(NAVY)
    c.rect(0, PAGE_H - 1.6 * cm, PAGE_W, 1.6 * cm, fill=1, stroke=0)

    c.setFillColor(ACCENT)
    c.rect(0, PAGE_H - 1.75 * cm, PAGE_W, 0.18 * cm, fill=1, stroke=0)

    c.setFillColor(WHITE)
    c.setFont("Helvetica-Bold", 11)
    c.drawString(MARGIN, PAGE_H - 1.1 * cm, "HEAUV  |  MISSION STATISTICS")

    c.setFont("Helvetica", 9)
    c.drawRightString(PAGE_W - MARGIN, PAGE_H - 1.1 * cm, mission_str)

    # ── Section heading (identical to all other pages) ────────────────────
    heading_y = PAGE_H - 2.8 * cm
    c.setFillColor(BLACK)
    c.setFont("Helvetica-Bold", 16)
    c.drawCentredString(PAGE_W / 2, heading_y, "Mission Statistics")

    text_w = c.stringWidth("Mission Statistics", "Helvetica-Bold", 16)
    c.setStrokeColor(ACCENT)
    c.setLineWidth(1.2)
    c.line(PAGE_W / 2 - text_w / 2 - 8, heading_y - 4,
           PAGE_W / 2 + text_w / 2 + 8, heading_y - 4)

    # ── Stats rows ────────────────────────────────────────────────────────
    rows = [
        ("Vehicle:",            stats["vehicle"]),
        ("Mission start time:", stats["start_time"]),
        ("Mission end time:",   stats["end_time"]),
        ("Mission duration:",   stats["duration"]),
        ("Maximum depth:",      stats["max_depth"]),
        ("Avg depth:",          stats["avg_depth"]),
        ("Distance travelled:", stats["distance"]),
        ("Mean speed:",         stats["mean_speed"]),
        ("Home Latitude:",      stats["home_lat"]),
        ("Home Longitude:",     stats["home_lon"]),
    ]

    content_top    = heading_y - 1.2 * cm
    content_bottom = 1.8 * cm
    content_h      = content_top - content_bottom
    row_h          = content_h / (len(rows) + 1)

    # Column split — label right-aligns to col_mid, value left-aligns after
    col_mid = PAGE_W * 0.42

    # Subtle vertical divider
    c.setStrokeColor(colors.HexColor("#DDDDDD"))
    c.setLineWidth(0.5)
    c.line(col_mid, content_bottom + 0.3 * cm,
           col_mid, content_top - 0.3 * cm)

    for i, (lbl, val) in enumerate(rows):
        y = content_top - (i + 0.75) * row_h

        # Alternating row highlight
        if i % 2 == 0:
            c.setFillColor(ROW_ALT)
            c.rect(MARGIN, y - row_h * 0.35,
                   PAGE_W - 2 * MARGIN, row_h * 0.75,
                   fill=1, stroke=0)

        # Label — right-aligned, ACCENT blue bold
        c.setFillColor(ACCENT)
        c.setFont("Helvetica-Bold", 11)
        c.drawRightString(col_mid - 0.4 * cm, y, lbl)

        # Value — left-aligned, dark text
        c.setFillColor(colors.HexColor("#1A1A1A"))
        c.setFont("Helvetica", 11)
        c.drawString(col_mid + 0.4 * cm, y, val)

    # ── Footer (identical to all other pages) ─────────────────────────────
    c.setFillColor(NAVY)
    c.rect(0, 0, PAGE_W, 1.4 * cm, fill=1, stroke=0)

    c.setFillColor(ACCENT)
    c.rect(0, 1.4 * cm, PAGE_W, 0.12 * cm, fill=1, stroke=0)

    c.setFillColor(WHITE)
    c.setFont("Helvetica", 8)
    c.drawString(MARGIN, 0.5 * cm, "HEAUV  —  Mission Telemetry Report")
    c.drawRightString(PAGE_W - MARGIN, 0.5 * cm, f"Page {page_number}")

    c.restoreState()
    c.showPage()
    return True