from flask import Flask, render_template, request, jsonify, send_file
import subprocess
import os
import numpy as np
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import sys
import tkinter as tk
from tkinter import filedialog
import time as time_module
import platform
import threading

sys.path.append(os.path.join(os.getcwd(), "telemetry-viewer"))

from config.telemetry_config import TELEMETRY_PARAMETERS, TIME_COLUMN

app = Flask(__name__)

UPLOAD_FOLDER = "uploads"
os.makedirs(UPLOAD_FOLDER, exist_ok=True)

_plot_run_folder = ""
_extract_run_folder = ""

# Progress tracking state
_progress_state = {
    "progress": 0.0,
    "time_remaining": "--",
    "is_processing": False
}

# Maps the word(s) at position [1] in the filename to a config key.
FILENAME_TO_SUBSYSTEM = {
    "CONTROL"       : "CONTROL",
    "TSU"           : "TSU",
    "BLDC"          : "BLDC",
    "EASSC"         : "EASSC",
    "SMC"           : "SMC",
    "BMS"           : "BMS",
    "DVL"           : "DVL",
    "REMOTE"        : "REMOTE",
    "HEALTH"        : "HEALTH",
    "CONTROLGAINS"  : "CONTROLGAINS",
    "SEQLEGS"       : "SEQLEGS",
    "PRESETPARAMETERS": "PRESETPARAMETERS",
    "IPS"           : "IPS",
    "RUNTERMINATION": "RUNTERMINATION",
    "MCSRTC"        : "MCSRTC",
    "SONAR"         : "SONAR",
    "CTD"           : "CTD",
    "SECSS"         : "SECSS",
    "IPSDETECTION"  : "IPSDETECTION",
    "RECOVERY"      : "RECOVERY",
    "MAST"          : "MAST",
    # aliases found in actual filenames
    "GAINS"         : "CONTROLGAINS",
    "GAINS_FILE"    : "CONTROLGAINS",
    "HEALTH_FILE"   : "HEALTH",
    "REMOTE_FILE"   : "REMOTE",
    "SEQ_LEGS_FILE" : "SEQLEGS",
    "SEQ"           : "SEQLEGS",
    "PRESET_PARAMETERS_FILE": "PRESETPARAMETERS",
    "PRESET"        : "PRESETPARAMETERS",
    "MCS_RTC"       : "MCSRTC",
    "MCSRTC"        : "MCSRTC",
    "SONAR_DATA_FILE": "SONAR",
    "IPS_DATA_FILE" : "IPS",
    "SECSS_DATA_FILE": "SECSS",
    "MAST_PACKET_DATA_FILE": "MAST",
    "DETECTION_PACKET_DATA_FILE": "IPSDETECTION",
    "RECOVERY_CONDITIONS_PACKET_DATA_FILE": "RECOVERY",
    "RUNTERMINATION_DATA_FILE": "RUNTERMINATION",
    "CTD_DATA_FILE" : "CTD",
}


def detect_subsystem(file_name):
    """
    Filename format: RUN-1_<SubsystemWords...>_DD-M-YYYY_HH-MM-SS.dat
    Strategy: strip the run prefix (part[0]) and the trailing date+time
    (last two parts that match digit patterns), then join what remains
    and look it up in FILENAME_TO_SUBSYSTEM.
    """
    base = file_name.replace(".dat", "")
    parts = base.split("_")

    # Drop part[0] which is always the run ID e.g. "RUN-1"
    if len(parts) < 2:
        return None
    parts = parts[1:]

    # Drop trailing date and time parts (contain '-' and are mostly digits)
    while parts and any(c.isdigit() for c in parts[-1]) and '-' in parts[-1]:
        parts = parts[:-1]

    if not parts:
        return None

    # Try progressively shorter prefixes (longest match first)
    for length in range(len(parts), 0, -1):
        candidate = "_".join(parts[:length]).upper()
        if candidate in FILENAME_TO_SUBSYSTEM:
            key = FILENAME_TO_SUBSYSTEM[candidate]
            if key in TELEMETRY_PARAMETERS:
                return key

    return None


@app.route("/")
def home():
    return render_template("index.html")


# ============================================
# EXTRACTION ENDPOINTS (CROSS-PLATFORM)
# ============================================

@app.route("/pick-extract-folder", methods=["GET"])
def pick_extract_folder():
    global _extract_run_folder

    root = tk.Tk()
    root.withdraw()
    root.attributes("-topmost", True)
    root.lift()
    root.focus_force()
    root.update()

    folder = filedialog.askdirectory(
        parent=root,
        title="Select Folder Containing BIN Files"
    )
    root.destroy()

    if not folder:
        return jsonify({"status": "cancelled", "message": "No folder selected"})

    _extract_run_folder = folder
    bin_files = [f for f in os.listdir(folder) if f.lower().endswith(".bin")]

    return jsonify({
        "status": "success",
        "folder": folder,
        "message": f"{os.path.basename(folder)}  ({len(bin_files)} .bin file(s) found)"
    })


