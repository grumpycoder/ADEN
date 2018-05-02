// reports.js

(function () {
    console.log('reports ready');
});

function createReportDocumentLinks(container, options) {
    var documents = options.data.documents;
    console.log('repdoclink', documents);
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

$(document).on('click', '[data-document-viewer]', function (e) {
    console.log('doc link click');

    var $currentTarget = $(e.currentTarget);
    var targetModal = $currentTarget.data('target');
    var $modal = $(targetModal);

    var remoteContent = $currentTarget.data('url');
    var title = $currentTarget.data('doc-title');
    var id = $currentTarget.data('doc-id');
    console.log('url', remoteContent);

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


