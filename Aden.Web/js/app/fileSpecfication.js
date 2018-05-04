﻿//fileSpecification.js

$(function () {
    console.log('file specification ready');

    var $grid = $('#grid').dxDataGrid('instance');

    $(document).on('click', '[data-edit]', function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        $.get(url,
            function (data) {
                $('#editContainer').html(data);
                $('#editModal').modal({ show: true });
            });

        $(document).on('click', '#saveEditSpecificationForm', function (e) {
            e.preventDefault();
            console.log('save');
        });

    });

    $(document).on('click', '[data-retire]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('filespec-id');

        window.$toggleWorkingButton(btn);

        $.ajax({
            url: '/api/filespecifications/retire/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh();
                window.$log.success('Retired ' + data.fileNumber + ' - ' + data.fileName);
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Something went wrong: ' + err.message);
            }
        }).always(function () {
            window.$toggleWorkingButton(btn);
        });

    });

    $(document).on('click', '[data-activate]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('filespec-id');

        window.$toggleWorkingButton(btn);

        $.ajax({
            url: '/api/filespecifications/activate/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Activated ' + data.fileNumber + ' - ' + data.fileName);
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Something went wrong: ' + err.message);
            }
        }).always(function () {
            window.$toggleWorkingButton(btn);
        });
    });


});

function createFileSpecificationGridActionButtons(container, options) {
    var lnk = '';
    var isRetired = options.data.isRetired;
    var fileNumber = options.data.fileNumber;
    var dataYear = options.data.dataYear;
    var filespecId = options.data.id;

    if (!isRetired) {
        lnk += '<a class="btn btn-default btn-sm btn-grid" href="/reports/' + fileNumber + '/' + dataYear + '" data-retire data-filespec-id=' + filespecId + '><i class="fa fa-spinner fa-spin hidden"></i> Retire</a>&nbsp;';
    }
    if (isRetired) {
        lnk += '<a class="btn btn-default btn-sm btn-grid" href="/reports/' + fileNumber + '/' + dataYear + '" data-activate data-filespec-id=' + filespecId + '><i class="fa fa-spinner fa-spin hidden"></i> Activate</a>&nbsp;';
    }
    lnk += '<a class="btn btn-default btn-sm btn-grid" href="/EditFileSpecification/' + filespecId + '" data-edit data-id="' + filespecId + '">Edit</a>';
    container.append(lnk);
}


function OnSpecificationUpdateFormComplete(data) {
    if (data.responseText === 'success') {
        $('#editModal').modal('hide');
        $('#editContainer').html("");
        $('#grid').dxDataGrid('instance').refresh().done(function (e) { console.log('done', e) });
        window.toastr.success('Saved ');
    }
}
