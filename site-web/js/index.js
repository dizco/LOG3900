$(document).ready(function() {
    if (!localStorage.getItem("userId")) { //User is not logged in, redirect to login
        window.location.replace('./login.html');
    }

    const POLLING_INTERVAL = 15000;

    var polling = false;
    var pollingInterval;

    var $portfolio = $("#portfolio");

    var listEndpoint = localStorage.getItem("serverIp") + "/drawings?visibility=public";

    $.get(listEndpoint + "&page=1").done(function(data) {
        data.docs.forEach(function(doc) {
            appendDrawing(doc)
        });

        for (var i = 2; i <= data.pages; i++) {
            $.get(listEndpoint + "&page=" + i).done(function(data) {
                data.docs.forEach(function(doc) {
                    appendDrawing(doc)
                });
            });
        }

        if (!polling && data.total > 0) {
            startRefreshPolling();
        }
    });

    function startRefreshPolling() {
        pollingInterval = setInterval(function() {
            $(".portfolio-item").each(function(index, item) {
                $.get(buildThumbnailEndpoint($(item).data("drawing-id"))).done(function(thumb) {
                    var builtThumb = buildBase64String(thumb.thumbnail);
                    if (builtThumb !== $(item).find(".thumbnail-src")) {
                        $(item).find(".thumbnail-src").attr("src", builtThumb);
                        $(item).find(".thumbnail-href").attr("href", builtThumb);
                        customGridInitialize();
                    }
                });
            });
        }, POLLING_INTERVAL);
    }

    function buildThumbnailEndpoint(drawingId) {
        return localStorage.getItem("serverIp") + "/drawings/" + drawingId + "/thumbnail";
    }

    function buildBase64String(base64) {
        var core = "data:image/jpeg;base64,";
        if (!base64) {
            return core + "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"; //transparent
        }
        return core + base64;
    }

    function appendDrawing(doc) {
        $.get(buildThumbnailEndpoint(doc._id)).done(function(thumb) {
            $portfolio.append(
                "<article class=\"portfolio-item pf-" + doc.mode + "\" data-drawing-id=\"" + doc._id + "\">\n" +
                "  <div class=\"portfolio-image\" style=\"border: 1px solid black;\">\n" +
                "    <img src=\"" + buildBase64String(thumb.thumbnail) + "\" alt=\"Open Imagination\" class=\"thumbnail-src\">\n" +
                "  </div>\n" +
                "  <div class=\"portfolio-desc\">\n" +
                "    <h3><a href=\"portfolio-single.html\">" + doc.name + "</a></h3>\n" +
                "    <span>Mode " + doc.mode + "</span>\n" +
                "  </div>\n" +
                "</article>"
            );

            customGridInitialize();
        });
    }

    function customGridInitialize() {
        //Couldn't manage to call the SEMICOLON.portfolio.gridInit() method so copy-pasted it here
        var $container = $('.grid-container');
        $container.each( function(){
            var element = $(this),
                elementTransition = element.attr('data-transition'),
                elementLayoutMode = element.attr('data-layout'),
                elementStagger = element.attr('data-stagger'),
                elementOriginLeft = true;

            if( !elementTransition ) { elementTransition = '0.65s'; }
            if( !elementLayoutMode ) { elementLayoutMode = 'masonry'; }
            if( !elementStagger ) { elementStagger = 0; }

            setTimeout( function(){
                if( element.hasClass('portfolio') ){
                    element.isotope({
                        layoutMode: elementLayoutMode,
                        isOriginLeft: elementOriginLeft,
                        transitionDuration: elementTransition,
                        stagger: Number( elementStagger ),
                        masonry: {
                            columnWidth: element.find('.portfolio-item:not(.wide)')[0]
                        }
                    });
                } else {
                    element.isotope({
                        layoutMode: elementLayoutMode,
                        isOriginLeft: elementOriginLeft,
                        transitionDuration: elementTransition
                    });
                }
            }, 300);
        });
    }
});
