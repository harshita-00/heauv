"""
pdf_builder.py
--------------
ReportLab helpers shared by every subsystem report module.
Handles:
  - Cover page  (HEAUV title + AUV illustration + mission info box)
  - One-parameter-per-page plot pages
  - Consistent styling  (white grid plot, bold centred heading)
  - Landscape A4 throughout
"""

import io
import datetime
import os
import sys
import math

import matplotlib
matplotlib.use("Agg")          # non-interactive backend — no Qt needed
import matplotlib.pyplot as plt
import matplotlib.ticker as ticker
import numpy as np

from reportlab.lib.pagesizes import A4, landscape
from reportlab.lib import colors
from reportlab.lib.units import cm
from reportlab.lib.styles import getSampleStyleSheet, ParagraphStyle
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.utils import ImageReader                  # Windows-safe image embedding
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, PageBreak, Image as RLImage,
    HRFlowable, Table, TableStyle
)
from reportlab.graphics.shapes import Drawing, String, Line, Rect, Ellipse, Circle, Path
from reportlab.graphics import renderPDF
from reportlab.pdfgen import canvas as rl_canvas

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
if ROOT not in sys.path:
    sys.path.insert(0, ROOT)

# ── Page constants (landscape) ────────────────────────────────────────────────
PAGE_W, PAGE_H = landscape(A4)
MARGIN        = 1.8 * cm
PLOT_W        = PAGE_W - 2 * MARGIN
PLOT_H        = PAGE_H - 6.5 * cm      # room for heading + axis labels


# ── Colour palette ────────────────────────────────────────────────────────────
NAVY          = colors.HexColor("#0A1628")
STEEL_BLUE    = colors.HexColor("#1B3A6B")
ACCENT        = colors.HexColor("#2E86C1")
LIGHT_BLUE    = colors.HexColor("#AED6F1")
WHITE         = colors.white
BLACK         = colors.black
PLOT_LINE     = "#1A5276"       # deep navy line on white background


# ═════════════════════════════════════════════════════════════════════════════
#  AUV illustration drawn purely with ReportLab shapes
# ═════════════════════════════════════════════════════════════════════════════

