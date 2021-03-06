﻿// submission.module.js
$(function () {
    console.log('submission ready');
    //var $grid = $('#grid').dxDataGrid('instance');

    var uri = "/api/submission";

    var $grid = $('#grid').dxDataGrid({
        dataSource: DevExpress.data.AspNet.createStore({
            key: 'id',
            loadUrl: uri,
            updateUrl: uri,
        }),
        remoteOperations: true,

        columns: [
            {
                type: "buttons",
                buttons: ["edit", "delete"]
            },
            //{
            //    width: 50,
            //    alignment: 'center',
            //    cellTemplate: function (container, options) {
            //        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
            //            .text('Start')
            //            .on('dxclick',
            //                function (e) {
            //                    //Do something with options.data;
            //                    console.log('container', container);
            //                })
            //            .appendTo(container);
            //    }
            //},
            { dataField: 'fileNumber', caption: 'File Number' },
            { dataField: 'fileName', caption: 'File Name' },
            { dataField: 'submissionStateDisplay', caption: 'Status' },
            { dataField: 'currentAssignee', caption: 'Assigned' },
            { dataField: 'lastUpdatedFriendly', caption: 'Last Update' },
            { dataField: 'deadlineDate', caption: 'Submission Deadline', dataType: 'date', },
            { dataField: 'submissionDate', caption: 'Date Submitted', dataType: 'date', },
            { dataField: 'displayDataYear', caption: 'Data Year' },
            { dataField: 'section', caption: 'Section' },
            {
                dataField: 'isSEA', caption: 'SEA',
                dataType: 'boolean',
                visible: true,
                showEditorAlways: false,
                trueText: 'Yes',
                falseText: 'No',
                customizeText: function (cellInfo) {
                    if (cellInfo.value) return 'Yes';

                    return 'No';
                },
            },
            {
                dataField: 'isLEA', caption: 'LEA', dataType: 'boolean',
                visible: true,
                showEditorAlways: false,
                trueText: 'Yes',
                falseText: 'No',
                customizeText: function (cellInfo) {
                    if (cellInfo.value) return 'Yes';

                    return 'No';
                },
            },
            {
                dataField: 'isSCH', caption: 'SCH',
                dataType: 'boolean',
                visible: true,
                showEditorAlways: false,
                trueText: 'Yes',
                falseText: 'No',
                customizeText: function (cellInfo) {
                    if (cellInfo.value) return 'Yes';

                    return 'No';
                },
            },
            {
                width: 200,
                alignment: 'center',
                cellTemplate: function (container, options) {
                    if (options.data.canStart) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Start')
                            .on('dxclick',
                                function (e) {
                                    //Do something with options.data;
                                    console.log('container', container);
                                    startWorkFlow($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canCancel) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Cancel')
                            .on('dxclick',
                                function () {
                                    cancelWorkFlow($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canReview) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Review File')
                            .attr('href', '/report/' + options.data.dataYear + '/' + options.data.fileNumber)
                            .attr('target', '_blank')
                            .appendTo(container);
                    }

                    if (options.data.canReopen) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Reopen')
                            .on('dxclick',
                                function (e) {
                                    reopenSubmission($(this), options.data);
                                })
                            .appendTo(container);
                    }

                    if (options.data.canWaiver) {
                        $('<a/>').addClass('btn btn-default btn-sm btn-grid')
                            .text('Waiver')
                            .on('dxclick',
                                function (e) {
                                    waiverWorkFlow($(this), options.data);
                                })
                            .appendTo(container);
                    }



                }
            },
        ],
        wordWrapEnabled: true,
        allowColumnResizing: true,
        columnResizingMode: "nextColumn",
        columnAutoWidth: true,
        columnChooser: {
            enabled: true
        },
        stateStoring: {
            enabled: true,
            type: "localStorage",
            storageKey: "gridFilterStorage"
        },
        filterRow: {
            visible: true
        },
        headerFilter: {
            visible: true
        },
        groupPanel: {
            visible: true
        },
        scrolling: {
            mode: "virtual"
        },
        height: 800,
        sortByGroupSummaryInfo: [{
            summaryItem: "count"
        }],
        summary: {
            totalItems: [
                {
                    column: "id",
                    displayFormat: '{0} Total Submissions',
                    summaryType: 'count',
                    showInGroupFooter: true,
                    showInColumn: 'FileNumber'
                },
            ],
            groupItems: [
                {
                    summaryType: "count",
                    displayFormat: '{0} Submissions',
                },

            ]
        },
        onToolbarPreparing: function (e) {
            var dataGrid = e.component;

            e.toolbarOptions.items.unshift(
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        text: "Collapse All",
                        width: 136,
                        onClick: function (e) {
                            var expanding = e.component.option("text") === "Expand All";
                            dataGrid.option("grouping.autoExpandAll", expanding);
                            e.component.option("text", expanding ? "Collapse All" : "Expand All");
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "refresh",
                        onClick: function () {
                            dataGrid.refresh();
                        }
                    }
                },
                {
                    location: "after",
                    widget: "dxButton",
                    options: {
                        icon: "fa fa-undo",
                        onClick: function () {
                            dataGrid.state({});
                        }
                    }
                },
            );
        }
    }).dxDataGrid("instance");

    function startWorkFlow(container, data) {
        window.$toggleWorkingButton(container);

        $.ajax({
            url: '/api/submission/start/' + id,
            type: 'POST'
        })
            .done(function () {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Created assignment');
            })
            .fail(function (err) {
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            })
            .always(function () {
                window.$toggleWorkingButton(btn, 'off');
            });

    }

    function cancelWorkFlow(container, data) {
        var id = data.id;

        window.$toggleWorkingButton(container);

        $.ajax({
            url: '/api/submission/cancel/' + id,
            type: 'POST'
        })
            .done(function () {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Cancelled assignments');
            })
            .fail(function (err) {
                window.$log.error('Something went wrong: ' + err.responseJSON.message);
            })
            .always(function () {
                window.$toggleWorkingButton(container, 'off');
            });
    }

    function reopenSubmission(container, data) {

        var url = '/reopenaudit/' + data.id;
        var title = 'Reopen Reason';
        var postUrl = '/api/submission/reopen/' + data.id;

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.BootstrapDialog.show({
                    size: window.BootstrapDialog.SIZE_WIDE,
                    draggable: true,
                    title: title,
                    message: $('<div></div>').load(url,
                        function (resp, status, xhr) {
                            if (status === 'error') {
                                window.$log.error('Error showing history');
                            }
                        }),
                    buttons: [
                        {
                            label: 'Close',
                            action: function (dialogRef) {
                                dialogRef.close();
                            }
                        },
                        {
                            label: 'Save',
                            cssClass: 'btn-primary',
                            action: function (dialogRef) {
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
                                        dialogRef.close();
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
        })

    }

    function waiverWorkFlow(container, data) {
        var title = 'Waiver Reason';
        var url = '/audit/' + data.id;
        var postUrl = '/api/submission/waiver/' + data.id;

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.BootstrapDialog.show({
                    size: window.BootstrapDialog.SIZE_WIDE,
                    draggable: true,
                    title: title,
                    message: $('<div></div>').load(url, function (resp, status, xhr) {
                        if (status === 'error') {
                            window.$log.error('Error showing history');
                        }
                    }),
                    buttons: [
                        {
                            label: 'Close',
                            action: function (dialogRef) {
                                dialogRef.close();
                            }
                        },
                        {
                            label: 'Save',
                            cssClass: 'btn-primary',
                            action: function (dialogRef) {
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
                                        dialogRef.close();
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
    }

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
                window.BootstrapDialog.show({
                    size: window.BootstrapDialog.SIZE_WIDE,
                    draggable: true,
                    title: title,
                    message: $('<div></div>').load(url, function (resp, status, xhr) {
                        if (status === 'error') {
                            window.$log.error('Error showing history');
                        }
                    }),
                    buttons: [
                        {
                            label: 'Close',
                            action: function (dialogRef) {
                                dialogRef.close();
                            }
                        },
                        {
                            label: 'Save',
                            cssClass: 'btn-primary',
                            action: function (dialogRef) {
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
                                        dialogRef.close();
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
        console.log('posturl');
        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.BootstrapDialog.show({
                    size: window.BootstrapDialog.SIZE_WIDE,
                    draggable: true,
                    title: title,
                    message: $('<div></div>').load(url, function (resp, status, xhr) {
                        if (status === 'error') {
                            window.$log.error('Error showing history');
                        }
                    }),
                    buttons: [
                        {
                            label: 'Close',
                            action: function (dialogRef) {
                                dialogRef.close();
                            }
                        },
                        {
                            label: 'Save',
                            cssClass: 'btn-primary',
                            action: function (dialogRef) {
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
                                        dialogRef.close();
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
        var title = 'Reassign Task';
        var url = $(this).attr('href') + '/' + id;
        $.ajax({
            url: url,
            type: 'POST',
            success: function (data) {
                window.BootstrapDialog.show({
                    size: window.BootstrapDialog.SIZE_WIDE,
                    draggable: true,
                    title: title,
                    message: $('<div></div>').load(url, function (resp, status, xhr) {
                        if (status === 'error') {
                            window.$log.error('Error showing history');
                        }
                    }),
                    buttons: [
                        {
                            label: 'Close',
                            action: function (dialogRef) {
                                dialogRef.close();
                            }
                        },
                        {
                            label: 'Save',
                            cssClass: 'btn-primary',
                            action: function (dialogRef) {
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
                                    dialogRef.close();
                                    window.$hideModalWorking();
                                });

                            }
                        }
                    ]
                });

            },
            error: function (err) {
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
        lnk += '<a class="btn btn-default btn-sm btn-grid" href="/report/' + dataYear + '/' + fileNumber + '">Review File</a>&nbsp;';
    }
    if (canCancel) {
        lnk += '<a href="/audit/' + submissionId + '" class="btn btn-default btn-sm btn-grid" data-cancel data-title="Reason for Reset" data-submission-id=' + submissionId +
            '><i class="fa fa-spinner fa-spin hidden"></i> Reset</a>&nbsp;';
    }
    if (canReopen) {
        lnk += '<a href="/reopenaudit/' +
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

