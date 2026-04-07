let plotFiles = [];

// ----------------------------------------
// PROGRESS BAR CLASS (reusable)
// ----------------------------------------

class ProgressBar {
    constructor(config) {
        this.config = {
            componentId: config.componentId,
            fillElementId: config.fillElementId,
            percentElementId: config.percentElementId,
            timeElementId: config.timeElementId,
            textElementId: config.textElementId,
            pollUrl: config.pollUrl || '/get-progress',
            pollInterval: config.pollInterval || 500,
        };
        
        this.progressInterval = null;
        this.isActive = false;
    }

    show() {
        const component = document.getElementById(this.config.componentId);
        if (component) component.classList.add('active');
        this.isActive = true;
    }

    hide() {
        const component = document.getElementById(this.config.componentId);
        if (component) component.classList.remove('active');
        this.isActive = false;
        if (this.progressInterval) clearInterval(this.progressInterval);
    }

    startPolling() {
        this.show();
        this.progressInterval = setInterval(() => this.poll(), this.config.pollInterval);
    }

    stopPolling() {
        if (this.progressInterval) {
            clearInterval(this.progressInterval);
            this.progressInterval = null;
        }
    }

    async poll() {
        try {
            const response = await fetch(this.config.pollUrl);
            const data = await response.json();

            if (data.progress !== undefined) {
                this.update(data.progress, data.time_remaining);
            }
        } catch (error) {
            console.log('Progress poll error:', error.message);
        }
    }

    update(progress, timeRemaining) {
        const percent = Math.round(progress * 100);
        const fillEl = document.getElementById(this.config.fillElementId);
        const percentEl = document.getElementById(this.config.percentElementId);
        const textEl = document.getElementById(this.config.textElementId);
        const timeEl = document.getElementById(this.config.timeElementId);

        if (fillEl) fillEl.style.width = percent + '%';
        if (percentEl) percentEl.textContent = percent + '%';
        if (textEl) textEl.textContent = percent + '%';
        if (timeEl) timeEl.textContent = timeRemaining || '--';
    }

    setComplete() {
        this.update(1.0, 'Complete!');
        this.stopPolling();
    }

    reset() {
        this.update(0.0, '--');
        this.stopPolling();
        this.hide();
    }
}

// ----------------------------------------
// INITIALIZE PROGRESS BARS
// ----------------------------------------

const plotProgressBar = new ProgressBar({
    componentId: 'plotProgressBar',
    fillElementId: 'plotProgressFill',
    percentElementId: 'plotProgressPercent',
    timeElementId: 'plotTimeRemaining',
    textElementId: 'plotProgressText',
    pollUrl: '/get-progress',
    pollInterval: 500
});

// Initialize extraction progress bar (reusing same class)
const extractProgressBar = new ProgressBar({
    componentId: 'extractProgressBar',
    fillElementId: 'extractProgressFill',
    percentElementId: 'extractProgressPercent',
    timeElementId: 'extractTimeRemaining',
    textElementId: 'extractProgressText',
    pollUrl: '/get-progress',
    pollInterval: 500
});

// *** ADDED: Initialize report progress bar (same pattern as above) ***
const reportProgressBar = new ProgressBar({
    componentId: 'reportProgressBar',
    fillElementId: 'reportProgressFill',
    percentElementId: 'reportProgressPercent',
    timeElementId: 'reportTimeRemaining',
    textElementId: 'reportProgressText',
    pollUrl: '/get-progress',
    pollInterval: 500
});

// ----------------------------------------
// NAVIGATION WITH NAVBAR ACTIVE STATE
// ----------------------------------------

function showSection(sectionId) {

    const sections = ['home', 'extract', 'plots', 'report'];

    sections.forEach(id => {

        const el = document.getElementById(id);

        if (id === 'home') {
            el.style.display = 'none';
        } else {
            el.classList.add('hidden');
        }

    });

    const target = document.getElementById(sectionId);

    if (sectionId === 'home') {
        target.style.display = 'block';
    } else {
        target.classList.remove('hidden');
    }

    // Update navbar active state
    document.querySelectorAll('.nav-link').forEach(link => {
        link.classList.remove('active');
    });
    
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => {
        if (link.getAttribute('onclick').includes(sectionId)) {
            link.classList.add('active');
        }
    });
}

// Initialize home as active on page load
document.addEventListener('DOMContentLoaded', function() {
    const homeLink = document.querySelector('[onclick="showSection(\'home\')"]');
    if (homeLink) {
        homeLink.classList.add('active');
    }
});


// ----------------------------------------
// EXTRACT  (with progress bar)
// ----------------------------------------

function pickExtractFolder() {
    document.getElementById("fileNameDisplay").value = "Opening folder picker...";

    fetch("/pick-extract-folder")
        .then(r => r.json())
        .then(data => {
            if (data.status === "success") {
                document.getElementById("fileNameDisplay").value = data.message;
            } else if (data.status === "cancelled") {
                document.getElementById("fileNameDisplay").value = "";
            } else {
                document.getElementById("fileNameDisplay").value = "";
                alert("Error: " + data.message);
            }
        })
        .catch(() => {
            document.getElementById("fileNameDisplay").value = "";
            alert("Could not open folder picker");
        });
}

