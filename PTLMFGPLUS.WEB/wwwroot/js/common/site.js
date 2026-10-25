let loaderCount = 0;
window.showLoader = function () {
    loaderCount++;
    $.busyLoadFull("show", {
        spinner: "accordion",
        text: "Please wait...",
        background: "rgba(0,0,0,0.6)"
    });
}

window.hideLoader = function () {
    loaderCount--;
    if (loaderCount <= 0) {
        loaderCount = 0;
        $.busyLoadFull("hide");
    }
}


function setTheme(theme) {
    document.documentElement.style.transition = "all 0.35s ease";
    const themeButton = document.getElementById("themeToggle") || document.getElementById("light-dark-toggle");

    if (themeButton) {
        if (theme == "dark") {
            themeButton.innerHTML = `<i class="bi bi-brightness-high-fill"></i>`;
        }
        else {
            themeButton.innerHTML = `<i class="bi bi-moon-fill"></i>`;
        }
    }
    document.documentElement.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);

    // remove inline transition after animation (prevents lag on scroll/layout)
    setTimeout(() => {
        document.documentElement.style.transition = "";
    }, 400);
}

function toggleTheme() {
    const current = localStorage.getItem("theme") || "light";
    setTheme(current === "dark" ? "light" : "dark");
}



function ShowFooterWithButtons() {
    const divFooter = document.querySelector("#div-footer");

    if (!divFooter) {
        return;
    }

    divFooter.style.display = "flex";
    divFooter.setAttribute("aria-hidden", "false");
    document.body.classList.add("has-action-footer");
}
function HideFooterWithButtons() {
    const divFooter = document.querySelector("#div-footer");

    if (!divFooter) {
        return;
    }

    divFooter.style.display = "none";
    divFooter.setAttribute("aria-hidden", "true");
    document.body.classList.remove("has-action-footer");
}

function HideRefresh() {
    const btn = document.querySelector("#btnFooterRefresh");
    btn.style.display = "none";
}

function HideRecalculate() {
    const btn = document.querySelector("#btnFooterRecalculate");
    btn.style.display = "none";
}

function HideSave() {
    const btn = document.querySelector("#btnFooterSave");
    btn.style.display = "none";
}

function HideCancel() {
    const btn = document.querySelector("#btnFooterCancel");
    btn.style.display = "none";
}
function HideApprove() {
    const btn = document.querySelector("#btnFooterApprove");
    btn.style.display = "none";
}

function ShowingDelete() {
    const btn = document.querySelector("#btnFooterDelete");
    btn.style.display = "inline-block";
}

function ShowApprove() {
    const btn = document.querySelector("#btnFooterApprove");
    btn.style.display = "inline-block";
}




function ShowSaveNew() {
    const btn = document.querySelector("#btnFooterSaveNew");
    btn.style.display = "inline";
}
function formatDate(dateStr) {
    if (window.EnterpriseUI?.dates) {
        return window.EnterpriseUI.dates.format(dateStr);
    }

    const d = new Date(dateStr);

    if (Number.isNaN(d.getTime())) {
        return "";
    }

    const day = String(d.getDate()).padStart(2, '0');
    const month = d.toLocaleString('en-US', { month: 'short' });
    const year = d.getFullYear();

    return `${day}-${month}-${year}`;
}

// Shared, reference-counted loader. This override keeps legacy showLoader/hideLoader
// calls compatible while preventing concurrent or nested requests from hiding early.
window.AppLoader = (function () {
    "use strict";

    let requestCount = 0;
    const defaults = {
        spinner: "accordion",
        text: "Please wait...",
        background: "rgba(0,0,0,0.6)"
    };

    function show(options) {
        requestCount += 1;

        if (requestCount > 1 || !window.jQuery?.busyLoadFull) {
            return;
        }

        $.busyLoadFull("show", $.extend({}, defaults, options || {}));
    }

    function hide() {
        requestCount = Math.max(0, requestCount - 1);

        if (requestCount === 0 && window.jQuery?.busyLoadFull) {
            $.busyLoadFull("hide");
        }
    }

    function reset() {
        requestCount = 0;

        if (window.jQuery?.busyLoadFull) {
            $.busyLoadFull("hide");
        }
    }

    async function run(action, options) {
        if (typeof action !== "function") {
            throw new TypeError("AppLoader.run requires an action function.");
        }

        show(options);

        try {
            return await action();
        }
        finally {
            hide();
        }
    }

    return {
        show: show,
        hide: hide,
        reset: reset,
        run: run,
        getCount: function () {
            return requestCount;
        }
    };
})();

window.showLoader = function (options) {
    window.AppLoader.show(options);
};

window.hideLoader = function () {
    window.AppLoader.hide();
};