@app.route("/extract", methods=["POST"])
def extract_folder():
    """
    Extract BIN files using MyApp executable (cross-platform)
    Uses native folder path selected via tkinter — no file upload needed
    """
    global _extract_run_folder

    if not _extract_run_folder:
        return jsonify({"status": "error", "message": "No folder selected. Please click Browse first."})

    if not os.path.isdir(_extract_run_folder):
        return jsonify({"status": "error", "message": f"Folder not found: {_extract_run_folder}"})

    bin_files = [f for f in os.listdir(_extract_run_folder) if f.lower().endswith(".bin")]

    if not bin_files:
        return jsonify({"status": "error", "message": "No .bin files found in the selected folder."})

    saved_files = bin_files  # kept so success message count stays the same
    bin_folder = os.path.abspath(_extract_run_folder)

    print("BIN folder detected:", bin_folder)

    # 🌍 CROSS-PLATFORM: Detect OS and get correct executable path
    BASE_DIR = os.path.dirname(os.path.abspath(__file__))
    system = platform.system()

    if system == "Windows":
        extractor_path = os.path.join(
            BASE_DIR,
            "MyApp",
            "bin",
            "Debug",
            "net10.0",
            "win-x64",
            "MyApp.exe"
        )
    elif system == "Linux":
        extractor_path = os.path.join(
            BASE_DIR,
            "MyApp",
            "bin",
            "Debug",
            "net10.0",
            "linux-x64",
            "MyApp"
        )
    elif system == "Darwin":  # macOS
        extractor_path = os.path.join(
            BASE_DIR,
            "MyApp",
            "bin",
            "Debug",
            "net10.0",
            "osx-x64",
            "MyApp"
        )
    else:
        return jsonify({
            "status": "error",
            "message": f"Unsupported platform: {system}"
        })

    print("Running extractor:", extractor_path)
    print("Platform detected:", system)

    try:
        global _progress_state
        
        # Mark extraction as started
        _progress_state["is_processing"] = True
        _progress_state["progress"] = 0.0
        _progress_state["time_remaining"] = "Starting extraction..."
        
        start_time = time_module.time()
        extraction_complete = [False]  # Use list to allow modification in nested function
        
        # Define progress simulator function
        def simulate_extraction_progress():
            """Simulate smooth progress while extraction runs"""
            try:
                while _progress_state["is_processing"] and not extraction_complete[0]:
                    elapsed = time_module.time() - start_time
                    # Smooth progress: 0% -> 95% over time
                    # Progress increases gradually, capping at 95% until extraction finishes
                    progress = min(0.95, (elapsed / 120.0))  # Smooth curve over ~120 seconds
                    
                    if progress > _progress_state["progress"]:
                        _progress_state["progress"] = progress
                        _progress_state["time_remaining"] = "Extracting..."
                    
                    time_module.sleep(0.3)
            except Exception as e:
                print(f"Progress simulation error: {e}")
        
        # Start progress simulator in background thread
        progress_thread = threading.Thread(target=simulate_extraction_progress, daemon=True)
        progress_thread.start()
        
        # Run actual extraction
        subprocess.run([extractor_path, bin_folder], check=True)
        
        # Mark extraction as complete
        extraction_complete[0] = True
        _progress_state["is_processing"] = False
        _progress_state["progress"] = 1.0
        _progress_state["time_remaining"] = "Complete!"
        
        time_module.sleep(0.5)  # Give thread time to finish

        return jsonify({
            "status": "success",
            "message": f"Extraction completed for {len(saved_files)} BIN files"
        })

    except Exception as e:
        _progress_state["is_processing"] = False
        return jsonify({
            "status": "error",
            "message": str(e)
        })


# ============================================
# PLOTTING ENDPOINTS (WITH PROGRESS BAR)
# ============================================

@app.route("/pick-plot-folder", methods=["GET"])
def pick_plot_folder():
    """
    Open native OS folder picker for selecting run folder
    Stores the path server-side
    """
    global _plot_run_folder

    root = tk.Tk()
    root.withdraw()
    root.attributes("-topmost", True)
    root.lift()
    root.focus_force()
    root.update()

    folder = filedialog.askdirectory(
        parent=root,
        title="Select Run Folder for Plotting"
    )
    root.destroy()

    if not folder:
        return jsonify({"status": "cancelled", "message": "No folder selected"})

    _plot_run_folder = folder

    dat_files = [f for f in os.listdir(folder) if f.endswith(".dat")]

    return jsonify({
        "status": "success",
        "folder": folder,
        "message": f"{os.path.basename(folder)}  ({len(dat_files)} .dat file(s) found)"
    })