def _draw_auv_illustration(c: rl_canvas.Canvas, cx: float, cy: float,
                            width: float = 400, height: float = 180):
    """
    Draw a clean, DRDO-appropriate AUV illustration centred at (cx, cy).
    Uses only ReportLab primitives — no external image file required.
    """
    c.saveState()

    # ── Ocean background panel ────────────────────────────────────────────
    panel_w, panel_h = width + 60, height + 60
    px, py = cx - panel_w / 2, cy - panel_h / 2

    for i, col in enumerate(["#0D2137", "#0E2A47", "#0F3460", "#1A4A7A", "#1B5E8A"]):
        c.setFillColor(colors.HexColor(col))
        strip_h = panel_h / 5
        c.rect(px, py + i * strip_h, panel_w, strip_h + 1, fill=1, stroke=0)

    c.setStrokeColor(colors.HexColor("#2471A3"))
    c.setLineWidth(0.6)
    for i in range(4):
        y_wave = py + panel_h * 0.15 + i * 12
        c.line(px + 10, y_wave, px + panel_w - 10, y_wave)

    # ── AUV body ──────────────────────────────────────────────────────────
    bx     = cx - width * 0.40
    by     = cy - height * 0.10
    body_w = width * 0.80
    body_h = height * 0.28

    c.setFillColor(colors.HexColor("#0A2040"))
    c.roundRect(bx + 4, by - 4, body_w, body_h, body_h / 2, fill=1, stroke=0)

    c.setFillColor(colors.HexColor("#1C3F6E"))
    c.roundRect(bx, by, body_w, body_h, body_h / 2, fill=1, stroke=0)

    c.setFillColor(colors.HexColor("#2E6FA3"))
    c.roundRect(bx + 8, by + body_h * 0.55, body_w * 0.82, body_h * 0.25,
                body_h * 0.12, fill=1, stroke=0)

    c.setFillColor(colors.HexColor("#E74C3C"))
    c.roundRect(bx + body_w * 0.25, by, body_w * 0.06, body_h, 2, fill=1, stroke=0)

    c.setFillColor(colors.HexColor("#2ECC71"))
    c.roundRect(bx + body_w * 0.33, by, body_w * 0.03, body_h, 2, fill=1, stroke=0)

    # ── Nose cone ─────────────────────────────────────────────────────────
    nose_x   = bx + body_w
    nose_tip = nose_x + width * 0.13
    mid_y    = by + body_h / 2

    path = c.beginPath()
    path.moveTo(nose_x, by)
    path.lineTo(nose_tip, mid_y)
    path.lineTo(nose_x, by + body_h)
    path.close()
    c.setFillColor(colors.HexColor("#2471A3"))
    c.drawPath(path, fill=1, stroke=0)

    # ── Tail cone ─────────────────────────────────────────────────────────
    tail_x   = bx
    tail_tip = tail_x - width * 0.08

    path2 = c.beginPath()
    path2.moveTo(tail_x, by + body_h * 0.2)
    path2.lineTo(tail_tip, mid_y)
    path2.lineTo(tail_x, by + body_h * 0.8)
    path2.close()
    c.setFillColor(colors.HexColor("#154360"))
    c.drawPath(path2, fill=1, stroke=0)

    # ── Dorsal fin ────────────────────────────────────────────────────────
    sail_base_x = bx + body_w * 0.35
    sail_h      = height * 0.30

    path3 = c.beginPath()
    path3.moveTo(sail_base_x, by + body_h)
    path3.lineTo(sail_base_x + body_w * 0.04, by + body_h + sail_h)
    path3.lineTo(sail_base_x + body_w * 0.16, by + body_h)
    path3.close()
    c.setFillColor(colors.HexColor("#1A5276"))
    c.drawPath(path3, fill=1, stroke=0)

    # ── Propeller ─────────────────────────────────────────────────────────
    prop_cx = tail_tip - 4
    prop_r  = body_h * 0.55

    c.setStrokeColor(colors.HexColor("#85C1E9"))
    c.setLineWidth(2)
    for angle_deg in [30, 90, 150, 210, 270, 330]:
        a = math.radians(angle_deg)
        c.line(prop_cx, mid_y,
               prop_cx + prop_r * math.cos(a),
               mid_y   + prop_r * math.sin(a))

    c.setFillColor(colors.HexColor("#2980B9"))
    c.circle(prop_cx, mid_y, 4, fill=1, stroke=0)

    # ── Sonar dome ────────────────────────────────────────────────────────
    c.setFillColor(colors.HexColor("#5DADE2"))
    c.circle(nose_tip - 6, mid_y, 5, fill=1, stroke=0)

    # ── DVL transducers ───────────────────────────────────────────────────
    c.setFillColor(colors.HexColor("#85C1E9"))
    for i in range(4):
        tx = bx + body_w * (0.40 + i * 0.06)
        c.rect(tx, by - 6, 6, 6, fill=1, stroke=0)

    # ── Sonar rings ───────────────────────────────────────────────────────
    c.setStrokeColor(colors.HexColor("#5DADE2"))
    c.setLineWidth(0.5)
    for r in [18, 28, 38]:
        c.circle(nose_tip - 6, mid_y, r, fill=0, stroke=1)

    # ── Bubbles ───────────────────────────────────────────────────────────
    c.setFillColor(colors.HexColor("#AED6F1"))
    for bub in [(nose_tip + 8,  mid_y + 12, 3),
                (nose_tip + 15, mid_y - 6,  2),
                (nose_tip + 22, mid_y + 20, 2),
                (nose_tip + 5,  mid_y + 28, 2)]:
        c.circle(bub[0], bub[1], bub[2], fill=1, stroke=0)

    # ── Panel border ──────────────────────────────────────────────────────
    c.setStrokeColor(colors.HexColor("#2E86C1"))
    c.setLineWidth(1.2)
    c.roundRect(px, py, panel_w, panel_h, 8, fill=0, stroke=1)

    c.restoreState()


# ═════════════════════════════════════════════════════════════════════════════
#  Cover page
# ═════════════════════════════════════════════════════════════════════════════

