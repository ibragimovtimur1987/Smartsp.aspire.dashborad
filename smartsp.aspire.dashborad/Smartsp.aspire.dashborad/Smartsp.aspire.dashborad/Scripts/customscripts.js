(function ($) {

    $(document).ready(function () {
        $('.navbar a.dropdown-toggle').on('click', function (e) {
            var $el = $(this);
            var $parent = $(this).offsetParent(".dropdown-menu");
            $(this).parent("li").toggleClass('open');

            if (!$parent.parent().hasClass('nav')) {
                $el.next().css({ "top": $el[0].offsetTop, "left": $parent.outerWidth() - 4 });
            }

            $('.nav li.open').not($(this).parents("li")).removeClass("open");

            return false;
        });
        // Accordion transitions
        if (/\/activity\/journal\/?/i.test(location.pathname)) {
            var accordion = UIkit.accordion($('.uk-accordion'), { collapse: false, showfirst: false });
            var current;

            switch (location.hash) {
                case '#about': current = 0; break;
                case '#subscribe': current = 5; break;
            }

            if (current !== undefined) {
                var el = accordion.toggle[current];

                window.scroll(0, findPos(el));
                el.click();
            }
        }

        $('a#for-authors').on('click', function (e) {
            e.preventDefault();

            var accordion = UIkit.accordion($('.uk-accordion'), { collapse: false, showfirst: false });
            var el = accordion.toggle[3];

            window.scroll(0, findPos(el));
            el.click();
        });

        // news alias fix on homepage
        $('#home-news a').each(function () {
            $(this).attr('href', $(this).attr('href').replace(/^\/(ru|en)/g, '/$1/news'));
        });

        // news home clip - title
        $('#home-news .uk-panel-title').dotdotdot({
            ellipsis: ' ...',
            height: 140,
            watch: "window.resize"
        });

        // staff description clip
        $('#w-staff .staff-text').dotdotdot({
            ellipsis: ' ...',
            height: 180,
            watch: "window.resize"
        });

        // news page clip - title
        $('.w-news .uk-panel-title').dotdotdot({
            ellipsis: ' ...',
            height: 50,
            watch: "window.resize"
        });

        // news page clip - short text
        $('.w-news .uk-panel-title + div').dotdotdot({
            ellipsis: ' ...',
            height: 90,
            watch: "window.resize"
        });

        // widget - accordion
        $('#w-accordion .uk-accordion-title').on('click', function () {
            $(window).resize();

            $(this).toggleClass('active');

            $('#w-accordion .uk-accordion-content').addClass('hidden');
            $(this).parent().find('.accordion-content').toggleClass('hidden');
        });

        /* yandex map ru */
        if ($('#map').length) {
            ymaps.ready(init);

            function init() {
                var map = new ymaps.Map("map", {
                    center: [55.766577, 37.604839],
                    zoom: 16
                }, {
                    searchControlProvider: 'yandex#search'
                });

                ymaps.route([
                  'РњРѕСЃРєРІР°, РўРІРµСЂСЃРєР°СЏ СѓР»РёС†Р°, 16СЃРѕРѕСЂ1',
                  {
                      point: 'РњРѕСЃРєРІР°, РќР°СЃС‚Р°СЃСЊРёРЅСЃРєРёР№ РїРµСЂРµСѓР»РѕРє, 5c2',
                      type: 'viaPoint'
                  },
                  'РњРѕСЃРєРІР°, РќР°СЃС‚Р°СЃСЊРёРЅСЃРєРёР№ РїРµСЂРµСѓР»РѕРє, Рґ. 3, СЃС‚СЂРѕРµРЅРёРµ 2'
                ]).then(function (route) {

                    route.getPaths().options.set({
                        strokeColor: '007EC3',
                        opacity: 0.9
                    });

                    map.geoObjects.add(route);
                    // Р—Р°РґР°РґРёРј СЃРѕРґРµСЂР¶Р°РЅРёРµ РёРєРѕРЅРѕРє РЅР°С‡Р°Р»СЊРЅРѕР№ Рё РєРѕРЅРµС‡РЅРѕР№ С‚РѕС‡РєР°Рј РјР°СЂС€СЂСѓС‚Р°.
                    // РЎ РїРѕРјРѕС‰СЊСЋ РјРµС‚РѕРґР° getWayPoints() РїРѕР»СѓС‡Р°РµРј РјР°СЃСЃРёРІ С‚РѕС‡РµРє РјР°СЂС€СЂСѓС‚Р°.
                    // РњР°СЃСЃРёРІ С‚СЂР°РЅР·РёС‚РЅС‹С… С‚РѕС‡РµРє РјР°СЂС€СЂСѓС‚Р° РјРѕР¶РЅРѕ РїРѕР»СѓС‡РёС‚СЊ СЃ РїРѕРјРѕС‰СЊСЋ РјРµС‚РѕРґР° getViaPoints.
                    var points = route.getWayPoints(),
                    lastPoint = points.getLength() - 1;
                    // Р—Р°РґР°РµРј СЃС‚РёР»СЊ РјРµС‚РєРё - РёРєРѕРЅРєРё Р±СѓРґСѓС‚ Р·РµР»РµРЅРѕРіРѕ С†РІРµС‚Р° Рё
                    // РёС… РёР·РѕР±СЂР°Р¶РµРЅРёСЏ Р±СѓРґСѓС‚ СЂР°СЃС‚СЏРіРёРІР°С‚СЊСЃСЏ РїРѕРґ РєРѕРЅС‚РµРЅС‚.
                    points.options.set('preset', 'islands#blueStretchyIcon');

                    // Р—Р°РґР°РµРј РєРѕРЅС‚РµРЅС‚ РјРµС‚РѕРє РІ РЅР°С‡Р°Р»СЊРЅРѕР№ Рё РєРѕРЅРµС‡РЅРѕР№ С‚РѕС‡РєР°С….
                    points.get(0).properties.set('iconContent', 'РџСѓС€РєРёРЅСЃРєР°СЏ');
                    points.get(0).properties.set('balloonContent', 'РЎС‚Р°РЅС†РёСЏ РјРµС‚СЂРѕ РџСѓС€РєРёРЅСЃРєР°СЏ');

                    points.get(lastPoint).properties.set('iconContent', 'РќРР¤Р');
                    points.get(lastPoint).properties.set('balloonContent', 'РќР°СѓС‡РЅРѕ-РёСЃСЃР»РµРґРѕРІР°С‚РµР»СЊСЃРєРёР№<br>С„РёРЅР°РЅСЃРѕРІС‹Р№ РёРЅСЃС‚РёС‚СѓС‚');

                    var viaPoints = route.getViaPoints();

                    route.getViaPoints().each(function (viaPoint, i) {
                        viaPoint.options.set('visible', false);
                    })

                    console.log(viaPoints);

                }, function (error) {
                    alert('РћС€РёР±РєР°: ' + error.message);
                });
            }

        }

    });

    function findPos(obj) {
        var curtop = 0;
        if (obj.offsetParent) {
            do {
                curtop += obj.offsetTop;
            } while (obj = obj.offsetParent);
            return curtop;
        }
    }

})(jQuery);