﻿function UpdateSuccess(data) {
    debugger;
    console.log('updatesuccess');
    if (data !== "success") {
        console.log('error', data);
        $('#editContainer').html(data);
        return;
    }
    $('#editModal').modal('hide');
    $('#editContainer').html("");
    $('#grid').dxDataGrid('instance').refresh().done(function (e) { console.log('done', e) });
    window.toastr.success('Saved ');
}

function createFileSpecificationGridActionButtons(container, options) {
    var lnk = '';
    var isRetired = options.data.isRetired;
    var fileNumber = options.data.fileNumber;
    var dataYear = options.data.dataYear;
    var filespecId = options.data.id;

    if (!isRetired) {
        lnk += '<a class="btn btn-default btn-sm btn-grid" href="reports/' + fileNumber + '/' + dataYear + '" data-retire data-filespec-id=' + filespecId + '>Retire</a>&nbsp;';
    }
    if (isRetired) {
        lnk += '<a class="btn btn-default btn-sm btn-grid" href="reports/' + fileNumber + '/' + dataYear + '" data-activate data-filespec-id=' + filespecId + '>Activate</a>&nbsp;';
    }
    lnk += '<a class="btn btn-default btn-sm btn-grid" href="/EditFileSpecification/' + filespecId + '" data-edit data-id="' + filespecId + '">Edit</a>';
    container.append(lnk);
}

function createSubmissionGridActionButtons(container, options) {
    var lnk = '';
    var reportStateId = options.data.reportStateId;
    var canStartReport = options.data.canStartReport;
    var submissionId = options.data.id;
    var fileNumber = options.data.fileNumber;
    var dataYear = options.data.dataYear;

    if (reportStateId !== 1) {
        lnk = '<a class="btn btn-default btn-sm btn-grid" href="reports/' + fileNumber + '/' + dataYear + '">Reports</a>&nbsp;';

    }
    if (reportStateId >= 5) {
        lnk += '<button class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId + '>ReOpen</button>';
    }
    if (reportStateId === 1) {
        if (canStartReport) {
            lnk += '<button class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId + '>Start</button>&nbsp;';

        } else {
            lnk += '<button class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId +
                ' disabled="true" data-toggle="tooltip" data-title="Missing action on specification" title="Missing action on specification" data-placement="left">Start</button>&nbsp;';
        }
        lnk += '<button class="btn btn-default btn-sm btn-grid" data-waiver data-submission-id=' + submissionId + '>Waiver</button>';
    }
    container.append(lnk);
}

function rowPrepared(options) {
    var reportStateId = 0;
    var dueDate = window.moment();
    if (options.data !== undefined) {
        reportStateId = options.data.reportStateId;
        dueDate = options.data.dueDate;
    }

    var classes = [];
    classes = rowStyle(reportStateId, dueDate);
    console.log('cell prepared classes', classes);
    console.log('options', options);
    classes = [];
}

function editorPrepared(info) {
    if (info.parentType === 'filterRow' && info.editorName === "dxSelectBox") {
        info.trueText = "Yes";
        info.falseText = "No";
    }
}

function rowStyle(reportStateId, dueDate) {
    var classes = ['active', 'success', 'info', 'warning', 'danger'];
    var $moment = window.moment();

    console.log($moment.add(14, 'days'));

    if (reportStateId === 6) {
        return {
            classes: classes[4]
        };
    }
    if (reportStateId === 7) {
        return {
            classes: classes[1]
        };
    }
    if ($moment.isSameOrAfter(dueDate)) {
        return {
            classes: classes[3]
        };
    }
    if ($moment.add(14, 'days').isSameOrAfter(dueDate) && reportStateId !== 6) {
        return {
            classes: classes[2]
        };
    }

    return {};
}

$(function () {
    console.log('ready');

    $('body').tooltip({ selector: '[data-toggle=tooltip]' });

    var $grid = $('#grid').dxDataGrid('instance');
    var $log = window.toastr;

    $(document).on('click', '[data-waiver]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('submission-id');
        $.ajax({
            url: '/api/reports/waiver/' + id,
            type: 'POST',
            success: function (data) {
                $log.success('Waived' + data.fileNumber + ' - ' + data.fileName);
                $grid.refresh().done(function (e) { console.log('done', e) });
            },
            error: function (err) {
                console.log('err', err);
            }
        });
    });

    $(document).on('click', '[data-start]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('submission-id');
        $.ajax({
            url: '/api/reports/create/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh().done(function (e) { console.log('done', e) });
                $log.success('Created assignment');
            },
            error: function (err) {
                console.log('err', err);
                $log.error('Something went wrong: ' + err.responseJSON.message);
            }
        });
    });

    $(document).on('click', '[data-history]', function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        var title = $(this).data('title');
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                window.showBSModal({
                    title: title,
                    body: data,
                    size: "lg"
                });
            },
            error: function (err) {
                console.log('err', err);
                $log.error('Error showing history');
            }
        });

    });

    $(document).on('click', '[data-retire]', function (e) {
        e.preventDefault();
        console.log('retire');
        var btn = $(this);
        var id = btn.data('filespec-id');
        $.ajax({
            url: '/api/filespecifications/retire/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh().done(function (e) { console.log('done', e) });
                $log.success('Retired ' + data.fileNumber + ' - ' + data.fileName);
            },
            error: function (err) {
                console.log('err', err);
                $log.error('Something went wrong: ' + err.message);
            }
        });
    });

    $(document).on('click', '[data-activate]', function (e) {
        e.preventDefault();
        console.log('start');
        var btn = $(this);
        var id = btn.data('filespec-id');
        $.ajax({
            url: '/api/filespecifications/activate/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh().done(function (e) { console.log('done', e) });
                $log.success('Activated ' + data.fileNumber + ' - ' + data.fileName);
            },
            error: function (err) {
                console.log('err', err);
                $log.error('Something went wrong: ' + err.message);
            }
        });
    });

    $(document).on('click', '[data-edit]', function (e) {
        e.preventDefault();
        var url = $(this).attr("href");

        console.log('edit url', url);
        //window.showBSModal({
        //    remote: url
        //});

        $.get(url,
            function (data) {
                console.log('data', data);

                $('#editContainer').html(data);
                $('#editModal').modal({ show: true });
            });


        $(document).on('click', '#saveEditSpecificationForm', function (e) {
            e.preventDefault();
            console.log('save');
        });

        

        function UpdateFailed(data) {
            console.log('failed data', data);
            $log.error('Something went wrong: ' + data.message);
        }

    });
});