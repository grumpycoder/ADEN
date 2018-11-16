//fileSpecification.module.js
$(function () {
    console.log('file specification ready');

    var $grid = $('#grid').dxDataGrid('instance');

    $(document).on('click', '[data-edit]', function (e) {
        e.preventDefault();
        var url = $(this).attr("href");
        var title = 'Edit Specification';

        $.ajax({
            url: url,
            type: 'GET',
            success: function (data) {
                window.BootstrapDialog.show({
                    size: window.BootstrapDialog.SIZE_WIDE,
                    closeByBackdrop: false,
                    closeByKeyboard: false,
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
                                var form = $('form');
                                var url = '/api/filespecification/' + $('#Id').val();
                                window.$showModalWorking();

                                $.ajax({
                                    type: "PUT",
                                    url: url,
                                    data: $('form').serialize(),
                                    success: function (msg) {
                                        dialogRef.close();
                                        $('#grid').dxDataGrid('instance').refresh().done(function (e) { });
                                        window.toastr.success('Saved ');
                                    },
                                    error: function (err) {
                                        var validationErrors = JSON.parse(err.responseText);
                                        $.each(validationErrors.modelState, function (i, ival) {
                                            window.remoteErrors(form, i, ival);
                                        });

                                    },
                                    complete: function () {
                                        //console.log('complete');
                                        dialogRef.close();
                                    }

                                });
                            }
                        }
                    ]
                });
            },
            error: function (err) {
                window.$log.error('Error showing edit form');
            }
        });

    });

    $(document).on('click', '[data-retire]', function (e) {
        e.preventDefault();
        var btn = $(this);
        var id = btn.data('filespec-id');

        window.$toggleWorkingButton(btn);

        $.ajax({
            url: '/api/filespecification/retire/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh();
                window.$log.success('Retired ' + data.fileNumber + ' - ' + data.fileName);
            },
            error: function (err) {
                console.log('err', err);
                window.$log.error('Something went wrong: ' + err.message);
            }
        }).always(function () {
            window.$toggleWorkingButton(btn);
        });

    });

    $(document).on('click', '[data-activate]', function (e) {
        e.preventDefault();
        var btn = $(this);
        window.$toggleWorkingButton(btn);
        var id = btn.data('filespec-id');
        $.ajax({
            url: '/api/filespecification/activate/' + id,
            type: 'POST',
            success: function (data) {
                $grid.refresh().done(function (e) { console.log('done', e) });
                window.$log.success('Activated ' + data.fileNumber + ' - ' + data.fileName);
            },
            error: function (err) {
                //console.log('err', err);
                window.$log.error('Something went wrong: ' + err.message);
            }
        }).always(function () {
            window.$toggleWorkingButton(btn);
        });

    });

    $(document).on('click', '[data-modify-group]', function (e) {
        e.preventDefault();

        var btn = $(this);
        var id = btn.data('id');
        var groupName = btn.data('group-name');
        var url = '/editgroupmembership/' + id + '/' + groupName;
        var title = 'Edit Membership';

        window.BootstrapDialog.show({
            size: window.BootstrapDialog.SIZE_NORMAL,
            closable: true,
            closeByBackdrop: false,
            closeByKeyboard: false,
            title: title,
            message: $('<div></div>').load(url, function (resp, status, xhr) {
                if (status === 'error') {
                    console.log('status', status);
                    console.log('resp', resp);
                    window.$log.error('Error showing group membership');
                }
            }),
            buttons: [
                {
                    label: 'Close',
                    action: function (dialogRef) {
                        dialogRef.close();
                    }
                }
            ]
        });

    });

    $(document).on('click', '#btnAddGroupMember',
        function (e) {
            e.preventDefault();
            var url = '/api/filespecification/groupmembers/';
            $.ajax({
                type: "PUT",
                url: url,
                data: $('#groupForm').serialize(),
                success: function (data) {
                    console.log('success', data);
                    window.$log.success('Request has been sent to HelpDesk. Check back in 24 hours. ');
                },
                error: function (err) {
                    console.log('error', err);
                }
            }).always(function () {

            });
        });

    $(document).on('click', '#btnRemoveGroupMember',
        function (e) {
            e.preventDefault();
            var btn = $(this);
            var url = '/api/filespecification/groupmembers/';
            var email = btn.data('email');
            $('#memberForm #email').val(email); 
            
            $.ajax({
                type: "DELETE",
                url: url,
                data: $('#memberForm').serialize(),
                processData: false,
                success: function (data) {
                    console.log('success', data);
                    window.$log.success('Request has been sent to HelpDesk. Check back in 24 hours. ');
                },
                error: function (err) {
                    console.log('error', err);
                    window.$log.success('Error in request. ' + err);
                }
            }).always(function () {

            });
        });

});

function createFileSpecificationGridActionButtons(container, options) {
    var lnk = '';
    var canRetire = options.data.canRetire;
    var canActivate = options.data.canActivate;
    var fileSpecId = options.data.id;

    if (canRetire) {
        lnk += '<button class="btn btn-default btn-sm btn-grid" data-retire data-filespec-id=' + fileSpecId + '><i class="fa fa-spinner fa-spin hidden"></i> Retire</button>';
    }
    if (canActivate) {
        lnk += '<button class="btn btn-default btn-sm btn-grid" data-activate data-filespec-id=' + fileSpecId + '><i class="fa fa-spinner fa-spin hidden"></i> Activate</button>';
    }
    lnk += '<button class="btn btn-default btn-sm btn-grid" href="/EditFileSpecification/' + fileSpecId + '" data-edit data-id="' + fileSpecId + '">Edit</button>';
    container.append(lnk);
}



