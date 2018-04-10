String.prototype.isEmpty = function() {
    return (this.length === 0 || !this.trim());
};

var POLYPAINTPRO = POLYPAINTPRO || {};
(function($) {

    // USE STRICT
    "use strict";

    const POLLING_INTERVAL = 15000;
    var isPolling = false;
    var pollingInterval;

    var $portfolio = $("#portfolio");

    POLYPAINTPRO.initialize = {
        init: function () {
            POLYPAINTPRO.portfolio.initialize();
        }
    };

    POLYPAINTPRO.documentOnReady = {
        init: function () {
            POLYPAINTPRO.initialize.init();
        }
    };

    POLYPAINTPRO.portfolio = {
        initialize: function() {
            //Nothing yet
        },

        gridInitialize: function() {
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
        },

        getDrawings: function(endpoint) {
            $.get(endpoint + "&page=1").done(function(data) {
                data.docs.forEach(function(doc) {
                    POLYPAINTPRO.portfolio.appendDrawing(doc)
                });

                for (var i = 2; i <= data.pages; i++) {
                    $.get(endpoint + "&page=" + i).done(function(data) {
                        data.docs.forEach(function(doc) {
                            POLYPAINTPRO.portfolio.appendDrawing(doc)
                        });
                    });
                }

                if (!isPolling && data.total > 0) {
                    POLYPAINTPRO.portfolio.startRefreshPolling();
                }
            });
        },

        startRefreshPolling: function() {
            pollingInterval = setInterval(function() {
                $(".portfolio-item").each(function(index, item) {
                    $.get(POLYPAINTPRO.portfolio.buildThumbnailEndpoint($(item).data("drawing-id"))).done(function(thumb) {
                        var builtThumb = POLYPAINTPRO.portfolio.buildBase64String(thumb.thumbnail);
                        if (builtThumb !== $(item).find(".thumbnail-src")) {
                            $(item).find(".thumbnail-src").attr("src", builtThumb);
                            $(item).find(".thumbnail-href").attr("href", builtThumb);
                            POLYPAINTPRO.portfolio.gridInitialize();
                        }
                    });
                });
            }, POLLING_INTERVAL);
        },

        appendDrawing: function(doc) {
            $.get(POLYPAINTPRO.portfolio.buildThumbnailEndpoint(doc._id)).done(function(thumb) {
                $portfolio.append(
                    "<article class=\"portfolio-item pf-" + doc.mode + "\" data-drawing-id=\"" + doc._id + "\">\n" +
                    "  <div class=\"portfolio-image\" style=\"border: 1px solid black;\">\n" +
                    "    <img src=\"" + POLYPAINTPRO.portfolio.buildBase64String(thumb.thumbnail) + "\" alt=\"Open Imagination\" class=\"thumbnail-src\">\n" +
                    "  </div>\n" +
                    "  <div class=\"portfolio-desc\">\n" +
                    "    <h3><a href=\"portfolio-single.html\">" + doc.name + "</a></h3>\n" +
                    "    <span>Mode " + doc.mode + "</span>\n" +
                    "  </div>\n" +
                    "</article>"
                );

                POLYPAINTPRO.portfolio.gridInitialize();
            });
        },

        buildBase64String: function (base64) {
            var core = "data:image/jpeg;base64,";
            if (!base64) {
                return core + "R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7"; //transparent
            }
            return core + base64;
        },

        buildThumbnailEndpoint: function(drawingId) {
            return POLYPAINTPRO.login.getServer() + "/drawings/" + drawingId + "/thumbnail";
        }
    };

    POLYPAINTPRO.login = {
        isLoggedIn: function() {
            return !!localStorage.getItem("user");
        },

        saveServer: function(serverAddress) {
            localStorage.setItem("serverIp", serverAddress);
        },

        saveUser: function(userId, username) {
            localStorage.setItem("user", JSON.stringify({id: userId, username: username}));
        },

        getServer: function() {
            return localStorage.getItem("serverIp");
        },

        getUser: function() {
            return JSON.parse(localStorage.getItem("user"));
        }
    };

    $(document).ready(POLYPAINTPRO.documentOnReady.init);
})(jQuery);
