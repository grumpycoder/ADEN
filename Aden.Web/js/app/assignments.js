// assignments.js

$(function() {
    console.log('assignments ready');
    
    var $gridCurrentAssignments = $('#gridCurrentAssignments').dxDataGrid('instance');
    var $gridRetrievableAssignments = $('#gridRetrievableAssignments').dxDataGrid('instance');

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
                window.$log.success('Submitted errors');
            },
            error: function (error) {
                console.log('error', error);
                window.$log.error('Something went wrong. ' + error.resonseJson.message);
                $('#workItemModal').modal('hide');
                $('#workItemContainer').html("");
            },
            complete: function () {
                $('.modal-dialog').removeClass('loader');
            }
        });
    });

    $(document).on('click', '[data-workitem-id]', function (e) {
        e.preventDefault();
        console.log('work item click');
        var btn = $(this);
        var id = btn.data('workitem-id');
        window.$toggleWorkingButton(btn);
        $.ajax({
            url: '/api/wi/complete/' + id,
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

    $(document).on('click', '[data-submit-error]', function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        $.get(url, function (data) {
            $('#workItemContainer').html(data);
            $('#workItemModal').modal({ show: true });
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
                window.$log.success('Cancelled ' + data.action + ' - ' + data.state);
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            }
        });

    });


});

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
            '"><i class="fa fa-spinner fa-spin hidden"></i> ' +
            action +
            '</button>&nbsp;';

    if (action !== 'Generate') {
        lnk += '<a class="btn btn-default btn-sm btn-grid" href="/reports/' + options.data.dataYear + '/' + options.data.fileNumber + '">Report</a>&nbsp;';
    }
    if (action === 'Submit') {
        lnk +=
            '<a href="/EditWorkItem/' + workItemId + '" class="btn btn-danger btn-grid" data-submit-error><i class="fa fa-spinner fa-spin hidden"></i> Submit With Errors</a>';
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
