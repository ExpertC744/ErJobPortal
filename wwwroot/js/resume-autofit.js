/* =========================================================
   RESUME AUTOFIT
   File: ~/js/resume-autofit.js

   The resume sheet is a fixed A4 landscape page (1123 x 794).
   When a column has more content than fits, this script
   shrinks the content inside that column (font size, padding,
   margins, gaps) just enough to fit, instead of letting
   overflow: hidden cut it off.

   - Works for every ViewProfile template (no CSS changes).
   - Uses real pixel values (no CSS zoom / transform), so
     print and html2canvas PDF export see the same result.
   - Only shrinks, never enlarges. Smallest scale is 60%.

   Optional attributes on <div id="resumeSheet">:
     data-autofit="off"     -> disable autofit
     data-fit-min="0.5"     -> smallest allowed scale (0.3 - 0.99)
========================================================= */

(function () {

    "use strict";

    var DEFAULT_MIN_SCALE = 0.6;
    var TOLERANCE = 2;
    var SEARCH_STEPS = 7;

    var PROPS = [
        "fontSize",
        "paddingTop",
        "paddingRight",
        "paddingBottom",
        "paddingLeft",
        "marginTop",
        "marginBottom",
        "rowGap",
        "columnGap"
    ];

    var sheet = document.getElementById("resumeSheet");

    if (!sheet || sheet.getAttribute("data-autofit") === "off") {
        return;
    }

    var minScale = parseFloat(sheet.getAttribute("data-fit-min"));

    if (!(minScale > 0.3 && minScale < 1)) {
        minScale = DEFAULT_MIN_SCALE;
    }


    /* -----------------------------------------------------
       READ ORIGINAL SIZES (before anything is scaled)
    ----------------------------------------------------- */

    function capture(root) {

        var items = [];
        var nodes = root.querySelectorAll("*");

        for (var i = 0; i < nodes.length; i++) {

            var element = nodes[i];
            var style = window.getComputedStyle(element);
            var base = {};
            var found = false;

            for (var p = 0; p < PROPS.length; p++) {

                var value = parseFloat(style[PROPS[p]]);

                if (!isNaN(value) && value > 0) {
                    base[PROPS[p]] = value;
                    found = true;
                }
            }

            if (found) {
                items.push({ element: element, base: base });
            }
        }

        return items;
    }


    var columns = [".left-column", ".right-column"]
        .map(function (selector) {

            var root = sheet.querySelector(selector);

            return root
                ? { root: root, items: capture(root) }
                : null;
        })
        .filter(Boolean);


    /* -----------------------------------------------------
       APPLY SCALE
    ----------------------------------------------------- */

    function applyScale(column, scale) {

        column.items.forEach(function (item) {

            for (var name in item.base) {

                item.element.style[name] =
                    scale >= 1
                        ? ""
                        : (item.base[name] * scale).toFixed(2) + "px";
            }
        });
    }


    function overflows(root) {

        return (
            root.scrollHeight - root.clientHeight > TOLERANCE ||
            root.scrollWidth - root.clientWidth > TOLERANCE
        );
    }


    /* -----------------------------------------------------
       FIT ONE COLUMN
    ----------------------------------------------------- */

    function fit(column) {

        var root = column.root;

        if (!root.clientHeight && !root.clientWidth) {
            return;
        }

        applyScale(column, 1);

        if (!overflows(root)) {
            return;
        }

        applyScale(column, minScale);

        if (overflows(root)) {
            return;
        }

        var low = minScale;
        var high = 1;

        for (var i = 0; i < SEARCH_STEPS; i++) {

            var middle = (low + high) / 2;

            applyScale(column, middle);

            if (overflows(root)) {
                high = middle;
            } else {
                low = middle;
            }
        }

        applyScale(column, low);
    }


    var scheduled = false;

    function run() {

        columns.forEach(fit);
    }

    function schedule() {

        if (scheduled) {
            return;
        }

        scheduled = true;

        window.requestAnimationFrame(function () {

            scheduled = false;
            run();
        });
    }


    /* -----------------------------------------------------
       WHEN TO RUN
    ----------------------------------------------------- */

    run();

    window.addEventListener("load", schedule);
    window.addEventListener("beforeprint", run);

    if (document.fonts && document.fonts.ready) {
        document.fonts.ready.then(schedule);
    }

    /* the address text is filled in later by the location lookup */
    if (window.MutationObserver) {

        var observer = new MutationObserver(schedule);

        ["resumeAddress", "resumeAddressLeft"].forEach(function (id) {

            var element = document.getElementById(id);

            if (element) {
                observer.observe(element, {
                    childList: true,
                    characterData: true,
                    subtree: true
                });
            }
        });
    }

    window.resumeAutofit = { run: run };

})();