def build_cover_page(c: rl_canvas.Canvas, run_folder: str, mission_info: dict):
    """
    Draw the full cover page directly onto the canvas.

    mission_info keys:
        run_name   : e.g. 'RUN-0'
        bin_name   : e.g. 'bbbin'
        display_dt : e.g. '09-03-2026  20:20:39'
    """

    c.saveState()

    # ── Background ────────────────────────────────────────────────────────
    c.setFillColor(NAVY)
    c.rect(0, 0, PAGE_W, PAGE_H, fill=1, stroke=0)

    # ── Top decorative bars ───────────────────────────────────────────────
    c.setFillColor(ACCENT)
    c.rect(0, PAGE_H - 1.0 * cm, PAGE_W, 0.35 * cm, fill=1, stroke=0)

    c.setFillColor(colors.HexColor("#E74C3C"))
    c.rect(0, PAGE_H - 1.4 * cm, PAGE_W, 0.35 * cm, fill=1, stroke=0)

    # ── Organisation label ────────────────────────────────────────────────
    c.setFillColor(LIGHT_BLUE)
    c.setFont("Helvetica", 10)
    c.drawCentredString(PAGE_W / 2, PAGE_H - 2.0 * cm,
                        "DEFENCE RESEARCH AND DEVELOPMENT ORGANISATION")

    # ── Horizontal rule ───────────────────────────────────────────────────
    c.setStrokeColor(ACCENT)
    c.setLineWidth(1.0)
    c.line(MARGIN, PAGE_H - 2.5 * cm, PAGE_W - MARGIN, PAGE_H - 2.5 * cm)

    # ── Main title block ──────────────────────────────────────────────────
    c.setFillColor(ACCENT)
    c.setFont("Helvetica-Bold", 32)
    c.drawCentredString(PAGE_W / 2, PAGE_H - 4.2 * cm, "HEAUV")

    c.setFillColor(WHITE)
    c.setFont("Helvetica-Bold", 20)
    c.drawCentredString(PAGE_W / 2, PAGE_H - 5.2 * cm,
                        "High Endurance Autonomous Underwater Vehicle")

    # ── Subtitle ──────────────────────────────────────────────────────────
    c.setFillColor(LIGHT_BLUE)
    c.setFont("Helvetica", 12)
    c.drawCentredString(PAGE_W / 2, PAGE_H - 6.2 * cm,
                        "Mission Telemetry Analysis Report")

    # ── AUV illustration ──────────────────────────────────────────────────
    auv_cx = PAGE_W / 2
    auv_cy = PAGE_H / 2 + 1.0 * cm        # centred with good gap above & below
    _draw_auv_illustration(c, auv_cx, auv_cy, width=420, height=160)

    # ── Mission info box ──────────────────────────────────────────────────
    run_name   = mission_info.get("run_name",   os.path.basename(run_folder))
    bin_name   = mission_info.get("bin_name",   "—")
    display_dt = mission_info.get("display_dt", "—")
    generated  = datetime.datetime.now().strftime("%d %B %Y   %H:%M hrs")

    info_y = PAGE_H / 2 - 6.8 * cm        # clear gap below AUV panel
    box_h  = 4.2 * cm

    # Box background
    c.setFillColor(STEEL_BLUE)
    c.roundRect(MARGIN, info_y, PAGE_W - 2 * MARGIN, box_h,
                6, fill=1, stroke=0)

    # Box border
    c.setStrokeColor(ACCENT)
    c.setLineWidth(1)
    c.roundRect(MARGIN, info_y, PAGE_W - 2 * MARGIN, box_h,
                6, fill=0, stroke=1)

    # Vertical divider
    mid_x = PAGE_W / 2
    c.setStrokeColor(colors.HexColor("#2E86C1"))
    c.setLineWidth(0.6)
    c.line(mid_x, info_y + 0.3 * cm, mid_x, info_y + box_h - 0.3 * cm)

    # ── Left column : Bin + Run ───────────────────────────────────────────
    left_cx = MARGIN + (mid_x - MARGIN) / 2

    c.setFillColor(LIGHT_BLUE)
    c.setFont("Helvetica", 8)
    c.drawCentredString(left_cx, info_y + 3.5 * cm, "BIN FOLDER")

    c.setFillColor(WHITE)
    c.setFont("Helvetica-Bold", 11)
    c.drawCentredString(left_cx, info_y + 2.85 * cm, bin_name)

    c.setFillColor(LIGHT_BLUE)
    c.setFont("Helvetica", 8)
    c.drawCentredString(left_cx, info_y + 2.1 * cm, "RUN")

    c.setFillColor(WHITE)
    c.setFont("Helvetica-Bold", 11)
    c.drawCentredString(left_cx, info_y + 1.5 * cm, run_name)

    # ── Right column : Mission datetime + Generated ───────────────────────
    right_cx = mid_x + (PAGE_W - MARGIN - mid_x) / 2

    c.setFillColor(LIGHT_BLUE)
    c.setFont("Helvetica", 8)
    c.drawCentredString(right_cx, info_y + 3.5 * cm, "MISSION EXECUTED")

    c.setFillColor(WHITE)
    c.setFont("Helvetica-Bold", 11)
    c.drawCentredString(right_cx, info_y + 2.85 * cm, display_dt)

    c.setFillColor(LIGHT_BLUE)
    c.setFont("Helvetica", 8)
    c.drawCentredString(right_cx, info_y + 2.1 * cm, "REPORT GENERATED")

    c.setFillColor(WHITE)
    c.setFont("Helvetica-Bold", 9)
    c.drawCentredString(right_cx, info_y + 1.5 * cm, generated)

    # ── Bottom bar ────────────────────────────────────────────────────────
    c.setFillColor(STEEL_BLUE)
    c.rect(0, 0, PAGE_W, 1.6 * cm, fill=1, stroke=0)

    c.setFillColor(ACCENT)
    c.rect(0, 1.6 * cm, PAGE_W, 0.15 * cm, fill=1, stroke=0)

    c.setFillColor(LIGHT_BLUE)
    c.setFont("Helvetica", 8)
    c.drawCentredString(PAGE_W / 2, 0.6 * cm,
                        "CONFIDENTIAL — FOR OFFICIAL USE ONLY")

    c.restoreState()
    c.showPage()


