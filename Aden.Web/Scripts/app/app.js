function OnWorkItemUpdateFormComplete(data) {
    console.log('workitem complete');
    if (data.responseText === 'success') {
        $('#workItemModal').modal('hide');
        $('#workItemContainer').html("");
        $('#gridCurrentAssignments').dxDataGrid('instance').refresh().done(function (e) { console.log('done', e) });
        $('#gridRetrievableAssignments').dxDataGrid('instance').refresh().done(function (e) { console.log('done', e) });
        window.toastr.success('Saved ');
    }
}

function OnSpecificationUpdateFormComplete(data) {
    if (data.responseText === 'success') {
        $('#editModal').modal('hide');
        $('#editContainer').html("");
        $('#grid').dxDataGrid('instance').refresh().done(function (e) { console.log('done', e) });
        window.toastr.success('Saved ');
    }
}

function createAssignmentsGridActionButtons(container, options) {
    var reportId = options.data.reportId;
    var action = options.data.action;
    var workItemId = options.data.id;

    var lnk =
        '<button class="btn btn-primary btn-grid" data-report-id="' +
            reportId +
            '" data-workitem-id="' +
            workItemId +
            '" data-workitem-action="' +
            action +
            '">' +
            action +
            '</button>&nbsp;';
    if (action === 'Submit') {
        lnk +=
            '<a href="/EditWorkItem/' + workItemId + '" class="btn btn-danger btn-grid" data-submit-error>Submit With Errors</a>';
    }

    container.append(lnk);
}

function createGridCancelActionButtons(container, options) {
    var action = options.data.action;
    var workItemId = options.data.id;
    var lnk = '';
    if (options.data.canCancel) lnk = '<button class="btn btn-default btn-grid" data-cancel-workitem data-cancel-workitem-id="' + workItemId + '">Cancel ' + action + '</button>';
    container.append(lnk);
}

function createReportDocumentLinks(container, options) {
    var documents = options.data.documents;

    var lnk = '<span><ul class="list-unstyled">';
    documents.forEach(function (doc) {
        lnk += '<li>' +
            '<a href="#myModal" ' +
            'data-document-viewer ' +
            'data-doc-id="' + doc.id + '" ' +
            'data-url="/document/' + doc.id + '" ' +
            'data-toggle="modal" ' +
            'data-target="#myModal" ' +
            'data-title="' + doc.filename + '" ' +
            'data-toggle="tooltip" ' +
            'title="' + doc.filename + '" ' +
            'data-placement="right" ' +
            'data-original-title="' + doc.filename + '">' + doc.filename +
            '</a>' +
            '</li>';
    });
    lnk += '</ul></span>';
    container.append(lnk);
}

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

