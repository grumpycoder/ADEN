// reports.module.js

(function () {
    console.log('reports ready');
});

$('a[data-document-viewer]').each(function (e) {
    var id = $(this).data('doc-id');

    $('[data-doc-id=' + id + ']').on('click', function (e) {
        var $currentTarget = $(this);
        var title = $currentTarget.data('title');
        var url = $currentTarget.data('url');

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
                            label: 'Download',
                            cssClass: 'btn-primary', 
                            action: function (dialogRef) {
                                var downloadUrl = '/download/' + id;
                                window.downloadFile(downloadUrl);
                                window.$log.info('Your file is being downloaded');
                                dialogRef.close();
                            }
                        }
                    ]
                });
            },
            error: function (err) {
                window.$log.error('Error showing file contents');
            }
        });

    });
});

function createReportDocumentLinks(container, options) {
    var documents = options.data.documents;
    var lnk = '<span><ul class="list-unstyled">';
    documents.forEach(function (doc) {
        lnk += '<li class="doc-link">' +
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
            'data-original-title="' + doc.filename + '">v.' + doc.version + ' - ' + doc.filename +
            '</a>' +
            '</li>';
    });
    lnk += '</ul></span>';
    container.append(lnk);
}
