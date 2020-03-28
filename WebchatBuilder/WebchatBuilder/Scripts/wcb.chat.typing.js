(function ($) {
    $.fn.wcbtyping = function (options) {
        return this.each(function (i, elem) {
            listenToTyping(elem, options);
        });
    };
    function listenToTyping(elem, options) {
        var settings = $.extend({
            start: null,
            stop: null,
            delay: 400
        }, options);
        var $elem = $(elem),
            wcbtyping = false,
            delayedCallback;
        function startTyping(event) {
            if (!wcbtyping) {
                wcbtyping = true;
                if (settings.start) {
                    settings.start(event, $elem);
                }
            }
        }
        function stopTyping(event, delay) {
            if (wcbtyping) {
                clearTimeout(delayedCallback);
                delayedCallback = setTimeout(function () {
                    wcbtyping = false;
                    if (settings.stop) {
                        settings.stop(event, $elem);
                    }
                }, delay >= 0 ? delay : settings.delay);
            }
        }

        $elem.keypress(startTyping);
        $elem.keydown(function (event) {
            if (event.keyCode === 8 || event.keyCode === 46) {
                startTyping(event);
            }
        });
        $elem.keyup(stopTyping);
        $elem.blur(function (event) {
            stopTyping(event, 0);
        });
    }
})(jQuery);
