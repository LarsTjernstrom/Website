(function () {
    if (Object.observe) {
        return;
    }

    var script = document._currentScript || document.currentScript;

    script.parentNode.innerHTML = ['<div id="observe-required">',
        '<h3>This page requires support for <code>Object.observe</code> in your web browser</h3>',
        '<p>It is a new JavaScript feature that is currently implemented in Google Chrome and Opera.</p>',
        '<p>We are working on extending support to all modern web browsers.</p>',
    '</div>'].join("");
})();