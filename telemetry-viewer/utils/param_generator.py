from config.telemetry_config import TELEMETRY_PARAMETERS


def get_params(subsystem):

    name = subsystem.split("_")[1].upper()

    if name in TELEMETRY_PARAMETERS:
        return TELEMETRY_PARAMETERS[name]

    return []