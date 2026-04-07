import os

from PyQt5.QtWidgets import (
    QWidget,
    QHBoxLayout,
    QVBoxLayout,
    QListWidget,
    QPushButton,
    QFileDialog,
    QLabel,
    QListWidgetItem,
)
from PyQt5.QtCore import Qt

from core.loader import load_dat_file
from gui.plot_widget import TelemetryPlot
from config.telemetry_config import TELEMETRY_PARAMETERS


class MainWindow(QWidget):

    def __init__(self, run_folder=None):

        super().__init__()

        self.setWindowTitle("Telemetry Viewer")

        self.data = None 
        self.run_folder = None

        self.current_subsystem = None
        self.current_parameter = None 

        self.run_folder = run_folder

        layout = QHBoxLayout()

        left = QVBoxLayout()

        self.load_btn = QPushButton("Load Run Folder")

        self.subsystem_list = QListWidget()
        self.param_list = QListWidget()

        left.addWidget(self.load_btn)
        left.addWidget(QLabel("Subsystems"))
        left.addWidget(self.subsystem_list)
        left.addWidget(QLabel("Parameters"))
        left.addWidget(self.param_list)

        right = QVBoxLayout()

        self.selected_file = QLabel("File : None")
        self.selected_param = QLabel("Parameter : None")

        self.plot = TelemetryPlot()

        right.addWidget(self.selected_file)
        right.addWidget(self.selected_param)
        right.addWidget(self.plot)

        layout.addLayout(left, 1)
        layout.addLayout(right, 3)

        self.setLayout(layout)

        self.load_btn.clicked.connect(self.load_folder)
        self.subsystem_list.itemClicked.connect(self.load_subsystem)

        # checkbox toggle → replot
        self.param_list.itemChanged.connect(self.on_param_changed)

        # auto-populate if folder passed in via command line
        if self.run_folder:
            self._populate_subsystem_list(self.run_folder)

    # --------------------------------------

    def _populate_subsystem_list(self, folder):
        self.subsystem_list.clear()
        for file in os.listdir(folder):
            if file.endswith(".dat"):
                self.subsystem_list.addItem(file)

    # --------------------------------------

    def load_folder(self):

        folder = QFileDialog.getExistingDirectory(self)

        if not folder:
            return

        self.run_folder = folder

        self.subsystem_list.clear()

        for file in os.listdir(folder):
            if file.endswith(".dat"):
                self.subsystem_list.addItem(file)

    # --------------------------------------

    def load_subsystem(self, item):

        file = item.text()

        path = os.path.join(self.run_folder, file)

        self.data = load_dat_file(path)

        if self.data is None:
            return

        subsystem = self.extract_name(file)

        self.current_subsystem = subsystem

        self.selected_file.setText(f"File : {file}")

        params = TELEMETRY_PARAMETERS.get(subsystem, [])

        # block signals while repopulating to avoid spurious itemChanged
        self.param_list.blockSignals(True)
        self.param_list.clear()

        for p in params:
            list_item = QListWidgetItem(p)
            list_item.setFlags(list_item.flags() | Qt.ItemIsUserCheckable)
            list_item.setCheckState(Qt.Unchecked)
            self.param_list.addItem(list_item)

        self.param_list.blockSignals(False)

        self.selected_param.setText("Parameter : None")

    # --------------------------------------

    def extract_name(self, file):

        parts = file.split("_")

        if len(parts) > 1:
            return parts[1].upper()

        return file.replace(".dat", "").upper()

    # --------------------------------------

    def _get_checked_params(self):
        """Return list of (row_index, param_name) for all checked items."""
        checked = []
        for i in range(self.param_list.count()):
            it = self.param_list.item(i)
            if it.checkState() == Qt.Checked:
                checked.append((i, it.text()))
        return checked

    # --------------------------------------

    def _resolve_time(self, signal):
        """Same auto-detect time logic as original."""
        try:
            candidate_time = self.data[:, -2]
            if candidate_time.max() - candidate_time.min() > 0:
                return candidate_time
            else:
                raise Exception()
        except:
            return list(range(len(signal)))

    # --------------------------------------

    def on_param_changed(self, _item):
        """Called whenever any checkbox state changes."""

        if self.data is None:
            return

        checked = self._get_checked_params()
        total_columns = self.data.shape[1]

        if not checked:
            self.selected_param.setText("Parameter : None")
            return

        if len(checked) == 1:
            # ---- single plot — original behaviour ----
            column, param_name = checked[0]

            if column >= total_columns:
                return

            signal = self.data[:, column]
            time = self._resolve_time(signal)

            self.current_parameter = param_name
            self.selected_param.setText(f"Parameter : {param_name}")

            self.plot.draw_plot(time, signal, param_name)

        else:
            # ---- multi-line on single plot ----
            names = ", ".join(name for _, name in checked)
            self.selected_param.setText(f"Parameters : {names}")

            datasets = []

            for column, param_name in checked:
                if column >= total_columns:
                    continue
                signal = self.data[:, column]
                time = self._resolve_time(signal)
                datasets.append((time, signal, param_name))

            self.plot.draw_multi_plot(datasets)

    # --------------------------------------

    # original plot_parameter kept intact — nothing removed
    def plot_parameter(self, item):

        if self.data is None:
            return

        column = self.param_list.row(item)

        total_columns = self.data.shape[1]

        if column >= total_columns:
            return

        signal = self.data[:, column]

        try:
            candidate_time = self.data[:, -2]
            if candidate_time.max() - candidate_time.min() > 0:
                time = candidate_time
            else:
                raise Exception()
        except:
            time = list(range(len(signal)))

        self.current_parameter = item.text()
        self.selected_param.setText(f"Parameter : {self.current_parameter}")

        self.plot.draw_plot(time, signal, item.text())