function createSubmissionGridActionButtons(container, options) {
    var lnk = '';
    var reportStateId = options.data.reportStateId;
    var canStartReport = options.data.canStartReport;
    var submissionId = options.data.id;
    var fileNumber = options.data.fileNumber;
    var dataYear = options.data.dataYear;

    if (reportStateId !== 1) {
        lnk = '<a class="btn btn-default btn-sm btn-grid" href="/reports/' + dataYear + '/' + fileNumber + '">Reports</a>&nbsp;';

    }
    if (reportStateId >= 5) {
        lnk += '<button class="btn btn-default btn-sm btn-grid" data-start data-submission-id=' + submissionId + '>ReOpen</button>';
    }
    if (reportStateId === 1) {
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

function rowPrepared(options) {
    var reportStateId = 0;
    var dueDate = window.moment();
    if (options.data !== undefined) {
        reportStateId = options.data.reportStateId;
        dueDate = options.data.dueDate;
    }

    var classes = [];
    classes = rowStyle(reportStateId, dueDate);
    //console.log('cell prepared classes', classes);
    //console.log('options', options);
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

function toggleWorkingButton(button) {
    button.prop('disabled', !button.prop('disabled'));
    button.find('i').toggleClass('hidden');
}

$(function () {
    console.log('ready');

    $('body').tooltip({ selector: '[data-toggle=tooltip]' });

    var $grid = $('#grid').dxDataGrid('instance');
    var $gridCurrentAssignments = $('#gridCurrentAssignments').dxDataGrid('instance');
    var $gridRetrievableAssignments = $('#gridRetrievableAssignments').dxDataGrid('instance');

    var $log = window.toastr;

    $(document).on('click', '[data-waiver]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('submission-id');
        toggleWorkingButton(btn);

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
            toggleWorkingButton(btn);
        });
    });

    $(document).on('click', '[data-start]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('submission-id');

        toggleWorkingButton(btn);

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
        }).always(function () {
            toggleWorkingButton(btn);
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
        var btn = $(this);
        var id = btn.data('filespec-id');

        toggleWorkingButton(btn);

        $.ajax({
            url: '/api/filespecifications/retire/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh();
                $log.success('Retired ' + data.fileNumber + ' - ' + data.fileName);
            },
            error: function (err) {
                console.log('err', err);
                $log.error('Something went wrong: ' + err.message);
            }
        }).always(function () {
            toggleWorkingButton(btn);
        });

    });

    $(document).on('click', '[data-activate]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('filespec-id');

        toggleWorkingButton(btn);

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
        }).always(function() {
            toggleWorkingButton(btn);
        });
    });

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

    $(document).on('click', '[data-document-viewer]', function (e) {
        var $currentTarget = $(e.currentTarget);
        var targetModal = $currentTarget.data('target');
        var $modal = $(targetModal);

        var remoteContent = $currentTarget.data('url');
        var title = $currentTarget.data('doc-title');
        var id = $currentTarget.data('doc-id');

        var modalBody = $(targetModal + ' .modal-body');
        var modalTitle = $(targetModal + ' .modal-title');

        modalTitle.html(title);
        $modal.on('show.bs.modal', function () {
            var lnk = $(this).find('a[download]');
            lnk.attr('href', '/download/' + id);
            modalBody.load(remoteContent);
        }).modal();
        return false;
    });

    $(document).on('click', '[data-workitem-id]', function (e) {
        e.preventDefault();
        console.log('work item click');
        var btn = $(this);
        var id = btn.data('workitem-id');
        $.ajax({
            url: '/api/wi/complete/' + id,
            type: 'POST',
            success: function (data) {
                $gridCurrentAssignments.refresh();
                $gridRetrievableAssignments.refresh();
                $log.success(data.action + ' - ' + data.state);
            },
            error: function (err) {
                console.log('err', err);
                $log.error('Something went wrong: ' + err.responseJSON.message);
            }
        });
    });

    $(document).on('click', 'button[data-cancel-workitem]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('cancel-workitem-id');
        $.ajax({
            url: '/api/wi/undo/' + id,
            type: 'POST',
            success: function (data) {
                $gridCurrentAssignments.refresh().done(function (e) { console.log('done', e) });
                $gridRetrievableAssignments.refresh().done(function (e) { console.log('done', e) });
                $log.success('Cancelled ' + data.action + ' - ' + data.state);
            },
            error: function (err) {
                console.log('err', err);
                $log.error('Something went wrong: ' + err.responseJSON.message);
            }
        });

    });

    $(document).on('click', '[data-submit-error]', function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        $.get(url, function (data) {
            $('#workItemContainer').html(data);
            $('#workItemModal').modal({ show: true });
        });
    });

    $(document).on('submit', '#formSubmitWithError', function (e) {
        e.preventDefault();
        $('.modal-dialog').addClass('loader');

        var formData = new FormData();
        var files = document.getElementById("files").files;

        formData.append("Notes", $("#Notes").val());
        formData.append("Id", $("#Id").val());

        if (files.length > 0) {
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }
        }

        $.ajax({
            type: "POST",
            url: '/saveworkitem',
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                $('#workItemModal').modal('hide');
                $('#workItemContainer').html("");
                $gridCurrentAssignments.refresh();
                $gridRetrievableAssignments.refresh();
                $log.success('Submitted errors');
            },
            error: function (error) {
                console.log('error', error);
                $log.error('Something went wrong. ' + error.resonseJson.message);
                $('#workItemModal').modal('hide');
                $('#workItemContainer').html("");
            },
            complete: function () {
                $('.modal-dialog').removeClass('loader');
            }
        });
    });


});