# ═════════════════════════════════════════════════════════════════════════════
#  Single parameter plot page
# ═════════════════════════════════════════════════════════════════════════════

def build_plot_page(c: rl_canvas.Canvas,
                    item: dict,
                    subsystem_name: str,
                    page_number: int = 1,
                    mission_info: dict = None):
    """
    Render one full landscape A4 page for a parameter.

    item keys (always):   label, units, plot_type
    plot_type = "line"    → time, signal
    plot_type = "scatter" → x, y, x_label, y_label
    plot_type = "dual"    → time, signal_a, signal_b, label_a, label_b
    """

    plot_type  = item.get("plot_type", "line")
    param_name = item["label"]
    units      = item.get("units", "")

    # Build mission string for top-right header
    if mission_info:
        bin_name = mission_info.get("bin_name", "")
        run_name = mission_info.get("run_name", "")
        mission_str = f"Mission Report — {bin_name}  |  {run_name}"
    else:
        mission_str = "Telemetry Analysis Report"

    c.saveState()

    # ── White page background ─────────────────────────────────────────────
    c.setFillColor(WHITE)
    c.rect(0, 0, PAGE_W, PAGE_H, fill=1, stroke=0)

    # ── Header strip ──────────────────────────────────────────────────────
    c.setFillColor(NAVY)
    c.rect(0, PAGE_H - 1.6 * cm, PAGE_W, 1.6 * cm, fill=1, stroke=0)

    c.setFillColor(ACCENT)
    c.rect(0, PAGE_H - 1.75 * cm, PAGE_W, 0.18 * cm, fill=1, stroke=0)

    c.setFillColor(WHITE)
    c.setFont("Helvetica-Bold", 11)
    c.drawString(MARGIN, PAGE_H - 1.1 * cm, f"HEAUV  |  {subsystem_name}")

    c.setFont("Helvetica", 9)
    c.drawRightString(PAGE_W - MARGIN, PAGE_H - 1.1 * cm, mission_str)

    # ── Parameter heading ─────────────────────────────────────────────────
    heading_y = PAGE_H - 2.8 * cm
    c.setFillColor(BLACK)
    c.setFont("Helvetica-Bold", 16)
    display = f"{param_name}" + (f"  [{units}]" if units else "")
    c.drawCentredString(PAGE_W / 2, heading_y, display)

    text_w = c.stringWidth(display, "Helvetica-Bold", 16)
    c.setStrokeColor(ACCENT)
    c.setLineWidth(1.2)
    c.line(PAGE_W / 2 - text_w / 2 - 8, heading_y - 4,
           PAGE_W / 2 + text_w / 2 + 8, heading_y - 4)

    # ── Matplotlib figure ─────────────────────────────────────────────────
    fig_w_in = PLOT_W / (72 * cm / cm)
    fig_h_in = PLOT_H / (72 * cm / cm)

    fig, ax = plt.subplots(figsize=(fig_w_in * 1.38, fig_h_in * 1.38), dpi=150)
    fig.patch.set_facecolor("white")
    ax.set_facecolor("white")

    # ── Draw correct plot type ────────────────────────────────────────────
    if plot_type == "line":
        time   = item["time"]
        signal = item["signal"]
        ax.plot(time, signal, color=PLOT_LINE, linewidth=1.4, antialiased=True)
        ax.set_xlabel("Time (Sec)", fontsize=11, fontweight="bold", color="#1A1A1A")
        y_lbl = f"{param_name}" + (f" ({units})" if units else "")
        ax.set_ylabel(y_lbl, fontsize=11, fontweight="bold", color="#1A1A1A")

    elif plot_type == "scatter":
        x       = item["x"]
        y       = item["y"]
        x_label = item.get("x_label", "X")
        y_label = item.get("y_label", "Y")
        ax.plot(x, y, color="#C0392B", linewidth=0, marker=".",
                markersize=2.5, alpha=0.8)
        ax.set_xlabel(x_label, fontsize=11, fontweight="bold", color="#1A1A1A")
        ax.set_ylabel(y_label, fontsize=11, fontweight="bold", color="#1A1A1A")
        ax.set_aspect("equal", adjustable="datalim")

    
