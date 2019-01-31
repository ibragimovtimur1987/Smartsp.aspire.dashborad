/* Copyright (C) YOOtheme GmbH, YOOtheme Proprietary Use License (http://www.yootheme.com/license) */

jQuery(function ($) {

    var config = $('html').data('config') || {};

    // Social buttons
    //$('article[data-permalink]').socialButtons(config);

    // fit footer
    (function (main, meta, fn) {

        if (!main.length) return;

        fn = function () {

            main.css('min-height', '');

            meta = document.body.getBoundingClientRect();

            if (meta.height <= window.innerHeight) {
                main.css('min-height', (main.outerHeight() + (window.innerHeight - meta.height)) + 'px');
            }

            return fn;
        };

        UIkit.$win.on('load resize', fn());

    })($('#tm-main'));

});