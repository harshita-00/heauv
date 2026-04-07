"""
mission_sequence.py
-------------------
Builds the Mission Sequence Table page from the Preset Parameters .dat file.

Columns used (0-based):
    0 = Leg Duration
    1 = Speed
    2 = Depth
    3 = Yaw Ref
    4 = Turn Rate
    5 = DCO       (skipped)
    6 = CCO       (skipped)
    7 = Search Side
    8 = Search Pattern
    9 = RCL       (skipped)

NOTE: No matplotlib import here — importing matplotlib in this module
      would reset the Agg backend set by pdf_builder.py and corrupt
      the X-axis formatting on all plot pages.
"""

import os
import sys

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", "..", ".."))
if ROOT not in sys.path:
    sys.path.insert(0, ROOT)

import numpy as np
from reportlab.lib.pagesizes import A4, landscape
from reportlab.lib import colors
from reportlab.lib.units import cm
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.enums import TA_CENTER
from reportlab.platypus import Paragraph, Table, TableStyle
from reportlab.pdfgen import canvas as rl_canvas

from reports.utils.data_extractor import load_dat_file

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

# Columns to extract: (index, header label)
COLUMNS = [
    (0, "Leg Duration"),
    (1, "Speed"),
    (2, "Depth"),
    (3, "Yaw Ref"),
    (4, "Turn Rate"),
    (7, "Search Side"),
    (8, "Search Pattern"),
]


def _find_preset_file(run_folder: str) -> str | None:
    """Locate the Preset Parameters .dat file."""
    for fname in os.listdir(run_folder):
        clean = fname.lstrip("_").lower()
        if fname.endswith(".dat") and "preset" in clean:
            return os.path.join(run_folder, fname)
    return None


def build_mission_sequence_page(c: rl_canvas.Canvas,
                                  run_folder: str,
                                  page_number: int,
                                  mission_info: dict) -> bool:
    """
    Draw the Mission Sequence Table on a single landscape A4 page.
    Returns True if page was drawn, False if file not found.
    """
    path = _find_preset_file(run_folder)
    if path is None:
        return False

    data = load_dat_file(path)
    if data is None:
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
    c.drawString(MARGIN, PAGE_H - 1.1 * cm, "HEAUV  |  MISSION SEQUENCE")

    c.setFont("Helvetica", 9)
    c.drawRightString(PAGE_W - MARGIN, PAGE_H - 1.1 * cm, mission_str)

    # ── Section heading (identical to all other pages) ────────────────────
    heading_y = PAGE_H - 2.8 * cm
    c.setFillColor(BLACK)
    c.setFont("Helvetica-Bold", 16)
    c.drawCentredString(PAGE_W / 2, heading_y, "Mission Sequence")

    text_w = c.stringWidth("Mission Sequence", "Helvetica-Bold", 16)
    c.setStrokeColor(ACCENT)
    c.setLineWidth(1.2)
    c.line(PAGE_W / 2 - text_w / 2 - 8, heading_y - 4,
           PAGE_W / 2 + text_w / 2 + 8, heading_y - 4)

    # ── Build table ───────────────────────────────────────────────────────
    hdr_style = ParagraphStyle("hdr",
        fontName="Helvetica-Bold", fontSize=10,
        textColor=WHITE, alignment=TA_CENTER)

    cell_style = ParagraphStyle("cell",
        fontName="Helvetica", fontSize=9,
        textColor=colors.HexColor("#1A1A1A"), alignment=TA_CENTER)

    headers   = ["Leg #"] + [col[1] for col in COLUMNS]
    table_data = [[Paragraph(h, hdr_style) for h in headers]]

    total_cols = data.shape[1]
    for leg_idx, row in enumerate(data):
        cells = [Paragraph(str(leg_idx + 1), cell_style)]
        for col_idx, _ in COLUMNS:
            if col_idx < total_cols:
                val = row[col_idx]
                txt = str(int(val)) if val == int(val) else f"{val:.4f}"
            else:
                txt = "—"
            cells.append(Paragraph(txt, cell_style))
        table_data.append(cells)

    # Column widths
    usable_w   = PAGE_W - 2 * MARGIN
    leg_col_w  = 1.5 * cm
    data_col_w = (usable_w - leg_col_w) / len(COLUMNS)
    col_widths = [leg_col_w] + [data_col_w] * len(COLUMNS)

    # Alternating row backgrounds
    row_count = len(table_data)
    row_bgs   = []
    for i in range(1, row_count):
        colour = ROW_ALT if i % 2 == 1 else WHITE
        row_bgs.append(('BACKGROUND', (0, i), (-1, i), colour))

    tbl = Table(table_data, colWidths=col_widths, repeatRows=1)
    tbl.setStyle(TableStyle([
        ('BACKGROUND',    (0, 0), (-1, 0),  NAVY),
        ('TOPPADDING',    (0, 0), (-1, 0),  8),
        ('BOTTOMPADDING', (0, 0), (-1, 0),  8),
        ('TOPPADDING',    (0, 1), (-1, -1), 6),
        ('BOTTOMPADDING', (0, 1), (-1, -1), 6),
        ('VALIGN',        (0, 0), (-1, -1), 'MIDDLE'),
        ('ALIGN',         (0, 0), (-1, -1), 'CENTER'),
        ('GRID',          (0, 0), (-1, -1), 0.5, colors.HexColor('#BDC3C7')),
        ('LINEABOVE',     (0, 1), (-1, 1),  1.5, ACCENT),
        ('BOX',           (0, 0), (-1, -1), 1.5, NAVY),
    ] + row_bgs))

    table_top_y  = heading_y - 0.6 * cm
    tbl_w, tbl_h = tbl.wrap(usable_w, table_top_y - 1.8 * cm)
    tbl.drawOn(c, MARGIN, table_top_y - tbl_h)

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