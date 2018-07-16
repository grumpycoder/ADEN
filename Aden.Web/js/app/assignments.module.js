// assignments.module.js

$(function () {
    console.log('assignments ready');

    var $gridCurrentAssignments = $('#gridCurrentAssignments').dxDataGrid('instance');
    var $gridRetrievableAssignments = $('#gridRetrievableAssignments').dxDataGrid('instance');
    
    $(document).on('click', '[data-workitem-id]', function (e) {
        e.preventDefault();
        console.log('work item click');
        var btn = $(this);
        var id = btn.data('workitem-id');
        var url = '/api/assignment/complete/' + id;

        window.$toggleWorkingButton(btn);
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                $gridCurrentAssignments.refresh();
                $gridRetrievableAssignments.refresh();
                window.$log.success(data.action + ' - ' + data.state);
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            }
        });
    });

    $(document).on('click', 'button[data-cancel-workitem]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('cancel-workitem-id');
        var url = '/api/assignment/cancel/' + id;
        window.$toggleWorkingButton(btn);
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                $gridCurrentAssignments.refresh().done(function (e) { console.log('done', e) });
                $gridRetrievableAssignments.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Cancelled ' + data.action + ' - ' + data.state);
                window.$toggleWorkingButton(btn);
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
                window.$toggleWorkingButton(btn);
            }
        });
    });

    $(document).on('click', '[data-upload-report]', function (e) {
        e.preventDefault();
        console.log('upload report');
        var url = $(this).attr("href");

        var title = 'Upload Report';
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
                                console.log('e', e);
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
                                var id = $('#Id').val();

                                var url = '/api/assignment/submitreport/' + id;
                                var formData = new FormData();
                                var fileInput = $('#files').get(0);
                                var files = fileInput.files;

                                window.$showModalWorking();

                                for (var i = 0; i < files.length; i++) {
                                    formData.append(files[i].name, files[i]);
                                }

                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    data: formData,
                                    dataType: 'json',
                                    contentType: false,
                                    processData: false,
                                    success: function (response) {
                                        $gridCurrentAssignments.refresh();
                                        $gridRetrievableAssignments.refresh();
                                        window.$log.success('Submitted report');
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
                window.$log.error('Error showing report upload');
            }
        });
    });

    $(document).on('click', '[data-submit-error]', function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        var title = 'Submission Error';
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
                                var id = $('#Id').val();
                                var url = '/assignment/submiterror/';

                                window.$showModalWorking();

                                var formData = new FormData();
                                formData.append('Id', id);
                                formData.append('Note', $('#note').val());

                                var files = document.getElementById('files').files;
                                if (files.length > 0) {
                                    for (var i = 0; i < files.length; i++) {
                                        formData.append('files', files[i]);
                                    }
                                }

                                $.ajax({
                                    type: "POST",
                                    url: url,
                                    data: formData,
                                    contentType: false,
                                    processData: false,
                                    success: function (response) {
                                        console.log('success', response);
                                        $gridCurrentAssignments.refresh();
                                        $gridRetrievableAssignments.refresh();
                                        window.$log.success('Submitted report');
                                    },
                                    error: function (error) {
                                        console.log('error', error);
                                        window.$log.error('Error: ' + error.statusText);
                                    },
                                    complete: function () {
                                        $('.modalContainer').html('');
                                        $('.modal').modal('hide');
                                        console.log('complete');
                                        window.$hideModalWorking();
                                    }
                                });
                            }
                        }
                    ]
                });
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Error showing history');
            }
        });
    });
    
});

function createAssignmentsGridActionButtons(container, options) {
    var reportId = options.data.reportId;
    var action = options.data.action;
    var workItemId = options.data.id;
    var isManualUpload = options.data.isManualUpload;

    var lnk = '';

    if (action === 'Generate' && isManualUpload === true) {
        lnk += '<a class="btn btn-primary btn-grid" data-upload-report href="/uploadreport/' + workItemId + '">Upload</a>';
    }
    else {
        lnk +=
            '<button class="btn btn-success btn-grid" data-report-id="' +
            reportId +
            '" data-workitem-id="' +
            workItemId +
            '" data-workitem-action="' +
            action +
            '"><i class="fa fa-spinner fa-spin hidden"></i> ' +
            action +
            '</button>&nbsp;';
    }

    //Show Error opton link if work item in Submit mode
    if (action === 'Submit') {
        lnk +=
            '<a href="/ErrorReport/' + workItemId + '" class="btn btn-danger btn-grid" data-submit-error><i class="fa fa-spinner fa-spin hidden"></i> Errors</a>&nbsp;';
    }
    //Show Report Link if already documents generated 
    if (action !== 'Generate') {
        lnk += '<a class="btn btn-default btn-grid" href="/reports/' + options.data.dataYear + '/' + options.data.fileNumber + '">Report</a>&nbsp;';
    }

    container.append(lnk);
}


function createGridCancelActionButtons(container, options) {
    var action = options.data.action;
    var workItemId = options.data.id;
    var lnk = '';
    if (options.data.canCancel) lnk = '<button class="btn btn-default btn-grid" data-cancel-workitem data-cancel-workitem-id="' + workItemId + '"><i class="fa fa-spinner fa-spin hidden"></i> Cancel ' + action + '</button>';
    container.append(lnk);
}