function extractFolder() {

    const label = document.getElementById("fileNameDisplay").value;

    if (!label || label === "Opening folder picker...") {
        alert("Please select a folder first!");
        return;
    }

    // Show overlay and extraction progress bar
    document.getElementById("loading-overlay").style.display = "flex";
    document.getElementById("extractProgressBar").style.display = "block";
    document.getElementById("plotProgressBar").style.display = "none";
    
    extractProgressBar.reset();
    extractProgressBar.startPolling();

    fetch("/extract", {
        method: "POST"
    })
    .then(response => response.json())
    .then(data => {
        extractProgressBar.stopPolling();
        
        if (data.status === "success") {
            extractProgressBar.setComplete();
            setTimeout(() => {
                document.getElementById("loading-overlay").style.display = "none";
                document.getElementById("extractProgressBar").style.display = "none";
                alert(data.message);
            }, 1500);
        } else {
            document.getElementById("loading-overlay").style.display = "none";
            document.getElementById("extractProgressBar").style.display = "none";
            extractProgressBar.reset();
            alert(data.message);
        }
    })
    .catch(error => {
        document.getElementById("loading-overlay").style.display = "none";
        document.getElementById("extractProgressBar").style.display = "none";
        extractProgressBar.reset();
        console.error(error);
        alert("Extraction failed");
    });

}


function handleFolder(input, displayId) {

    if (input.files.length > 0) {

        const firstFile = input.files[0];

        const folderName = firstFile.webkitRelativePath.split('/')[0];

        document.getElementById(displayId).value =
            folderName + " (Folder Selected)";
    }

}


// ----------------------------------------
// PLOTS  (with progress bar integrated)
// ----------------------------------------

function uploadFolder() {

    document.getElementById("plotFileName").value = "Opening folder picker...";

    fetch("/pick-plot-folder")
        .then(r => r.json())
        .then(data => {

            if (data.status === "success") {
                document.getElementById("plotFileName").value = data.message;
            } else if (data.status === "cancelled") {
                document.getElementById("plotFileName").value = "";
            } else {
                document.getElementById("plotFileName").value = "";
                alert("Error: " + data.message);
            }

        })
        .catch(() => {
            document.getElementById("plotFileName").value = "";
            alert("Could not open folder picker");
        });
}


function triggerPlotting() {

    const label = document.getElementById("plotFileName").value;

    if (!label || label === "Opening folder picker...") {
        alert("Please select a run folder first!");
        return;
    }

    // Show overlay and plotting progress bar
    document.getElementById("loading-overlay").style.display = "flex";
    document.getElementById("plotProgressBar").style.display = "block";
    document.getElementById("extractProgressBar").style.display = "none";
    
    // Reset and show progress bar
    plotProgressBar.reset();
    plotProgressBar.startPolling();

    fetch("/plot-all", { method: "POST" })
        .then(response => response.json())
        .then(data => {
            plotProgressBar.stopPolling();
            
            if (data.status === "success") {
                plotProgressBar.setComplete();
                setTimeout(() => {
                    document.getElementById("loading-overlay").style.display = "none";
                    document.getElementById("plotProgressBar").style.display = "none";
                    alert(data.message);
                }, 1500);
            } else {
                document.getElementById("loading-overlay").style.display = "none";
                document.getElementById("plotProgressBar").style.display = "none";
                plotProgressBar.reset();
                alert(data.message);
            }
        })
        .catch(error => {
            document.getElementById("loading-overlay").style.display = "none";
            document.getElementById("plotProgressBar").style.display = "none";
            plotProgressBar.reset();
            console.error(error);
            alert("Plotting failed — check the terminal for details");
        });
}


// ----------------------------------------
// INTERACTIVE VIEWER  (unchanged)
// ----------------------------------------

function openInteractive() {

    fetch("/open-interactive")
    .then(response => response.json())
    .then(data => {
        alert(data.message);
    });

}

// ----------------------------------------
// REPORT
// ----------------------------------------

function pickReportFolder() {
    document.getElementById("reportFileName").value = "Opening folder picker...";

    fetch("/pick-report-folder")
        .then(r => r.json())
        .then(data => {
            if (data.status === "success") {
                document.getElementById("reportFileName").value = data.message;
            } else if (data.status === "cancelled") {
                document.getElementById("reportFileName").value = "";
            } else {
                document.getElementById("reportFileName").value = "";
                alert("Error: " + data.message);
            }
        })
        .catch(() => {
            document.getElementById("reportFileName").value = "";
            alert("Could not open folder picker");
        });
}

function triggerReport() {
    const label = document.getElementById("reportFileName").value;

    if (!label || label === "Opening folder picker...") {
        alert("Please select a run folder first!");
        return;
    }

    // Show overlay and report progress bar
    document.getElementById("loading-overlay").style.display = "flex";
    document.getElementById("reportProgressBar").style.display = "block";
    document.getElementById("extractProgressBar").style.display = "none";
    document.getElementById("plotProgressBar").style.display = "none";

    // *** ADDED: Reset and start polling, same as extract and plot ***
    reportProgressBar.reset();
    reportProgressBar.startPolling();

    fetch("/generate-report", { method: "POST" })
        .then(response => response.json())
        .then(data => {
            // *** ADDED: Stop polling and set complete, same as extract and plot ***
            reportProgressBar.stopPolling();

            if (data.status === "success") {
                reportProgressBar.setComplete();
                setTimeout(() => {
                    document.getElementById("loading-overlay").style.display = "none";
                    document.getElementById("reportProgressBar").style.display = "none";
                    // Show elegant popup
                    document.getElementById("reportPopup").style.display = "flex";
                }, 1500);
            } else {
                document.getElementById("loading-overlay").style.display = "none";
                document.getElementById("reportProgressBar").style.display = "none";
                reportProgressBar.reset();
                alert(data.message);
            }
        })
        .catch(error => {
            document.getElementById("loading-overlay").style.display = "none";
            document.getElementById("reportProgressBar").style.display = "none";
            reportProgressBar.reset();
            console.error(error);
            alert("Report generation failed — check the terminal for details");
        });
}