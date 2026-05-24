var Notifications = (function () {
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
    return { Sweet, Toastr };
})();
