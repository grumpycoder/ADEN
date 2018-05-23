// Global Functions
window.onbeforeunload = function () {
    console.log('before unload');
    $('.container-fluid').hide();
    $('.loading').show();

    //$.LoadingOverlay("show", { image: '', fontawesome: 'fa fa-cog fa-spin' });
};

window.onBeforeSend = function () {
}

window.onloadstart = function () {
}


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
    $('.container-fluid').show();
    $('.loading').hide();
});

