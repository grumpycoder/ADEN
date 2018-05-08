// submission.js
$(function () {
    console.log('submission ready'); 
    var $grid = $('#grid').dxDataGrid('instance');

    $(document).on('click', '[data-waiver]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('submission-id');
        

        window.$toggleWorkingButton(btn);

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
        }).always(function () {
            window.$toggleWorkingButton(btn);
        });
    });

    $(document).on('click', '[data-start]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('submission-id');

        window.$toggleWorkingButton(btn);

        $.ajax({
            url: '/api/reports/create/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Created assignment');
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            }
        }).always(function () {
            window.$toggleWorkingButton(btn);
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
                window.$log.error('Error showing history');
            }
        });

    });

});

function createSubmissionGridActionButtons(container, options) {
    var lnk = '';
    var submissionStateId = options.data.submissionStateId;
    var canStartReport = options.data.canStartReport;
    var hasAdmin = options.data.hasAdmin;
    var submissionId = options.data.id;
    var fileNumber = options.data.fileNumber;
    var dataYear = options.data.dataYear;

    if (submissionStateId !== 1) {
        lnk = '<a class="btn btn-default btn-sm btn-grid" href="/reports/' + dataYear + '/' + fileNumber + '">Reports</a>&nbsp;';

    }
    if (submissionStateId >= 5 && hasAdmin) {
        lnk += '<button class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> ReOpen</button>';
    }
    if (submissionStateId === 1 && hasAdmin) {
        if (canStartReport) {
            lnk += '<button class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> Start</button>&nbsp;';

        } else {
            lnk += '<button class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId +
                ' disabled="true" data-toggle="tooltip" data-title="Missing action on specification" title="Missing action on specification" data-placement="left"><i class="fa fa-spinner fa-spin hidden"></i> Start</button>&nbsp;';
        }
        lnk += '<button class="btn btn-default btn-sm btn-grid" data-waiver data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> Waiver</button>';
    }
    container.append(lnk);
}

function rowStyle(submissionState, dueDate) {
    var classes = ['active', 'success', 'info', 'warning', 'danger'];
    var $moment = window.moment();

    //TODO: What to do about CompletedWithError

    if (submissionState === 'Completed' || submissionState === 'Waived') {
        return classes[1];
    }

    if (submissionState === 'CompleteWithErrors') {
        console.log('complete with errors');
        return classes[2];
    }

    if (submissionState !== 'Completed' && $moment.isSameOrAfter(dueDate)) {
        return classes[4];
    }

    if (submissionState !== 'Completed' && $moment.add(14, 'days').isSameOrAfter(dueDate)) {
        return classes[3];
    }

    return '';
}

function rowPrepared(row) {
    if (row.rowType === 'data') {
        var css = rowStyle(row.data.submissionStateKey, row.data.dueDate);
        row.rowElement.addClass(css);
    }
}