# ONLY showing the UPDATED PART — rest of your file remains EXACTLY SAME

    elif plot_type == "dual":
        time    = item["time"]
        sig_a   = item["signal_a"]
        sig_b   = item["signal_b"]
        label_a = item.get("label_a", "Signal A")
        label_b = item.get("label_b", "Signal B")

    # ── Original dual lines (UNCHANGED) ─────────────────────────
        ax.plot(time, sig_a, color="#1A5276", linewidth=1.4,
            label=label_a, antialiased=True)

        ax.plot(time, sig_b, color="#C0392B", linewidth=1.4,
            label=label_b, antialiased=True, linestyle="--")

    # ── NEW: Extra signals support (NON-BREAKING EXTENSION) ─────
        extra_signals = item.get("extra_signals", [])
        extra_labels  = item.get("extra_labels", [])

    # Using same visual style family (no design change)
        extra_colors = ["#27AE60", "#8E44AD", "#F39C12"]

        for i, sig in enumerate(extra_signals):
            if sig is None:
                continue
            ax.plot(
                time,
                sig,
                linewidth=1.4,
                label=extra_labels[i] if i < len(extra_labels) else f"Extra {i+1}",
                color=extra_colors[i % len(extra_colors)],
                linestyle=":"   # subtle variation, same style family
            )

    # ── Labels (UNCHANGED STYLE) ────────────────────────────────
        ax.set_xlabel("Time (Sec)", fontsize=11, fontweight="bold", color="#1A1A1A")
        y_lbl = units if units else "Value"
        ax.set_ylabel(y_lbl, fontsize=11, fontweight="bold", color="#1A1A1A")

    # ── Legend (UNCHANGED STYLE) ────────────────────────────────
        leg = ax.legend(fontsize=9, loc="best", framealpha=0.9,
                    edgecolor="#CCCCCC")
        leg.get_frame().set_linewidth(0.8)


    # ── Grid styling (white bg + mini-square grid) ────────────────────────
    ax.grid(which="major", color="#CCCCCC", linewidth=0.7, linestyle="-")
    ax.grid(which="minor", color="#E8E8E8", linewidth=0.4, linestyle="-")
    ax.minorticks_on()
    ax.xaxis.set_minor_locator(ticker.AutoMinorLocator(5))
    ax.yaxis.set_minor_locator(ticker.AutoMinorLocator(5))

    ax.tick_params(axis="both", which="major", labelsize=9,
                   direction="in", length=5, color="#666666")
    ax.tick_params(axis="both", which="minor",
                   direction="in", length=3, color="#AAAAAA")

    for spine in ax.spines.values():
        spine.set_edgecolor("#333333")
        spine.set_linewidth(0.9)

    # ── Axis formatting ───────────────────────────────────────────────────
    if plot_type == "scatter":
        # Scatter (Lat/Long, XY Trajectory) — plain numbers on both axes
        ax.xaxis.set_major_formatter(ticker.ScalarFormatter())
        ax.yaxis.set_major_formatter(ticker.ScalarFormatter())
        ax.ticklabel_format(style="plain", axis="both")
    else:
        # Line / Dual — X axis always plain absolute seconds, no sci notation
        ax.xaxis.set_major_formatter(
            ticker.FuncFormatter(lambda x, _: f"{int(x)}")
        )
        # Y axis — plain notation, no long decimals, no sci notation offset
        sf = ticker.ScalarFormatter(useOffset=False, useMathText=False)
        sf.set_powerlimits((-4, 5))
        ax.yaxis.set_major_formatter(sf)

    plt.tight_layout(pad=0.6)

    # ── Save to memory buffer — nothing written to disk ───────────────────
    buf = io.BytesIO()
    fig.savefig(buf, format="png", dpi=150,
                facecolor="white", bbox_inches="tight")
    buf.seek(0)
    plt.close(fig)

    # ── Embed into PDF via ImageReader (Windows-safe) ─────────────────────
    img_reader = ImageReader(buf)

    plot_y = 1.8 * cm
    c.drawImage(
        img_reader,
        MARGIN, plot_y,
        width=PLOT_W, height=PLOT_H - 1.0 * cm,
        preserveAspectRatio=True,
        mask="auto"
    )
    buf.close()

    # ── Footer ────────────────────────────────────────────────────────────
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