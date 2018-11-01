// assignments.module.js

$(function () {
    console.log('assignments ready');

    var $gridCurrentAssignments = $('#gridCurrentAssignments').dxDataGrid('instance');
    var $gridRetrievableAssignments = $('#gridRetrievableAssignments').dxDataGrid('instance');
    
    $(document).on('click', '[data-workitem-action]', function (e) {
        e.preventDefault();
        var btn = $(this);
        window.$toggleWorkingButton(btn);
        var id = btn.data('workitem-id');
        var action = btn.data('workitem-action');
        var url = '/api/assignment/complete/' + id;
        if (action === 'reject') url = '/api/assignment/reject/' + id;
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                $gridCurrentAssignments.refresh();
                $gridRetrievableAssignments.refresh();
                window.$log.success('Completed ' + data.action + ' assignment');
            },
            error: function (err) {
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            }
        }).always(function () {
            window.$toggleWorkingButton(btn);
        });
    });

    $(document).on('click', 'button[data-cancel-workitem]', function (e) {
        e.preventDefault();
        var btn = $(this);
        window.$toggleWorkingButton(btn);
        var id = btn.data('cancel-workitem-id');
        var url = '/api/assignment/cancel/' + id;
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                $gridCurrentAssignments.refresh().done(function (e){});
                $gridRetrievableAssignments.refresh().done(function(e) {});
                window.$log.success('Cancelled ' + data.action + ' assignment');
            },
            error: function (err) {
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            }
        }).always(function () {
            window.$toggleWorkingButton(btn);
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

        var id = $(this).data('workitem-id');
        var url = '/ErrorReport/' + id;  
        var postUrl = '/assignment/submiterror/';
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
                       

                                window.$showModalWorking();

                                var formData = new FormData();
                                formData.append('Id', id);
                                formData.append('Description', $('#description').val());

                                var files = document.getElementById('files').files;
                                if (files.length > 0) {
                                    for (var i = 0; i < files.length; i++) {
                                        formData.append('files', files[i]);
                                    }
                                }
                                console.log('id', id);
                                $.ajax({
                                    type: "POST",
                                    url: postUrl,
                                    data: formData,
                                    contentType: false,
                                    processData: false,
                                    success: function (response) {
                                        $gridCurrentAssignments.refresh();
                                        $gridRetrievableAssignments.refresh();
                                        window.$log.success('Submitted report');
                                    },
                                    error: function (error) {
                                        window.$log.error('Error: ' + error.statusText);
                                    },
                                    complete: function () {
                                        $('.modalContainer').html('');
                                        $('.modal').modal('hide');
                                        window.$hideModalWorking();
                                    }
                                });
                            }
                        }
                    ]
                });
            },
            error: function (err) {
                window.$log.error('Error showing history');
            }
        });
    });

    $(document).on('click', '[data-image-viewer]', function (e) {
        e.preventDefault();
        var btn = $(this);

        var url = $(this).attr('href');
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
});

function createAssignmentsGridActionButtons(container, options) {
    var reportId = options.data.reportId;
    var canGenerate = options.data.canGenerate; 
    var canReject = options.data.canReject; 
    var canSubmit = options.data.canSubmit; 
    var canReviewError = options.data.canReviewError; 

    var action = options.data.action;
    var actionName = options.data.actionName;
    var actionDescription = options.data.actionDescription;
    var workItemId = options.data.id;
    var isManualUpload = options.data.isManualUpload;
    var lnk = '';

    if (canGenerate && isManualUpload === true) {
        lnk += '<a class="btn btn-primary btn-sm btn-grid" data-upload-report href="/uploadreport/' + workItemId + '">Upload</a>';
    }
    else {
        lnk +=
            '<button class="btn btn-success btn-sm btn-grid" data-report-id="' +
            reportId +
            '" data-workitem-id="' +
            workItemId +
            '" data-workitem-action="' +
            action +
            '"><i class="fa fa-spinner fa-spin hidden"></i> ' +
            actionName +
            '</button>';
    }

    if (canReject) {
        lnk +=
            '<button class="btn btn-danger btn-sm btn-grid" data-report-id="' +
            reportId +
            '" data-workitem-id="' +
            workItemId +
            '" data-workitem-action="' +
            'reject' +
            '"><i class="fa fa-spinner fa-spin hidden"></i>Reject</button>';
    }
     
    if (canSubmit) {
        lnk +=
            '<button class="btn btn-danger btn-sm btn-grid" data-submit-error data-workitem-id=' + workItemId + '><i class="fa fa-spinner fa-spin hidden"></i> Report Errors</button>';
    }

    //Show details link if action is to review error
    if (canReviewError) lnk += '<button class="btn btn-primary btn-sm btn-grid" data-image-viewer href="/workitemimages/' + workItemId + '">View Details</button>';

    //Show Report Link if already documents generated 
    if (!canGenerate) {
        lnk += '<a class="btn btn-default btn-sm btn-grid" href="/reports/' + options.data.dataYear + '/' + options.data.fileNumber + '">Review File</a>';
    }

    container.append(lnk);
}


function createGridCancelActionButtons(container, options) {
    var workItemId = options.data.id;
    var canCancel = options.data.canCancel; 
    var lnk = '';
    if (canCancel) lnk = '<button class="btn btn-default btn-grid" data-cancel-workitem data-cancel-workitem-id="' + workItemId + '"><i class="fa fa-spinner fa-spin hidden"></i> Cancel</button>';
    container.append(lnk);
}
