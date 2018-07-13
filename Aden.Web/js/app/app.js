// App Global Functions

$(document).on('click', '[role="navigation"]', function (e) {

});

window.$log = window.toastr;

window.$toggleWorkingButton = function (button, status) {
    if (status === 'off') {
        button.LoadingOverlay('hide');
    } else {
        button.LoadingOverlay('show',
            { image: '', fontawesome: 'fa fa-cog fa-spin' });
    }
}

window.$showModalWorking = function () {
    $('.modal-content').LoadingOverlay('show',
        { image: '', fontawesome: 'fa fa-cog fa-spin' });
}

window.$hideModalWorking = function () {
    $('.modal-content').LoadingOverlay('hide');
}

function editorPrepared(info) {
    if (info.parentType === 'filterRow' && info.editorName === "dxSelectBox") {
        info.trueText = "Yes";
        info.falseText = "No";
    }
}

function gridContentReady() {
    $("table[role='grid']").each(function (idx, elm) {
        $(elm).addClass('table');
    });
}


$(function () {
    console.log('application ready');
    $('body').tooltip({ selector: '[data-toggle=tooltip]' });
    //$('.container-fluid').show();
    //$('.loading').hide();

    $.ajaxSetup({
        error: function (x, status, error) {
            //debugger;
            if (x.status === 401) {
                //alert("Sorry, your session has expired. Please login again to continue");

                window.showBSModal({
                    title: "Session Expired",
                    body: "Sorry, your session has expired. Please login again to continue.",
                    size: "small",
                    actions: [{
                        label: 'Ok',
                        cssClass: 'btn-default',
                        onClick: function (e) {
                            //proceed to log out
                            $(e.target).parents('.modal').modal('hide');
                            window.location.href = "/";
                        }
                    }]
                });

                //window.$log.info('Sorry, your session has expired. Please login again to continue');

                //window.location.href = "/";
            }
            //else {
            //    alert("An error occurred: " + status + "nError: " + error);
            //}
        }
    });

});


//$(function () {
//    //setup ajax error handling
//    $.ajaxSetup({
//        error: function (x, status, error) {
//            if (x.status == 403) {
//                alert("Sorry, your session has expired. Please login again to continue");
//                window.location.href = "/Account/Login";
//            }
//            else {
//                alert("An error occurred: " + status + "nError: " + error);
//            }
//        }
//    });
//});
