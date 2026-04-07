import sys
from PyQt5.QtWidgets import QApplication
from gui.main_window import MainWindow
from gui.style import APP_STYLE


def main():

    app = QApplication(sys.argv)

    app.setStyleSheet(APP_STYLE)

    run_folder = None

    if len(sys.argv) > 1:
        run_folder = sys.argv[1]

    window = MainWindow(run_folder)

    window.show()

    sys.exit(app.exec_())


if __name__ == "__main__":
    main()