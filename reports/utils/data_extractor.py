"""
data_extractor.py
-----------------
Loads .dat files for the report engine.
Uses the exact same logic as core/loader.py — no duplication of the
underlying parser, just a thin wrapper so reports stay self-contained.
"""

import os
import sys
import numpy as np

ROOT = os.path.abspath(os.path.join(os.path.dirname(__file__), "..", ".."))
if ROOT not in sys.path:
    sys.path.insert(0, ROOT)

# ── Column index used as time across all subsystems ──────────────────────────
TIME_COLUMN = -2          # second-last column  (same as telemetry_config.py)


def load_dat_file(path: str):
    """
    Parse a whitespace-delimited .dat file into a 2-D numpy array.
    Returns None if the file is empty or unreadable.
    Identical logic to core/loader.py so existing behaviour is preserved.
    """
    rows = []

    with open(path, "r") as f:
        for line in f:
            parts = line.strip().split()
            if not parts:
                continue
            try:
                rows.append([float(x) for x in parts])
            except ValueError:
                continue          # skip header / label rows

    if not rows:
        return None

    return np.array(rows)


def resolve_time(data: np.ndarray, signal: np.ndarray):
    """
    Return the time axis for a given signal array.
    Mirrors _resolve_time() in main_window.py exactly:
      - try second-last column first
      - fall back to sequential indices if the range is zero
    """
    try:
        candidate = data[:, TIME_COLUMN]
        if candidate.max() - candidate.min() > 0:
            return candidate
        raise ValueError("flat time column")
    except Exception:
        return list(range(len(signal)))


def find_dat_file(run_folder: str, subsystem_key: str) -> str | None:
    """
    Locate the .dat file for a subsystem inside a run folder.

    Handles filenames with or without a leading underscore:
        RUN-0_Control_9-3-2026_20-20-32.dat      ← no leading underscore
        _RUN-1_Control_9-3-2026_20-20-33.dat     ← leading underscore stripped

    Matching is case-insensitive on the subsystem part (parts[1]).
    Returns the full path, or None if not found.
    """
    key = subsystem_key.lower()
    for fname in os.listdir(run_folder):
        if not fname.endswith(".dat"):
            continue
        # Strip any leading underscores before splitting so
        # _RUN-1_Control_... and RUN-0_Control_... both match correctly
        clean  = fname.lstrip("_")
        parts  = clean.split("_")
        if len(parts) > 1 and parts[1].lower() == key:
            return os.path.join(run_folder, fname)
    return None