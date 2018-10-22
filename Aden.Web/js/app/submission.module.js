﻿// submission.module.js
$(function () {
    console.log('submission ready');
    var $grid = $('#grid').dxDataGrid('instance');

    $(document).on('click', '[data-waiver]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('submission-id');
        var title = btn.data('title');
        var url = $(this).attr("href");
        var postUrl = '/api/submission/waiver/' + id;

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.showBSModal({
                    title: title,
                    body: data,
                    size: "large",
                    actions: [
                        {
                            label: 'Cancel',
                            cssClass: 'btn-default',
                            onClick: function (e) {
                                $(e.target).parents('.modal').modal('hide');
                                $('body').removeClass('modal-open');
                                //modal-open class is added on body so it has to be removed

                                $('.modal-backdrop').remove();
                                //need to remove div with modal-backdrop class
                            }
                        },
                        {
                            label: 'Submit',
                            cssClass: 'btn-primary',
                            onClick: function (e) {
                                window.$showModalWorking();
                                $.ajax({
                                    type: "POST",
                                    url: postUrl,
                                    data: $('form').serialize(),
                                    dataType: 'json',
                                    success: function (response) {
                                        $grid.refresh().done(function (e) { console.log('done', e) });
                                        window.$log.success('Waived submission');
                                    },
                                    error: function (error) {
                                        window.$log.error('Error: ' + error.responseJSON.message);
                                    },
                                    complete: function () {
                                        $('.modalContainer').html('');
                                        $('.modal').modal('hide');

                                        $(e.target).parents('.modal').modal('hide');
                                        $('body').removeClass('modal-open');
                                        //modal-open class is added on body so it has to be removed

                                        $('.modal-backdrop').remove();
                                        //need to remove div with modal-backdrop class

                                        window.$hideModalWorking();
                                    }
                                });
                            }
                        }
                    ]
                });
            },
            error: function (err) {
                window.$log.error('Error showing audit entry');
            }
        });
    });

    $(document).on('click', '[data-start]', function (e) {
        e.preventDefault();
        var btn = $(this);
        window.$toggleWorkingButton(btn);
        var id = btn.data('submission-id');


        $.ajax({
            url: '/api/submission/start/' + id,
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

    $(document).on('click', '[data-reopen]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var title = btn.data('title');
        var id = btn.data('submission-id');
        var url = $(this).attr("href");
        var postUrl = '/api/submission/reopen/' + id;

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.showBSModal({
                    title: title,
                    body: data,
                    size: "large",
                    actions: [
                        {
                            label: 'Cancel',
                            cssClass: 'btn-default',
                            onClick: function (e) {
                                $(e.target).parents('.modal').modal('hide');
                                $('body').removeClass('modal-open');
                                //modal-open class is added on body so it has to be removed

                                $('.modal-backdrop').remove();
                                //need to remove div with modal-backdrop class
                            }
                        },
                        {
                            label: 'Submit',
                            cssClass: 'btn-primary',
                            onClick: function (e) {

                                window.$showModalWorking();
                                $.ajax({
                                    type: "POST",
                                    url: postUrl,
                                    data: $('form').serialize(),
                                    dataType: 'json',
                                    success: function (response) {
                                        $grid.refresh().done(function (e) { console.log('done', e) });
                                        window.$log.success('ReOpened submission');
                                    },
                                    error: function (error) {
                                        console.log('error', error);
                                        window.$log.error('Error: ' + error.responseJSON.message);
                                    },
                                    complete: function () {
                                        $('.modalContainer').html('');
                                        $('.modal').modal('hide');

                                        $(e.target).parents('.modal').modal('hide');
                                        $('body').removeClass('modal-open');
                                        //modal-open class is added on body so it has to be removed

                                        $('.modal-backdrop').remove();
                                        //need to remove div with modal-backdrop class

                                        window.$hideModalWorking();
                                    }
                                });
                            }
                        }
                    ]
                });
            },
            error: function (err) {
                window.$log.error('Error showing audit entry');
            }
        });

    });

    $(document).on('click', '[data-cancel]', function (e) {
        e.preventDefault();
        var btn = $(this);
        window.$toggleWorkingButton(btn);
        var id = btn.data('submission-id');

        $.ajax({
            url: '/api/submission/cancel/' + id,
            type: 'POST'
        })
            .done(function () {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Cancelled assignments');
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
        console.log('url', url);
        window.BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
            draggable: true,
            title: title,
            message: $('<div></div>').load(url, function (resp, status, xhr) {
                if (status === 'error') {
                    window.$log.error('Error showing history');
                }
            }),
            buttons: [{
                label: 'Close',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
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

    $(document).on('click', '[data-image-viewer]', function (e) {
        e.preventDefault();

        var btn = $(this);
        var workItemId = btn.data('workitem-id');
        var url = '/workitemimages/' + workItemId;
        var title = 'Work Item Images';

        window.BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_WIDE,
            draggable: true,
            title: title,
            message: $('<div></div>').load(url),
            buttons: [{
                label: 'Close',
                action: function (dialogRef) {
                    dialogRef.close();
                }
            }]
        });

    });

    $('[data-toggle="tooltip"]').tooltip(); 

});

function createSubmissionGridActionButtons(container, options) {
    var lnk = '';
    var canCancel = options.data.canCancel;
    var canStart = options.data.canStart;
    var canReopen = options.data.canReopen;
    var canWaiver = options.data.canWaiver;
    var canReview = options.data.canReview;
    var startDisabled = options.data.startDisabled;
    var reopenDisabled = options.data.reopenDisabled;

    var submissionId = options.data.id;
    var fileNumber = options.data.fileNumber;
    var dataYear = options.data.dataYear;
    if (canReview) {
        lnk += '<a class="btn btn-success btn-sm btn-grid" href="/reports/' + dataYear + '/' + fileNumber + '">Review File</a>&nbsp;';
    }
    if (canCancel) {
        lnk += '<a href="#" class="btn btn-default btn-sm btn-grid" data-cancel data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> Cancel</a>&nbsp;';
    }
    if (canReopen) {
        lnk += '<a href="/audit/' +
            submissionId +
            '" class="btn btn-default btn-sm btn-grid" ' +
            'data-reopen data-title="ReOpen Reason" data-submission-id=' +
            submissionId;
        if (reopenDisabled) lnk += ' disabled data-toggle="tooltip" data-placement="left" title="Missing group assignments or report action"';
        lnk += '><i class="fa fa-spinner fa-spin hidden"></i> ReOpen</a>&nbsp;';
    }
    if (canStart) {
        lnk += '<a href="#" class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId;
        if (startDisabled) lnk += ' disabled data-toggle="tooltip" data-placement="left" title="Missing group assignments or report action"';
        lnk += '><i class="fa fa-spinner fa-spin hidden"></i> Start</a>&nbsp;';
    }
    if (canWaiver) {
        lnk += '<a href="/audit/' + submissionId + '" class="btn btn-default btn-sm btn-grid" data-waiver data-title="Waiver Reason" data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> Waiver</a>&nbsp;';
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
        var css = rowStyle(row.data.submissionStateDisplay, row.data.dueDate);
        row.rowElement.addClass(css);
    }
}

