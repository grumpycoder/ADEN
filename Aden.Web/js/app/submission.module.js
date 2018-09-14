// submission.module.js
$(function () {
    console.log('submission ready');
    var $grid = $('#grid').dxDataGrid('instance');

    $(document).on('click', '[data-waiver]', function (e) {
        e.preventDefault();
        var btn = $(this);
        window.$toggleWorkingButton(btn);
        var id = btn.data('submission-id');
        $.ajax({
            url: '/api/report/waiver/' + id,
            type: 'POST'
        })
            .done(function (data) {
                $grid.refresh().done(function (e) { });
                window.$log.success('Waived' + data.fileNumber + ' - ' + data.fileName);
            })
            .fail(function (err) {
                //console.log('err', err);
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            })
            .always(function () {
                window.$toggleWorkingButton(btn, 'off');
            });

    });

    $(document).on('click', '[data-start]', function (e) {
        e.preventDefault();
        var btn = $(this);
        window.$toggleWorkingButton(btn);
        var id = btn.data('submission-id');


        $.ajax({
            url: '/api/report/create/' + id,
            type: 'POST'
        })
            .done(function () {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Created assignment');
            })
            .fail(function (err) {
                //console.log('err', err);
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            })
            .always(function () {
                window.$toggleWorkingButton(btn, 'off');
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
                    size: "large"
                });
            },
            error: function (err) {
                //console.log('err', err);
                window.$log.error('Error showing history');
            }
        });

    });

    $(document).on('click', '[data-reassign]', function (e) {
        e.preventDefault();
        var id = $(this).data('workitem-id');
        var url = $(this).attr('href') + '/' + id;
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                window.showBSModal({
                    title: 'Reassign Task',
                    body: data,
                    size: "lg",
                    actions: [
                        {
                            label: 'Cancel',
                            cssClass: 'btn-default',
                            onClick: function (e) {
                                //console.log('e', e);
                                $(e.target).parents('.modal').modal('hide');
                                $('body').removeClass('modal-open');
                                //modal-open class is added on body so it has to be removed

                                $('.modal-backdrop').remove();
                                //need to remove div with modal-backdrop class
                            }
                        },
                        {
                            label: 'Save',
                            cssClass: 'btn-primary',
                            onClick: function (e) {
                                window.$showModalWorking();
                                var formData = $('form').serialize();
                                $.ajax({
                                    type: "POST",
                                    url: '/api/assignment/reassign',
                                    data: model = formData
                                }).done(function (data) {
                                    window.$log.success('Reassigned task');
                                }).fail(function (err) {
                                    window.$log.error('Failed to reassign task. ' + error);
                                }).always(function () {
                                    $('.modalContainer').html('');
                                    $('.modal').modal('hide');

                                    $(e.target).parents('.modal').modal('hide');
                                    $('body').removeClass('modal-open');
                                    //modal-open class is added on body so it has to be removed

                                    $('.modal-backdrop').remove();
                                    //need to remove div with modal-backdrop class
                                    window.$hideModalWorking();
                                });


                            }
                        }
                    ]
                });
            },
            error: function (err) {
                //console.log('err', err);
                window.$log.error('Error showing reassignment');
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

    //TODO: What to do about CompletedWithError color

    if (submissionState === 'Completed' || submissionState === 'Waived') {
        return classes[1];
    }

    if (submissionState === 'CompleteWithErrors') {
        //console.log('complete with errors');
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

