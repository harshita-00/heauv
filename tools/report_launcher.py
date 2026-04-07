"""
report_launcher.py
------------------
Standalone launcher for the HEAUV report generator.
Lives in  heauv/tools/report_launcher.py

Opens a native file-dialog so the user can select a RUN folder,
auto-extracts datetime from .dat filenames, names the PDF smartly,
then generates the PDF and shows a confirmation dialog.

Run directly:
    python tools/report_launcher.py

Or called from app.py later:
    from tools.report_launcher import launch_report_dialog
    launch_report_dialog()
"""

import os
import re
import sys

# ── Fix path so 'reports' module is always found ─────────────────────────────
ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
if ROOT not in sys.path:
    sys.path.insert(0, ROOT)

from PyQt5.QtWidgets import (
    QApplication, QFileDialog, QMessageBox,
    QProgressDialog, QWidget
)
from PyQt5.QtCore import Qt, QThread, pyqtSignal

from reports.report_builder import generate_report


# ═════════════════════════════════════════════════════════════════════════════
#  Helpers — extract info from run folder
# ═════════════════════════════════════════════════════════════════════════════

def extract_datetime_from_dat(run_folder: str) -> str:
    """
    Scan .dat files in the run folder and extract the datetime string.

    Matches filenames ending with:
        ..._D-M-YYYY_HH-MM-SS.dat      (single-digit day/month)
        ..._DD-MM-YYYY_HH-MM-SS.dat    (zero-padded)

    Returns formatted string like '09-03-2026_20-20-39'
    or 'Unknown' if no match found.
    """
    # Flexible pattern: date = 1-2 digits, any separator count
    # Time = HH-MM-SS where each part is 1-2 digits
    pattern = re.compile(
        r'_(\d{1,2}-\d{1,2}-\d{4})_(\d{1,2}-\d{1,2}-\d{1,2})\.dat$',
        re.IGNORECASE
    )

    for fname in os.listdir(run_folder):
        if not fname.endswith(".dat"):
            continue
        match = pattern.search(fname)
        if match:
            date_part = match.group(1)   # e.g. 9-3-2026
            time_part = match.group(2)   # e.g. 20-20-39

            # Zero-pad day and month for clean display
            d, m, y = date_part.split("-")
            date_padded = f"{int(d):02d}-{int(m):02d}-{y}"

            # Zero-pad time parts too
            h, mi, s = time_part.split("-")
            time_padded = f"{int(h):02d}-{int(mi):02d}-{int(s):02d}"

            return f"{date_padded}_{time_padded}"

    return "Unknown"


def extract_mission_info(run_folder: str) -> dict:
    """
    Extract all metadata from the run folder path and .dat filenames.

    Returns dict with:
        run_name    : e.g. 'RUN-0'
        bin_name    : e.g. 'bbbin'
        datetime_str: e.g. '09-03-2026_20-20-39'
        display_dt  : e.g. '09-03-2026  20:20:39'
    """
    run_name    = os.path.basename(run_folder)
    bin_name    = os.path.basename(os.path.dirname(run_folder))
    datetime_str = extract_datetime_from_dat(run_folder)

    # Human-readable version for cover page
    if datetime_str != "Unknown":
        date_p, time_p = datetime_str.split("_")
        time_display   = time_p.replace("-", ":")
        display_dt     = f"{date_p}  {time_display}"
    else:
        display_dt = "Unknown"

    return {
        "run_name":     run_name,
        "bin_name":     bin_name,
        "datetime_str": datetime_str,
        "display_dt":   display_dt,
    }


def build_pdf_filename(run_folder: str) -> str:
    """
    Build the default PDF output filename.
    Format: HEAUV_Report_<RUN-NAME>_<DD-MM-YYYY>_<HH-MM-SS>.pdf
    Example: HEAUV_Report_RUN-0_09-03-2026_20-20-39.pdf
    """
    info     = extract_mission_info(run_folder)
    run_name = info["run_name"]
    dt_str   = info["datetime_str"]

    return f"HEAUV_Report_{run_name}_{dt_str}.pdf"


