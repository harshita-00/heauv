import numpy as np


def load_dat_file(path):

    rows = []

    with open(path, "r") as f:

        for line in f:

            parts = line.strip().split()

            if not parts:
                continue

            try:
                row = [float(x) for x in parts]
                rows.append(row)
            except:
                continue

    if len(rows) == 0:
        return None

    return np.array(rows)