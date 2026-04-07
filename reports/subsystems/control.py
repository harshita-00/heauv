
import numpy as np
from reports.utils.data_extractor import load_dat_file, resolve_time, find_dat_file


SUBSYSTEM_NAME = "CONTROL"
DAT_KEY        = "control"


SINGLE_PARAMETERS = [
    {"label": "Altitude",         "column": 12, "units": "m"},
    {"label": "Shallow Depth",    "column": 2,  "units": "m"},
    {"label": "Heading Rate",     "column": 9,  "units": "deg/s"},
    {"label": "Heading",          "column": 6,  "units": "deg"},
    {"label": "Pitch",            "column": 5,  "units": "deg"},
    {"label": "Pitch Rate",       "column": 8,  "units": "deg/s"},
    {"label": "Pitch Command",    "column": 32, "units": "deg"},
    {"label": "INS Res Velocity", "column": 45, "units": "m/s"},
    {"label": "Roll Command",     "column": 31, "units": "deg"},
    {"label": "Roll Rate",        "column": 7,  "units": "deg/s"},
    {"label": "Roll",             "column": 4,  "units": "deg"},
    {"label": "Yaw Command",      "column": 33, "units": "deg"},
]


def get_plot_data(run_folder: str):

    path = find_dat_file(run_folder, DAT_KEY)
    if path is None:
        return []

    data = load_dat_file(path)
    if data is None:
        return []

    total_cols = data.shape[1]
    results    = []

    # ── Standard plots ─────────────────────────────────────────
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

    # ── Lat vs Long ───────────────────────────────────────────
    if 10 < total_cols and 11 < total_cols:
        results.append({
            "label":     "Latitude vs Longitude",
            "units":     "deg",
            "plot_type": "scatter",
            "x":         data[:, 11],
            "y":         data[:, 10],
            "x_label":   "Longitude (deg)",
            "y_label":   "Latitude (deg)",
        })

    # ── XY Trajectory ─────────────────────────────────────────
    if all(c < total_cols for c in [28, 29, 30, 6]):
        NVel = data[:, 28]
        EVel = data[:, 29]
        DVel = data[:, 30]
        Heading = data[:, 6]

        X = np.zeros(len(NVel))
        Y = np.zeros(len(NVel))

        for i in range(1, len(NVel)):
            v = np.sqrt(NVel[i]**2 + EVel[i]**2 + DVel[i]**2)
            X[i] = X[i-1] + 0.1 * v * np.cos(np.radians(Heading[i]))
            Y[i] = Y[i-1] + 0.1 * v * np.sin(np.radians(Heading[i]))

        results.append({
            "label":     "XY Trajectory",
            "units":     "m",
            "plot_type": "scatter",
            "x":         Y,
            "y":         X,
            "x_label":   "Y  —  East (m)",
            "y_label":   "X  —  North (m)",
        })

    # ── CUSTOM MULTI-PLOTS ─────────────────────────────────────

    def safe(idx):
        return data[:, idx] if idx < total_cols else None

    pitch_cmd = safe(32)
    pitch     = safe(5)
    pitch_rt  = safe(8)

    roll_cmd  = safe(31)
    roll      = safe(4)
    roll_rt   = safe(7)

    depth_raw = safe(2)

    base = pitch if pitch is not None else data[:, 0]
    time = resolve_time(data, base)

    # SPEED
    if all(c < total_cols for c in [28, 29, 30]):
        nvel = data[:, 28]
        evel = data[:, 29]
        dvel = data[:, 30]
        speed = np.sqrt(nvel**2 + evel**2 + dvel**2)
        speed = np.where((speed >= 0) & (speed < 20), speed, np.nan)
    else:
        speed = None

    # DEPTH
    depth = np.where(depth_raw > 0, depth_raw, np.nan) if depth_raw is not None else None

    # Plot 1
    if all(x is not None for x in [pitch_cmd, pitch, pitch_rt, depth]):
        results.append({
            "label": "Pitch Cmd , Pitch , Pitch Rate , Depth",
            "units": "",
            "plot_type": "dual",
            "time": time,
            "signal_a": pitch_cmd,
            "signal_b": pitch,
            "label_a": "Pitch Cmd",
            "label_b": "Pitch",
            "extra_signals": [pitch_rt, depth],
            "extra_labels": ["Pitch Rate", "Depth"],
        })

    # Plot 2
    if all(x is not None for x in [speed, pitch, pitch_rt]):
        results.append({
            "label": "Speed , Pitch , Pitch Rate",
            "units": "",
            "plot_type": "dual",
            "time": time,
            "signal_a": speed,
            "signal_b": pitch,
            "label_a": "Speed",
            "label_b": "Pitch",
            "extra_signals": [pitch_rt],
            "extra_labels": ["Pitch Rate"],
        })

    # Plot 3
    if all(x is not None for x in [roll_cmd, roll, roll_rt]):
        results.append({
            "label": "Roll Cmd , Roll , Roll Rate",
            "units": "",
            "plot_type": "dual",
            "time": time,
            "signal_a": roll_cmd,
            "signal_b": roll,
            "label_a": "Roll Cmd",
            "label_b": "Roll",
            "extra_signals": [roll_rt],
            "extra_labels": ["Roll Rate"],
        })

    return results

