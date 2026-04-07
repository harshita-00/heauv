"""
motor.py
--------
Report module for the MOTOR / BLDC subsystem.

Parameters and column indices as specified:
  Motor Input DC Voltage & Current  → col 2 (voltage) & col 3  (current)
  Motor Speed                       → col 8
  Motor Voltage & Current           → col 4 (voltage) & col 6  (current)
"""

from reports.utils.data_extractor import load_dat_file, resolve_time, find_dat_file


SUBSYSTEM_NAME = "MOTOR (BLDC)"
DAT_KEY        = "bldc"


# ── Dual-signal parameters (two signals on same axes) ─────────────────────────
DUAL_PARAMETERS = [
    {
        "label":   "Motor Input DC Voltage & Current",
        "col_a":   2,  "label_a": "Input DC Voltage", "units_a": "V",
        "col_b":   3,  "label_b": "Input DC Current",  "units_b": "A",
        "units":   "V / A",
    },
    {
        "label":   "Motor Voltage & Current",
        "col_a":   4,  "label_a": "Motor Voltage", "units_a": "V",
        "col_b":   6,  "label_b": "Motor Current", "units_b": "A",
        "units":   "V / A",
    },
]

# ── Single-signal parameters ───────────────────────────────────────────────────
SINGLE_PARAMETERS = [
    {"label": "Motor Speed", "column": 8, "units": "RPM"},
]


# ── Public API ─────────────────────────────────────────────────────────────────

def get_plot_data(run_folder: str):
    """
    Returns list of plot-data dicts.
    Dual plots use plot_type="dual"; single plots use plot_type="line".
    """
    path = find_dat_file(run_folder, DAT_KEY)
    if path is None:
        return []

    data = load_dat_file(path)
    if data is None:
        return []

    total_cols = data.shape[1]
    results    = []

    # ── 1. Dual-signal plots ───────────────────────────────────────────────
    for p in DUAL_PARAMETERS:
        ca, cb = p["col_a"], p["col_b"]
        if ca >= total_cols or cb >= total_cols:
            continue
        sig_a = data[:, ca]
        sig_b = data[:, cb]
        time  = resolve_time(data, sig_a)
        results.append({
            "label":     p["label"],
            "units":     p["units"],
            "plot_type": "dual",
            "time":      time,
            "signal_a":  sig_a,
            "signal_b":  sig_b,
            "label_a":   p["label_a"],
            "label_b":   p["label_b"],
        })

    # ── 2. Single-signal plots ─────────────────────────────────────────────
    for p in SINGLE_PARAMETERS:
        col = p["column"]
        if col >= total_cols:
            continue
        signal = data[:, col]
        time   = resolve_time(data, signal)
        results.append({
            "label":     p["label"],
            "units":     p["units"],
            "plot_type": "line",
            "time":      time,
            "signal":    signal,
        })

    return results