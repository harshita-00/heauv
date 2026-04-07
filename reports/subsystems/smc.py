"""
smc.py
------
Report module for the SMC subsystem.

Parameters as specified:
  Motor vs MPIDS 150V Plot  → SMC col 66  (Motor file col 2 handled separately
                               in motor.py — cross-file overlay done here)
"""

import os
import numpy as np
from reports.utils.data_extractor import (
    load_dat_file, resolve_time, find_dat_file
)


SUBSYSTEM_NAME = "SMC"
DAT_KEY        = "smc"


# ── Public API ─────────────────────────────────────────────────────────────────

def get_plot_data(run_folder: str):
    """
    Returns list of plot-data dicts.

    Motor vs MPIDS 150V:
      - SMC col 66      → MPIDS 150V signal
      - BLDC/Motor col 2 → Motor Input DC Voltage  (loaded from motor .dat)
      Both plotted as dual-line on the same page.
    """
    smc_path = find_dat_file(run_folder, DAT_KEY)
    if smc_path is None:
        return []

    smc_data = load_dat_file(smc_path)
    if smc_data is None:
        return []

    total_cols = smc_data.shape[1]
    results    = []

    # ── Motor vs MPIDS 150V ────────────────────────────────────────────────
    smc_col = 66
    if smc_col < total_cols:
        mpids_signal = smc_data[:, smc_col]
        time_smc     = resolve_time(smc_data, mpids_signal)

        # Try to load motor DC voltage (col 2) from the BLDC .dat file
        motor_path = find_dat_file(run_folder, "bldc")
        motor_signal = None

        if motor_path is not None:
            motor_data = load_dat_file(motor_path)
            if motor_data is not None and motor_data.shape[1] > 2:
                motor_raw    = motor_data[:, 2]
                motor_signal = motor_raw
                # Align lengths — use the shorter array
                min_len = min(len(mpids_signal), len(motor_signal))
                mpids_signal = mpids_signal[:min_len]
                motor_signal = motor_signal[:min_len]
                time_smc     = time_smc[:min_len]

        if motor_signal is not None:
            results.append({
                "label":     "Motor vs MPIDS 150V",
                "units":     "V",
                "plot_type": "dual",
                "time":      time_smc,
                "signal_a":  mpids_signal,
                "signal_b":  motor_signal,
                "label_a":   "MPIDS 150V (SMC col 66)",
                "label_b":   "Motor Input DC Voltage (col 2)",
            })
        else:
            # Motor file absent — plot MPIDS alone
            results.append({
                "label":     "MPIDS 150V",
                "units":     "V",
                "plot_type": "line",
                "time":      time_smc,
                "signal":    mpids_signal,
            })

    return results