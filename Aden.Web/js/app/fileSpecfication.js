//fileSpecification.js
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
    });

    $(document).on('click', '[data-retire]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('filespec-id');

        window.$toggleWorkingButton(btn);

        $.ajax({
            url: '/api/filespecification/retire/' + id,
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
            url: '/api/filespecification/activate/' + id,
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

    $(document).on('submit', 'form', (function (e) {
        e.preventDefault();
        var form = $('form');
        var url = '/api/filespecification/' + $('#Id').val();


        $.ajax({
            type: "PUT",
            url: url,
            data: $('form').serialize(),
            success: function (msg) {
                $('#editModal').modal('hide');
                $('#editContainer').html("");
                $('#grid').dxDataGrid('instance').refresh().done(function (e) { console.log('done', e) });
                window.toastr.success('Saved ');
            },
            error: function (err) {
                var validationErrors = JSON.parse(err.responseText);
                $.each(validationErrors.modelState, function (i, ival) {
                    window.remoteErrors(form, i, ival);
                });
            }

        });

    }));
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


