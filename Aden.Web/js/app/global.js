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
