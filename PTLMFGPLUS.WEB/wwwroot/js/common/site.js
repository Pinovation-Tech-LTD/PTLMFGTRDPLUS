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