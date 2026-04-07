import pyqtgraph as pg
from pyqtgraph.Qt import QtWidgets, QtCore


# distinct colors for multi-line plots
PLOT_COLORS = [
    (0, 200, 255),    # cyan  (default / first)
    (255, 100, 100),  # red
    (100, 255, 140),  # green
    (255, 200, 50),   # yellow
    (180, 100, 255),  # purple
    (255, 150, 50),   # orange
    (50, 200, 180),   # teal
    (255, 80, 180),   # pink
]


class CoordLabel(QtWidgets.QLabel):
    """Floating coordinate display overlay."""

    def __init__(self, parent):
        super().__init__(parent)

        self.setStyleSheet("""
            QLabel {
                color: #000000;
                background-color: rgba(255, 255, 255, 180);
                border: none;
                border-radius: 3px;
                padding: 2px 7px;
                font-family: 'Courier New', monospace;
                font-size: 11px;
            }
        """)

        self.setAttribute(QtCore.Qt.WA_TransparentForMouseEvents)
        self.setAlignment(QtCore.Qt.AlignLeft | QtCore.Qt.AlignVCenter)
        self.hide()


class TelemetryPlot(pg.PlotWidget):

    def __init__(self):

        super().__init__()

        self.setBackground("#101010")

        self.showGrid(x=True, y=True)

        # enable zoom + pan
        self.setMouseEnabled(x=True, y=True)

        self.getViewBox().setMouseMode(pg.ViewBox.RectMode)

        # crosshair
        self.vLine = pg.InfiniteLine(angle=90, movable=False)
        self.hLine = pg.InfiniteLine(angle=0, movable=False)

        self.addItem(self.vLine, ignoreBounds=True)
        self.addItem(self.hLine, ignoreBounds=True)

        # coordinate overlay label
        self._coord_label = CoordLabel(self)
        self._coord_label.raise_()

        self.proxy = pg.SignalProxy(
            self.scene().sigMouseMoved,
            rateLimit=60,
            slot=self.mouseMoved
        )

    # --------------------------

    def mouseMoved(self, evt):

        pos = evt[0]

        if self.sceneBoundingRect().contains(pos):

            mousePoint = self.getViewBox().mapSceneToView(pos)

            self.vLine.setPos(mousePoint.x())
            self.hLine.setPos(mousePoint.y())

            x_val = mousePoint.x()
            y_val = mousePoint.y()

            def fmt(v):
                return f"{v:.2e}" if abs(v) >= 1e4 or (abs(v) < 1e-2 and v != 0) else f"{v:.2f}"

            self._coord_label.setText(f"{fmt(x_val)}, {fmt(y_val)}")

            widget_pos = self.mapFromScene(pos)

            lw = self._coord_label.sizeHint().width() + 16
            lh = self._coord_label.sizeHint().height() + 6

            px = max(4, min(int(widget_pos.x()) + 14, self.width() - lw - 4))
            py = max(4, min(int(widget_pos.y()) - 28, self.height() - lh - 4))

            self._coord_label.move(px, py)
            self._coord_label.adjustSize()
            self._coord_label.show()
            self._coord_label.raise_()

        else:
            self._coord_label.hide()

    # --------------------------

    def draw_plot(self, time, signal, label):
        """Single parameter — original behaviour unchanged."""

        self.clear()

        self.addItem(self.vLine, ignoreBounds=True)
        self.addItem(self.hLine, ignoreBounds=True)

        pen = pg.mkPen(color=PLOT_COLORS[0], width=2)

        self.plot(time, signal, pen=pen)

        self.setLabel("left", label)
        self.setLabel("bottom", "Time")

        self.enableAutoRange()

    # --------------------------

    def draw_multi_plot(self, datasets):
        """
        Plot multiple parameters on the same canvas against time.
        datasets : list of (time, signal, label)
        Each line gets a distinct color and appears in the legend.
        """

        self.clear()

        self.addItem(self.vLine, ignoreBounds=True)
        self.addItem(self.hLine, ignoreBounds=True)

        legend = self.addLegend(offset=(10, 10))
        legend.setBrush(pg.mkBrush(30, 30, 30, 200))
        legend.setPen(pg.mkPen("#444"))

        for i, (time, signal, label) in enumerate(datasets):
            color = PLOT_COLORS[i % len(PLOT_COLORS)]
            pen = pg.mkPen(color=color, width=2)
            self.plot(time, signal, pen=pen, name=label)

        y_label = " vs ".join(label for _, _, label in datasets)
        self.setLabel("left", y_label)
        self.setLabel("bottom", "Time")

        self.enableAutoRange()