"""
eassc.py
--------
Report module for the EASSC subsystem.

Parameters — Command & Feedback pairs (subplots on same page):
  Stbd Bot Pos Cmd & Fdbk  → col 1  (cmd)  & col 11 (fdbk)
  Port Bot Pos Cmd & Fdbk  → col 2  (cmd)  & col 15 (fdbk)
  Stbd Top Pos Cmd & Fdbk  → col 3  (cmd)  & col 19 (fdbk)
  Port Top Pos Cmd & Fdbk  → col 4  (cmd)  & col 23 (fdbk)

Computation applied to BOTH cmd and fdbk values:
  value_scaled = raw_value * (40 / 65535) - 20
"""

import numpy as np
from reports.utils.data_extractor import load_dat_file, resolve_time, find_dat_file


SUBSYSTEM_NAME = "EASSC"
DAT_KEY        = "eassc"


# ── Command / Feedback pairs ──────────────────────────────────────────────────
# Each entry: label, cmd_col, fdbk_col
# Scaling applied: raw * (40/65535) - 20
CMD_FDBK_PAIRS = [
    {"label": "Stbd Bot Pos", "cmd_col": 1,  "fdbk_col": 11},
    {"label": "Port Bot Pos", "cmd_col": 2,  "fdbk_col": 15},
    {"label": "Stbd Top Pos", "cmd_col": 3,  "fdbk_col": 19},
    {"label": "Port Top Pos", "cmd_col": 4,  "fdbk_col": 23},
]

SCALE_FACTOR = 40.0 / 65535.0
SCALE_OFFSET = -20.0


def _scale(raw: np.ndarray) -> np.ndarray:
    """Apply EASSC scaling: raw * (40/65535) - 20"""
    return raw * SCALE_FACTOR + SCALE_OFFSET


# ── Public API ─────────────────────────────────────────────────────────────────

def get_plot_data(run_folder: str):
    """
    Returns a list of dual-line plot dicts (cmd vs fdbk on same axes).
    Each dict: { label, units, plot_type="dual", time, signal_a, signal_b,
                 label_a, label_b }
    """
    path = find_dat_file(run_folder, DAT_KEY)
    if path is None:
        return []

    data = load_dat_file(path)
    if data is None:
        return []

    total_cols = data.shape[1]
    results    = []

    for p in CMD_FDBK_PAIRS:
        cc, fc = p["cmd_col"], p["fdbk_col"]
        if cc >= total_cols or fc >= total_cols:
            continue

        cmd_raw  = data[:, cc]
        fdbk_raw = data[:, fc]

        cmd_scaled  = _scale(cmd_raw)
        fdbk_scaled = _scale(fdbk_raw)

        time = resolve_time(data, cmd_scaled)

        results.append({
            "label":     p["label"] + " — Cmd vs Fdbk",
            "units":     "deg",
            "plot_type": "dual",
            "time":      time,
            "signal_a":  cmd_scaled,
            "signal_b":  fdbk_scaled,
            "label_a":   p["label"] + " Cmd",
            "label_b":   p["label"] + " Fdbk",
        })

    return results