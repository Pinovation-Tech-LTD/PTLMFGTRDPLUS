var Notifications = (function () {
    var Sweet = {
        success: function (title, msg, cb) { swal({ title, text: msg, icon: "success" }).then(() => { if (cb) cb(); }) },
        error: function (title, msg, cb) { swal({ title, text: msg, icon: "error" }).then(() => { if (cb) cb(); }) },
        warning: function (title, msg, cb) { swal({ title, text: msg, icon: "warning" }).then(() => { if (cb) cb(); }) },
        info: function (title, msg, cb) { swal({ title, text: msg, icon: "info" }).then(() => { if (cb) cb(); }) }
    };

    var Toastr = {
        success: function (title, msg) { toastr.success(msg, title); },
        error: function (title, msg) { toastr.error(msg, title); },
        warning: function (title, msg) { toastr.warning(msg, title); },
        info: function (title, msg) { toastr.info(msg, title); }
    };
    return { Sweet, Toastr };
})();
