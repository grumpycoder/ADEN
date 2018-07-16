window.showBSModal = function self(options) {

    var options = $.extend({
        title: '',
        body: '',
        remote: false,
        backdrop: 'static',
        size: false,
        onShow: false,
        onHide: false,
        actions: false
    }, options);

    self.onShow = typeof options.onShow == 'function' ? options.onShow : function () { };
    self.onHide = typeof options.onHide == 'function' ? options.onHide : function () { };

    if (self.$modal == undefined) {
        self.$modal = $('<div class="modal fade"><div class="modal-dialog"><div class="modal-content"></div></div></div>').appendTo('body');
        self.$modal.on('shown.bs.modal', function (e) {
            self.onShow.call(this, e);
        });
        self.$modal.on('hidden.bs.modal', function (e) {
            self.onHide.call(this, e);
        });
    }

    var modalClass = {
        small: "modal-sm",
        large: "modal-lg"
    };

    self.$modal.data('bs.modal', false);
    self.$modal.find('.modal-dialog').removeClass().addClass('modal-dialog ' + (modalClass[options.size] || ''));
    self.$modal.find('.modal-content').html('<div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button><h4 class="modal-title">${title}</h4></div><div class="modal-body">${body}</div><div class="modal-footer"></div>'.replace('${title}', options.title).replace('${body}', options.body));

    var footer = self.$modal.find('.modal-footer');
    if (Object.prototype.toString.call(options.actions) === "[object Array]") {
        for (var i = 0, l = options.actions.length; i < l; i++) {
            options.actions[i].onClick = typeof options.actions[i].onClick == 'function' ? options.actions[i].onClick : function () { };
            $('<button type="button" class="btn ' + (options.actions[i].cssClass || '') + '">' + (options.actions[i].label || '{Label Missing!}') + '</button>').appendTo(footer).on('click', options.actions[i].onClick);
        }
    } else {
        $('<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>').appendTo(footer);
    }

    self.$modal.modal(options);
}
// Source: http://pixelscommander.com/en/javascript/javascript-file-download-ignore-content-type/
window.downloadFile = function (sUrl) {

    //iOS devices do not support downloading. We have to inform user about this.
    if (/(iP)/g.test(navigator.userAgent)) {
        //alert('Your device does not support files downloading. Please try again in desktop browser.');
        window.open(sUrl, '_blank');
        return false;
    }

    //If in Chrome or Safari - download via virtual link click
    if (window.downloadFile.isChrome || window.downloadFile.isSafari) {
        //Creating new link node.
        var link = document.createElement('a');
        link.href = sUrl;
        link.setAttribute('target', '_blank');

        if (link.download !== undefined) {
            //Set HTML5 download attribute. This will prevent file from opening if supported.
            var fileName = sUrl.substring(sUrl.lastIndexOf('/') + 1, sUrl.length);
            link.download = fileName;
        }

        //Dispatching click event.
        if (document.createEvent) {
            var e = document.createEvent('MouseEvents');
            e.initEvent('click', true, true);
            link.dispatchEvent(e);
            return true;
        }
    }

    // Force file download (whether supported by server).
    if (sUrl.indexOf('?') === -1) {
        sUrl += '?download';
    }

    window.open(sUrl, '_blank');
    return true;
}

window.downloadFile.isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
window.downloadFile.isSafari = navigator.userAgent.toLowerCase().indexOf('safari') > -1;

// Global Functions
window.addEventListener('load',
    function (e) {
        $('.loader').hide();
        return e;
    });

window.addEventListener('beforeunload',
    function (e) {
        $('.page').hide();
        $('.loader').show();
        return e;
    });

window.onBeforeSend = function () {
}

window.onloadstart = function () {
}

function remoteErrors(jForm, name, errors) {

    if (name === '$id') return; 
    function innerServerErrors(name, elements) {
        var toApply = function () {
            for (var i = 0; i < elements.length; i++) {
                var currElement = elements[i];
                var currDom = $('#' + name.split('.')[1]);
                if (currDom.length === 0) continue;
                var currForm = currDom.parents('form').first();
                if (currForm.length === 0) continue;

                if (!currDom.hasClass('input-validation-error'))
                    currDom.addClass('input-validation-error');
                var currDisplay = $(currForm).find('[data-valmsg-for=\'' + name.split('.')[1] + "']");
                if (currDisplay.length > 0) {
                    currDisplay.removeClass('field-validation-valid').addClass('field-validation-error');
                    if (currDisplay.attr("data-valmsg-replace") !== false) {
                        currDisplay.empty();
                        currDisplay.text(currElement);
                    }
                }
            }
        };
        setTimeout(toApply, 0);
    }
    
    jForm.find('.input-validation-error').removeClass('input-validation-error');
    jForm.find('.field-validation-error').removeClass('field-validation-error').addClass('field-validation-valid');
    var container = jForm.find('[data-valmsg-summary=true]');
    var list = container.find('ul');
    list.empty();
    if (errors && errors.length > 0) {
        $.each(errors, function (i, ival) {
            $("<li />").html(ival).appendTo(list);
        });
        container.addClass('validation-summary-errors').removeClass('validation-summary-valid');
        innerServerErrors(name, errors);
        setTimeout(function () { jForm.find('span.input-validation-error[data-element-type]').removeClass('input-validation-error') }, 0);
    }
    else {
        container.addClass('validation-summary-valid').removeClass('validation-summary-errors');
    }
}

function clearErrors(jForm) {
    remoteErrors(jForm, []);
}