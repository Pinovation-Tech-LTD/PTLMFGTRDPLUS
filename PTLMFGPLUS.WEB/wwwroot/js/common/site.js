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

    if (theme == "dark") {
        document.getElementById("light-dark-toggle").innerHTML = `<i class="bi bi-brightness-high-fill"></i>`;
    }
    else {
        document.getElementById("light-dark-toggle").innerHTML = `<i class="bi bi-moon-fill"></i>`;
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
    divFooter.style.display = "block";
}
function HideFooterWithButtons() {
    const divFooter = document.querySelector("#div-footer");
    divFooter.style.display = "block";
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
    const d = new Date(dateStr);
    const day = String(d.getDate()).padStart(2, '0');
    const month = d.toLocaleString('en-US', { month: 'short' });
    const year = d.getFullYear();

    return `${day}-${month}-${year}`;
}