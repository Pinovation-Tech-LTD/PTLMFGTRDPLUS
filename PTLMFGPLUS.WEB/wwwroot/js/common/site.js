let loaderCount = 0;
window.showLoader = function () {
    loaderCount++;
    $.busyLoadFull("show", {
        spinner: "accordion",
        text: "Please wait...",
        background: "rgba(255,255,255,0.6)"
    });
}

window.hideLoader = function () {
    loaderCount--;
    if (loaderCount <= 0) {
        loaderCount = 0;
        $.busyLoadFull("hide");
    }
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