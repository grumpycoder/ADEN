﻿// reports.module.js

(function () {
    console.log('reports ready');
});

function createReportDocumentLinks(container, options) {
    var documents = options.data.documents;
    var lnk = '<span><ul class="list-unstyled">';
    documents.forEach(function (doc) {
        lnk += '<li class="doclink">' +
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

    //SETUP LINK CLICK HANDLER
    $('li[class="doclink"] > a').each(function (e) {
        var id = $(this).data('doc-id');

        $('[data-doc-id=' + id + ']').on('click', function (e) {
            var $currentTarget = $(this);
            var title = $currentTarget.data('title');
            var modalUrl = $currentTarget.data('url');

            $.ajax({
                url: modalUrl,
                type: 'POST',
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
                                    //console.log('e', e);
                                    $(e.target).parents('.modal').modal('hide');
                                    $('body').removeClass('modal-open');
                                    //modal-open class is added on body so it has to be removed

                                    $('.modal-backdrop').remove();
                                    //need to remove div with modal-backdrop class
                                }
                            },
                            {
                                label: 'Download',
                                cssClass: 'btn-primary',
                                onClick: function (e) {
                                    var downloadUrl = '/download/' + id;
                                    window.downloadFile(downloadUrl);
                                    window.$log.info('Your file is being downloaded');
                                    $('.modalContainer').html('');
                                    $('.modal').modal('hide');

                                    $(e.target).parents('.modal').modal('hide');
                                    $('body').removeClass('modal-open');
                                    //modal-open class is added on body so it has to be removed

                                    $('.modal-backdrop').remove();
                                    //need to remove div with modal-backdrop class
                                }
                            }
                        ]
                    });
                },
                error: function (err) {
                    //console.log('err', err);
                    window.$log.error('Error showing history');
                }
            });

        });
    });
}
