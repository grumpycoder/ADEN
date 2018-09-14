// app.module.js
// App Global Functions

$(document).on('click', '[role="navigation"]', function (e) {

});

window.$log = window.toastr;

window.$toggleWorkingButton = function (button, status) {

    if ($('.loadingoverlay').length) status = 'off'; 

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
});
