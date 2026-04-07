"""
dvl.py
------
Report module for the DVL subsystem.

Parameters and column indices as specified:
  Average Beam Range       → col 17
  Bottom Track Velocity    → col 14
  Water Track Velocity     → col 15
"""

from reports.utils.data_extractor import load_dat_file, resolve_time, find_dat_file


SUBSYSTEM_NAME = "DVL"
DAT_KEY        = "dvl"


# ── Parameters to report ──────────────────────────────────────────────────────
PARAMETERS = [
    {"label": "Average Beam Range",    "column": 17, "units": "m"},
    {"label": "Bottom Track Velocity", "column": 14, "units": "m/s"},
    {"label": "Water Track Velocity",  "column": 15, "units": "m/s"},
]


# ── Public API ────────────────────────────────────────────────────────────────

def get_plot_data(run_folder: str):
    """
    Returns list of plot-data dicts.
    Each dict: { label, units, plot_type, time, signal }
    """
    path = find_dat_file(run_folder, DAT_KEY)
    if path is None:
        return []

    data = load_dat_file(path)
    if data is None:
        return []

    results    = []
    total_cols = data.shape[1]

    for p in PARAMETERS:
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