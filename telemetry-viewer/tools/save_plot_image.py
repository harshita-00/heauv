import os
import pyqtgraph.exporters


def save_plot(plot_widget, run_folder, subsystem, parameter):

    if run_folder is None:
        return

    if subsystem is None or parameter is None:
        return

    filename = f"{subsystem}_{parameter}.png"

    path = os.path.join(run_folder, filename)

    exporter = pyqtgraph.exporters.ImageExporter(
        plot_widget.plotItem
    )

    exporter.parameters()['width'] = 1920

    exporter.export(path)

    print("Saved plot:", path)