@app.route("/plot-all", methods=["POST"])
def plot_all_images():
    """
    Generate plots from DAT files with real-time progress tracking
    Updates progress state for frontend polling
    """
    global _plot_run_folder, _progress_state

    run_folder = _plot_run_folder

    if not run_folder:
        return jsonify({"status": "error", "message": "No run folder selected. Please click Browse first."})

    if not os.path.isdir(run_folder):
        return jsonify({"status": "error", "message": f"Folder not found: {run_folder}"})

    dat_files = [f for f in os.listdir(run_folder) if f.endswith(".dat")]

    if not dat_files:
        return jsonify({"status": "error", "message": "No .dat files found in the selected folder."})

    plots_saved = 0
    skipped = []
    start_time = time_module.time()
    total_files = len(dat_files)
    
    # Mark processing as started
    _progress_state["is_processing"] = True
    _progress_state["progress"] = 0.0

    try:
        for file_idx, file_name in enumerate(dat_files):

            dat_path = os.path.join(run_folder, file_name)

            # --- detect subsystem ---
            subsystem = detect_subsystem(file_name)

            if subsystem is None:
                skipped.append(f"{file_name} (subsystem not recognised)")
                print(f"[plot-all] SKIP (no subsystem match): {file_name}")
                continue

            # --- parse rows ---
            rows = []
            with open(dat_path, "r") as f:
                for line in f:
                    parts = line.strip().split()
                    if not parts:
                        continue
                    try:
                        rows.append([float(x) for x in parts])
                    except ValueError:
                        continue

            if len(rows) == 0:
                skipped.append(f"{file_name} (empty)")
                continue

            data_arr = np.array(rows)
            params    = TELEMETRY_PARAMETERS[subsystem]

            # --- save inside: <run_folder>/<SUBSYSTEM>/ ---
            subsystem_folder = os.path.join(run_folder, subsystem)
            os.makedirs(subsystem_folder, exist_ok=True)

            print(f"[plot-all] {file_name} -> {subsystem} ({len(params)} params, shape {data_arr.shape}) -> {subsystem_folder}")

            # --- time axis ---
            try:
                time = data_arr[:, TIME_COLUMN]
                if time.max() - time.min() == 0:
                    raise ValueError("flat time")
            except Exception:
                time = np.arange(len(data_arr))

            # --- one plot per parameter ---
            for col, param in enumerate(params):

                if col >= data_arr.shape[1]:
                    break

                signal = data_arr[:, col]

                fig, ax = plt.subplots(figsize=(8, 4))
                ax.plot(time, signal)
                ax.set_xlabel("Time")
                ax.set_ylabel(param)
                ax.set_title(f"{subsystem} — {param}")
                fig.tight_layout()

                out_path = os.path.join(subsystem_folder, f"{param}.png")
                fig.savefig(out_path, dpi=100)
                plt.close(fig)
                plots_saved += 1

            # --- calculate progress and time remaining ---
            elapsed = time_module.time() - start_time
            progress_ratio = (file_idx + 1) / total_files
            
            if progress_ratio > 0 and elapsed > 0:
                estimated_total_time = elapsed / progress_ratio
                time_remaining_sec = estimated_total_time - elapsed
            else:
                time_remaining_sec = 0
            
            # Format time remaining
            mins, secs = divmod(int(time_remaining_sec), 60)
            hours, mins = divmod(mins, 60)
            
            if hours > 0:
                time_str = f"{hours}h {mins}m {secs}s"
            elif mins > 0:
                time_str = f"{mins}m {secs}s"
            else:
                time_str = f"{secs}s"
            
            # Update progress state
            _progress_state["progress"] = progress_ratio
            _progress_state["time_remaining"] = time_str
            
            progress_percent = progress_ratio * 100
            print(f"[progress] {progress_percent:.1f}% | {time_str} remaining")

        msg = f"Saved {plots_saved} plot(s) inside {run_folder}"
        if skipped:
            msg += f"\nSkipped: {', '.join(skipped)}"

        print(f"[plot-all] DONE — {msg}")
        
        # Mark processing as complete
        _progress_state["is_processing"] = False
        _progress_state["progress"] = 1.0
        _progress_state["time_remaining"] = "Complete!"
        
        return jsonify({"status": "success", "message": msg})

    except Exception as e:
        import traceback
        traceback.print_exc()
        _progress_state["is_processing"] = False
        return jsonify({"status": "error", "message": str(e)})


