var Notifications = (function () {
    function getErrorDetails(error, fallbackMessage) {
        const message = error?.message
            || error?.objError
            || (typeof error === "string" ? error : "")
            || fallbackMessage
            || "The operation could not be completed.";
        const traceId = error?.traceId || error?.payload?.traceId;

        return {
            message: traceId ? `${message} Reference ID: ${traceId}` : message,
            traceId: traceId
        };
    }

    var Sweet = {
        success: function (title, msg, cb) {
            
            Swal.fire({
              title: title||"Success!!", text: msg|| "Update Successful!!!", icon: "success"
            }).then(() => { if (cb) cb(); }) },
        error: function (title, msg, cb) {  
            console.log(title, msg);
            Swal.fire({
               title: title||"Error!!", text: msg|| "Some error occured. Please contact PTL Team!!!", icon: "error"
            }).then(() => { if (cb) cb(); }) },
        warning: function (title, msg, cb) { 
            Swal.fire({
               title: title||"Warning!!", text: msg||"", icon: "warning"
            }).then(() => { if (cb) cb(); }) },
        info: function (title, msg, cb) { 
            Swal.fire({
               title: title||"Information!!", text: msg||"", icon: "info"
            }).then(() => { if (cb) cb(); }) },
       confirmSave: function (text, cb){
            Swal.fire({
                title: 'Are you sure?',
                text: text || "You want to save this data!",
                icon: 'question',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'OK',
                cancelButtonText: 'Cancel'
            }).then((result) => {
                if (result.isConfirmed) {
                    if (cb){
                        cb();
                   }
                }
            });
        }
    };

    var Toastr = {
        success: function (title, msg) { toastr.success(msg, title); },
        error: function (title, msg) { toastr.error(msg, title); },
        warning: function (title, msg) { toastr.warning(msg, title); },
        info: function (title, msg) { toastr.info(msg, title); }
    };
    function handleError(error, fallbackMessage, options) {
        const details = getErrorDetails(error, fallbackMessage);
        console.error("Application error", {
            message: details.message,
            traceId: details.traceId,
            error: error
        });

        if (options?.silent !== true) {
            Toastr.error("Error", details.message);
        }

        return details;
    }

    return {
        Sweet,
        Toastr,
        handleError,
        getErrorDetails
    };
})();

window.addEventListener("unhandledrejection", function (event) {
    Notifications.handleError(event.reason, "An unexpected error occurred.");
});

window.addEventListener("error", function (event) {
    Notifications.handleError(event.error || event.message, "An unexpected page error occurred.");
});
