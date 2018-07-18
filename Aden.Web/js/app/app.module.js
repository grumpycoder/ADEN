// app.module.js
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

    //$.ajaxSetup({
    //    error: function (x, status, error) {
    //        if (x.status === 401) {
    //            window.showBSModal({
    //                title: "Session Expired",
    //                body: "Sorry, your session has expired. Please login again to continue.",
    //                size: "small",
    //                actions: [{
    //                    label: 'Ok',
    //                    cssClass: 'btn-default',
    //                    onClick: function (e) {
    //                        //proceed to log out
    //                        $(e.target).parents('.modal').modal('hide');
    //                        window.location.href = "/";
    //                    }
    //                }]
    //            });
    //        }
    //    }
    //});

});
