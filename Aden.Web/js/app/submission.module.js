// submission.module.js
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
                                        window.$log.error('Error: ' + error.resonseJson.message);
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
                                        window.$log.error('Error: ' + error.resonseJson.message);
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

        console.log('id', id);
        $.ajax({
            url: '/api/report/cancel/' + id,
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
    var canCancel = options.data.canCancel;
    var canStart = options.data.canStart;
    var canWaiver = options.data.canWaiver;
    var canReview = options.data.canReview;

    var submissionStateId = options.data.submissionStateId;
    var hasAdmin = options.data.hasAdmin;
    var submissionId = options.data.id;
    var fileNumber = options.data.fileNumber;
    var dataYear = options.data.dataYear;
    if (canReview) {
        lnk += '<a class="btn btn-default btn-grid" href="/reports/' + dataYear + '/' + fileNumber + '">Review File</a>&nbsp;';
    }
    if (canCancel) {
        lnk += '<a href="#" class="btn btn-default btn-grid" data-cancel data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> Cancel</a>&nbsp;';
    }
    if (submissionStateId >= 5 && hasAdmin) {
        lnk += '<a href="/audit/' + submissionId + '" class="btn btn-default btn-grid" data-reopen data-title="ReOpen Reason" data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> ReOpen</a>&nbsp;';
    }
    if (canStart) {
        lnk += '<a href="#" class="btn btn-default btn-grid" data-start data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> Start</a>&nbsp;';
    }
    if (canWaiver) {
        lnk += '<a href="/audit/' + submissionId + '" class="btn btn-default btn-grid" data-waiver data-title="Waiver Reason" data-submission-id=' + submissionId + '><i class="fa fa-spinner fa-spin hidden"></i> Waiver</a>&nbsp;';
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