# ═════════════════════════════════════════════════════════════════════════════
#  Worker thread — keeps the GUI responsive while PDF builds
# ═════════════════════════════════════════════════════════════════════════════

class ReportWorker(QThread):
    finished = pyqtSignal(bool, str)   # (success, output_path)
    error    = pyqtSignal(str)

    def __init__(self, run_folder: str, output_path: str, mission_info: dict):
        super().__init__()
        self.run_folder   = run_folder
        self.output_path  = output_path
        self.mission_info = mission_info

    def run(self):
        try:
            ok = generate_report(
                run_folder   = self.run_folder,
                output_path  = self.output_path,
                mission_info = self.mission_info,
            )
            self.finished.emit(ok, self.output_path)
        except Exception as e:
            self.error.emit(str(e))


# ═════════════════════════════════════════════════════════════════════════════
#  Public launcher function
# ═════════════════════════════════════════════════════════════════════════════

def launch_report_dialog(parent: QWidget = None):
    """
    Show folder-picker → extract metadata → generate PDF → show result.
    Safe to call from app.py or any PyQt5 context.
    """

    # ── 1. Pick run folder ────────────────────────────────────────────────
    run_folder = QFileDialog.getExistingDirectory(
        parent,
        "Select Run Folder for Report",
        os.path.expanduser("~"),
        QFileDialog.ShowDirsOnly | QFileDialog.DontResolveSymlinks,
    )

    if not run_folder:
        return                              # user cancelled

    # ── 2. Extract mission metadata ───────────────────────────────────────
    mission_info = extract_mission_info(run_folder)

    # ── 3. Build smart default PDF filename ───────────────────────────────
    pdf_filename  = build_pdf_filename(run_folder)
    default_path  = os.path.join(os.path.dirname(run_folder), pdf_filename)

    # ── 4. Let user confirm / change save path ────────────────────────────
    output_path, _ = QFileDialog.getSaveFileName(
        parent,
        "Save Report As",
        default_path,
        "PDF Files (*.pdf)",
    )

    if not output_path:
        return                              # user cancelled

    if not output_path.endswith(".pdf"):
        output_path += ".pdf"

    # ── 5. Progress dialog ────────────────────────────────────────────────
    progress = QProgressDialog("Generating HEAUV Report…", None, 0, 0, parent)
    progress.setWindowTitle("HEAUV Report")
    progress.setWindowModality(Qt.WindowModal)
    progress.setMinimumDuration(0)
    progress.setValue(0)
    progress.show()

    # ── 6. Background worker ──────────────────────────────────────────────
    worker = ReportWorker(run_folder, output_path, mission_info)

    def on_finished(ok: bool, path: str):
        progress.close()
        if ok:
            QMessageBox.information(
                parent, "Report Ready",
                f"✅  Report saved successfully!\n\n{path}"
            )
        else:
            QMessageBox.warning(
                parent, "No Data",
                "⚠️  No matching .dat files found in the selected folder.\n"
                "Please check the run folder and try again."
            )

    def on_error(msg: str):
        progress.close()
        QMessageBox.critical(
            parent, "Report Error",
            f"❌  An error occurred while generating the report:\n\n{msg}"
        )

    worker.finished.connect(on_finished)
    worker.error.connect(on_error)
    worker.start()

    # Keep reference so thread isn't garbage-collected
    if not hasattr(launch_report_dialog, "_workers"):
        launch_report_dialog._workers = []
    launch_report_dialog._workers.append(worker)


# ═════════════════════════════════════════════════════════════════════════════
#  Entry point when run directly
# ═════════════════════════════════════════════════════════════════════════════

if __name__ == "__main__":
    app = QApplication(sys.argv)
    launch_report_dialog()
    sys.exit(app.exec_())