/* =========================================================
   RESUME AUTOFIT  (AUTO SHRINK + AUTO GROW)
   File: ~/js/resume-autofit.js

   Works for landscape (1123 x 794) and portrait (794 x 1123),
   and for every layout type:

   - FIXED-HEIGHT areas (sidebar, main column):
       too much data  -> content shrinks until it fits
       too little data -> content grows until it fills

   - CONTENT-SIZED areas (e.g. a footer dock whose height
     follows its content): never grown. They only shrink,
     and only when the fixed-height areas cannot fit
     otherwise.

   Scaling changes font size, padding, margins and gaps using
   real pixel values (no CSS zoom / transform), so print and
   html2canvas PDF export see the same result.

   Optional attributes on <div id="resumeSheet">:
     data-autofit="off"   -> disable autofit
     data-fit-min="0.5"   -> smallest scale (0.3 - 0.99)  default 0.6
     data-fit-max="1.6"   -> largest scale  (1.01 - 2.5)  default 1.5

   Public API:
     window.resumeAutofit.run()
     window.resumeAutofit.orientation()
========================================================= */

(function () {

    "use strict";

    var DEFAULT_MIN_SCALE = 0.6;
    var DEFAULT_MAX_SCALE = 1.5;
    var TOLERANCE = 2;
    var SEARCH_STEPS = 9;

    /* final scale is multiplied by this so PDF / print never
       overflows by a single line */
    var SAFETY_FACTOR = 0.99;

    /* scales tried for content-sized areas, largest first */
    var AUTO_STEPS = [1, 0.95, 0.9, 0.85, 0.8, 0.75, 0.7, 0.65, 0.6, 0.55, 0.5];

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

    var maxScale = parseFloat(sheet.getAttribute("data-fit-max"));

    if (!(maxScale > 1 && maxScale <= 2.5)) {
        maxScale = DEFAULT_MAX_SCALE;
    }


    /* Areas that are fitted. The header text may only shrink. */
    var TARGETS = [
        { selector: ".header-info", grow: 1 },
        { selector: ".left-column", grow: maxScale },
        { selector: ".right-column", grow: maxScale }
    ];


    /* -----------------------------------------------------
       ORIENTATION
    ----------------------------------------------------- */

    function orientation() {

        var style = window.getComputedStyle(sheet);

        var w = parseFloat(style.width) || sheet.offsetWidth;
        var h = parseFloat(style.height) || sheet.offsetHeight;

        return h > w ? "portrait" : "landscape";
    }

    function markOrientation() {
        sheet.setAttribute("data-orientation", orientation());
    }

    markOrientation();


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


    var columns = TARGETS
        .map(function (target) {

            var root = sheet.querySelector(target.selector);

            return root
                ? {
                    root: root,
                    grow: target.grow,
                    items: capture(root),
                    auto: false
                }
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
                    scale === 1
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
       IS THIS AREA CONTENT-SIZED?
       Scale it up as a test. If its own height changes, its
       height follows its content, so growing it is pointless
       (it would just get bigger and steal space).
    ----------------------------------------------------- */

    function isContentSized(column) {

        if (column.grow <= 1) {
            return false;
        }

        var root = column.root;

        applyScale(column, 1);

        var before = root.clientHeight;

        applyScale(column, column.grow);

        var after = root.clientHeight;

        applyScale(column, 1);

        return Math.abs(after - before) > TOLERANCE;
    }


    /* -----------------------------------------------------
       FIT ONE FIXED-HEIGHT AREA
       Finds the LARGEST scale (min..grow) that still fits.
       Returns true when the content fits.
    ----------------------------------------------------- */

    function fit(column) {

        var root = column.root;

        if (!root.clientHeight && !root.clientWidth) {
            return true;
        }

        var low = minScale;
        var high = column.grow;

        /* everything fits even at the largest allowed scale */
        applyScale(column, high);

        if (!overflows(root)) {
            return true;
        }

        /* does not fit even at the smallest scale */
        applyScale(column, low);

        if (overflows(root)) {
            return false;
        }

        /* binary search for the largest scale that fits */
        for (var i = 0; i < SEARCH_STEPS; i++) {

            var middle = (low + high) / 2;

            applyScale(column, middle);

            if (overflows(root)) {
                high = middle;
            } else {
                low = middle;
            }
        }

        applyScale(column, Math.max(minScale, low * SAFETY_FACTOR));

        return true;
    }


    /* -----------------------------------------------------
       RUN
    ----------------------------------------------------- */

    function run() {

        markOrientation();

        /* 1. reset everything to the original CSS sizes */
        columns.forEach(function (column) {
            applyScale(column, 1);
        });

        /* 2. find out which areas are content-sized */
        columns.forEach(function (column) {
            column.auto = isContentSized(column);
        });

        var autoColumns = columns.filter(function (c) { return c.auto; });
        var fixedColumns = columns.filter(function (c) { return !c.auto; });

        /* 3. fit the fixed-height areas. Content-sized areas stay
              at 100% unless the fixed areas cannot fit, then they
              are shrunk step by step to give them more room. */
        for (var s = 0; s < AUTO_STEPS.length; s++) {

            var scale = AUTO_STEPS[s];

            if (scale < minScale && s > 0) {
                break;
            }

            autoColumns.forEach(function (column) {
                applyScale(column, scale);
            });

            var allFit = true;

            fixedColumns.forEach(function (column) {

                if (!fit(column)) {
                    allFit = false;
                }
            });

            if (allFit || !autoColumns.length) {
                break;
            }
        }
    }

    var scheduled = false;

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
    window.addEventListener("afterprint", schedule);

    if (document.fonts && document.fonts.ready) {
        document.fonts.ready.then(schedule);
    }

    /* zoom breakpoints change on resize (width only) */
    var lastWidth = window.innerWidth;
    var resizeTimer = null;

    window.addEventListener("resize", function () {

        if (window.innerWidth === lastWidth) {
            return;
        }

        lastWidth = window.innerWidth;

        clearTimeout(resizeTimer);

        resizeTimer = setTimeout(schedule, 150);
    });

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

    window.resumeAutofit = {
        run: run,
        orientation: orientation
    };

})();