@app.route("/get-progress", methods=["GET"])
def get_progress():
    """
    Endpoint for frontend polling
    Returns current progress, time remaining, and processing status
    """
    global _progress_state
    return jsonify({
        "progress": _progress_state["progress"],
        "time_remaining": _progress_state["time_remaining"],
        "is_processing": _progress_state["is_processing"]
    })


@app.route("/open-interactive")
def open_interactive():
    """
    Open the interactive telemetry viewer
    """
    try:
        viewer_path = os.path.join(os.getcwd(), "telemetry-viewer", "main.py")
        python_path = sys.executable
        subprocess.Popen([python_path, viewer_path, _plot_run_folder])
        return jsonify({"status": "success", "message": "Telemetry Viewer opened"})
    except Exception as e:
        return jsonify({"status": "error", "message": str(e)})




# ============================================
# REPORT ENDPOINTS
# ============================================

_report_run_folder = ""
_last_report_path  = ""


@app.route("/pick-report-folder", methods=["GET"])
def pick_report_folder():
    global _report_run_folder

    root = tk.Tk()
    root.withdraw()
    root.attributes("-topmost", True)
    root.lift()
    root.focus_force()
    root.update()

    folder = filedialog.askdirectory(
        parent=root,
        title="Select Run Folder for Report"
    )
    root.destroy()

    if not folder:
        return jsonify({"status": "cancelled", "message": "No folder selected"})

    _report_run_folder = folder
    dat_files = [f for f in os.listdir(folder) if f.endswith(".dat")]

    return jsonify({
        "status": "success",
        "folder": folder,
        "message": f"{os.path.basename(folder)}  ({len(dat_files)} .dat file(s) found)"
    })


@app.route("/generate-report", methods=["POST"])
def generate_report_endpoint():
    global _report_run_folder, _last_report_path

    run_folder = _report_run_folder

    if not run_folder:
        return jsonify({"status": "error", "message": "No run folder selected. Please click Browse first."})

    if not os.path.isdir(run_folder):
        return jsonify({"status": "error", "message": f"Folder not found: {run_folder}"})

    try:
        sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
        from tools.report_launcher import extract_mission_info, build_pdf_filename
        from reports.report_builder import generate_report

        # Reset and start progress simulation (same pattern as extraction)
        _progress_state["is_processing"] = True
        _progress_state["progress"] = 0.0
        _progress_state["time_remaining"] = "Generating report..."

        report_complete = [False]
        start_time = time_module.time()

        def simulate_report_progress():
            """Simulate smooth progress while report generates"""
            try:
                while _progress_state["is_processing"] and not report_complete[0]:
                    elapsed = time_module.time() - start_time
                    progress = min(0.95, (elapsed / 60.0))  # Smooth curve over ~60 seconds
                    if progress > _progress_state["progress"]:
                        _progress_state["progress"] = progress
                        _progress_state["time_remaining"] = "Generating report..."
                    time_module.sleep(0.3)
            except Exception as e:
                print(f"Report progress simulation error: {e}")

        progress_thread = threading.Thread(target=simulate_report_progress, daemon=True)
        progress_thread.start()

        mission_info = extract_mission_info(run_folder)
        pdf_filename = build_pdf_filename(run_folder)

        # Save to user Downloads folder only — no copy in run folder
        downloads_dir = os.path.join(os.path.expanduser("~"), "Downloads")
        os.makedirs(downloads_dir, exist_ok=True)
        output_path = os.path.join(downloads_dir, pdf_filename)

        ok = generate_report(
            run_folder=run_folder,
            output_path=output_path,
            mission_info=mission_info
        )

        # Mark report complete
        report_complete[0] = True
        _progress_state["is_processing"] = False
        _progress_state["progress"] = 1.0
        _progress_state["time_remaining"] = "Complete!"
        time_module.sleep(0.5)

        if ok:
            _last_report_path = output_path
            return jsonify({
                "status": "success",
                "message": "Report generated successfully.",
                "filename": pdf_filename
            })
        else:
            return jsonify({"status": "error", "message": "No matching .dat files found in the selected folder."})

    except Exception as e:
        import traceback
        _progress_state["is_processing"] = False
        traceback.print_exc()
        return jsonify({"status": "error", "message": str(e)})


@app.route("/download-report", methods=["GET"])
def download_report():
    global _last_report_path
    if not _last_report_path or not os.path.isfile(_last_report_path):
        return jsonify({"status": "error", "message": "No report available."})
    return send_file(
        _last_report_path,
        as_attachment=True,
        download_name=os.path.basename(_last_report_path),
        mimetype="application/pdf"
    )

if __name__ == "__main__":
    app.run(debug=True)