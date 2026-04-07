
"""
report_builder.py
-----------------
Orchestrates the full PDF report:
  1. Cover page
  2. Mission Sequence
  3. Mission Statistics
  4. One page per parameter across all subsystems

Usage:
    from reports.report_builder import generate_report
    generate_report(run_folder, output_path, mission_info)
"""

import os
import sys

# ── Fix path ──────────────────────────────────────────────────────────────────
ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
if ROOT not in sys.path:
    sys.path.insert(0, ROOT)

# ✅ IMPORTANT: Keep this import (fixes your error)
from reportlab.pdfgen import canvas as rl_canvas
from reportlab.lib.pagesizes import A4, landscape

from reports.utils.pdf_builder import build_cover_page, build_plot_page

# ── Import every subsystem module ─────────────────────────────────────────────
from reports.subsystems import control, eassc, dvl, smc, motor

# ── Import summary page builders ─────────────────────────────────────────────
from reports.subsystems.mission_sequence import build_mission_sequence_page
from reports.subsystems.mission_statistics import build_mission_statistics_page

SUBSYSTEM_MODULES = [
    control,
    eassc,
    dvl,
    smc,
    motor,
]


# ═════════════════════════════════════════════════════════════════════════════

def generate_report(run_folder: str, output_path: str,
                    mission_info: dict = None) -> bool:
    """
    Build the full PDF report for the given run folder.
    """

    # Fallback if mission_info not provided
    if mission_info is None:
        mission_info = {
            "run_name":   os.path.basename(run_folder),
            "bin_name":   os.path.basename(os.path.dirname(run_folder)),
            "display_dt": "Unknown",
        }

    c = rl_canvas.Canvas(output_path, pagesize=landscape(A4))

    # ── Page 1 : Cover ────────────────────────────────────────────────────
    build_cover_page(c, run_folder, mission_info)

    page_num = 2
    any_plotted = False

    # ── Mission Sequence (NOW AFTER COVER) ────────────────────────────────
    drawn = build_mission_sequence_page(
        c=c,
        run_folder=run_folder,
        page_number=page_num,
        mission_info=mission_info,
    )
    if drawn:
        page_num += 1
        any_plotted = True

    # ── Mission Statistics (AFTER SEQUENCE) ───────────────────────────────
    drawn = build_mission_statistics_page(
        c=c,
        run_folder=run_folder,
        page_number=page_num,
        mission_info=mission_info,
    )
    if drawn:
        page_num += 1
        any_plotted = True

    # ── Subsystem plot pages (UNCHANGED LOGIC) ────────────────────────────
    for module in SUBSYSTEM_MODULES:

        datasets = module.get_plot_data(run_folder)

        if not datasets:
            continue

        for item in datasets:
            build_plot_page(
                c=c,
                item=item,
                subsystem_name=module.SUBSYSTEM_NAME,
                page_number=page_num,
                mission_info=mission_info,
            )
            page_num += 1
            any_plotted = True

    # ── Safety fallback ───────────────────────────────────────────────────
    if not any_plotted:
        c.showPage()

    c.save()
    return any_plotted

