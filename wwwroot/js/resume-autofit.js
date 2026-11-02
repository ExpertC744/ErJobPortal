/* =========================================================
   RESUME AUTOFIT  (UNIFORM SCALE + FILL REMAINING SPACE)
   File: ~/js/resume-autofit.js

   - Left and right columns always share ONE scale, so the
     font size is uniform across every section.
   - Scale = largest value at which nothing overflows
     (columns AND the sheet itself, so footer is respected).
   - Fonts, line-height, padding, margin, gaps and icon
     boxes are scaled with real pixel values (no zoom /
     transform) -> print and html2canvas see the same result.
   - Any space still left in a column after fitting is
     spread evenly between its sections (no empty gap).
   - Header text only shrinks.

   Optional attributes on <div id="resumeSheet">:
     data-autofit="off"   -> disable autofit
     data-fit-min="0.5"   -> smallest scale (0.3 - 0.99)  default 0.5
     data-fit-max="1.6"   -> largest scale  (1.01 - 2.5)  default 1.5
     data-fit-fill="off"  -> do not spread leftover space

   Public API:
     window.resumeAutofit.run()
     window.resumeAutofit.orientation()
========================================================= */

(function () {

    "use strict";

    var DEFAULT_MIN_SCALE = 0.5;
    var DEFAULT_MAX_SCALE = 1.5;
    var TOLERANCE = 1;
    var SEARCH_STEPS = 18;
    var SAFETY_FACTOR = 0.998;
    var MAX_FILL_GAP = 32;          /* max extra px added between two sections */

    var TEXT_PROPS = [
        "fontSize",
        "lineHeight",
        "paddingTop",
        "paddingRight",
        "paddingBottom",
        "paddingLeft",
        "marginTop",
        "marginBottom",
        "rowGap",
        "columnGap"
    ];

    /* fixed-size boxes that must grow/shrink with the text */
    var SIZE_SELECTOR =
        ".section-icon, .info-icon, .section-title-line, .header-accent";

    var SIZE_PROPS = [
        "width",
        "height",
        "minWidth",
        "minHeight",
        "flexBasis"
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

    var fillEnabled = sheet.getAttribute("data-fit-fill") !== "off";


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

    function readProps(element, names, base) {

        var style = window.getComputedStyle(element);
        var found = false;

        for (var i = 0; i < names.length; i++) {

            var value = parseFloat(style[names[i]]);

            if (!isNaN(value) && value > 0) {
                base[names[i]] = value;
                found = true;
            }
        }

        return found;
    }

    function capture(root) {

        var items = [];
        var nodes = root.querySelectorAll("*");

        for (var i = 0; i < nodes.length; i++) {

            var element = nodes[i];
            var base = {};
            var found = readProps(element, TEXT_PROPS, base);

            if (element.matches(SIZE_SELECTOR)) {
                found = readProps(element, SIZE_PROPS, base) || found;
            }

            if (found) {
                items.push({ element: element, base: base });
            }
        }

        return items;
    }

    function makeColumn(selector) {

        var root = sheet.querySelector(selector);

        if (!root) {
            return null;
        }

        var sections = Array.prototype.filter.call(
            root.children,
            function (child) {
                return child.classList.contains("resume-section");
            }
        );

        return {
            root: root,
            items: capture(root),
            sections: sections
        };
    }

    var headerColumn = makeColumn(".header-info");

    var bodyColumns = [
        makeColumn(".left-column"),
        makeColumn(".right-column")
    ].filter(Boolean);


    /* -----------------------------------------------------
       APPLY SCALE
    ----------------------------------------------------- */

    function applyScale(column, scale) {

        column.items.forEach(function (item) {

            for (var name in item.base) {

                item.element.style[name] =
                    scale === 1
                        ? ""
                        : (item.base[name] * scale).toFixed(3) + "px";
            }
        });
    }

    function applyAll(columns, scale) {
        columns.forEach(function (column) {
            applyScale(column, scale);
        });
    }

    function clearFill() {

        bodyColumns.forEach(function (column) {
            column.sections.forEach(function (section) {
                section.style.marginBottom = "";
            });
        });
    }

    function overflows(root) {
        return (
            root.scrollHeight - root.clientHeight > TOLERANCE ||
            root.scrollWidth - root.clientWidth > TOLERANCE
        );
    }

    /* columns AND the whole sheet (footer, header) must fit */
    function anyOverflow(columns) {

        if (overflows(sheet)) {
            return true;
        }

        return columns.some(function (column) {
            return overflows(column.root);
        });
    }


    /* -----------------------------------------------------
       FIT HEADER TEXT (own scale, shrink only)
    ----------------------------------------------------- */

    function fitHeader(column) {

        var root = column.root;

        applyScale(column, 1);

        if (!overflows(root)) {
            return;
        }

        var low = minScale;
        var high = 1;

        applyScale(column, low);

        if (overflows(root)) {
            return;
        }

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
    }


    /* -----------------------------------------------------
       FIT BODY COLUMNS (ONE shared scale)
    ----------------------------------------------------- */

    function fitShared(columns) {

        if (!columns.length) {
            return;
        }

        var low = minScale;
        var high = maxScale;

        /* everything fits at the largest allowed scale */
        applyAll(columns, high);

        if (!anyOverflow(columns)) {
            return;
        }

        /* does not fit even at the smallest scale: keep smallest */
        applyAll(columns, low);

        if (anyOverflow(columns)) {
            return;
        }

        /* binary search: largest common scale with no overflow */
        for (var i = 0; i < SEARCH_STEPS; i++) {

            var middle = (low + high) / 2;

            applyAll(columns, middle);

            if (anyOverflow(columns)) {
                high = middle;
            } else {
                low = middle;
            }
        }

        applyAll(columns, Math.max(minScale, low * SAFETY_FACTOR));
    }


    /* -----------------------------------------------------
       FILL LEFTOVER SPACE
       Spread the empty height of a column evenly between
       its sections so no blank area remains at the bottom.
    ----------------------------------------------------- */

    function usedHeight(column) {

        var sections = column.sections;
        var last = sections[sections.length - 1];

        var rootStyle = window.getComputedStyle(column.root);
        var lastStyle = window.getComputedStyle(last);

        return (
            last.getBoundingClientRect().bottom -
            column.root.getBoundingClientRect().top +
            (parseFloat(rootStyle.paddingBottom) || 0) +
            (parseFloat(lastStyle.marginBottom) || 0)
        );
    }

    function fillColumn(column) {

        var sections = column.sections;

        if (sections.length < 2) {
            return;
        }

        var leftover = column.root.clientHeight - usedHeight(column);

        if (leftover <= TOLERANCE * 2) {
            return;
        }

        var gaps = sections.slice(0, -1);

        var baseMargins = gaps.map(function (section) {
            return parseFloat(window.getComputedStyle(section).marginBottom) || 0;
        });

        var extra = Math.min(leftover / gaps.length, MAX_FILL_GAP);

        for (var attempt = 0; attempt < 6; attempt++) {

            gaps.forEach(function (section, index) {
                section.style.marginBottom =
                    (baseMargins[index] + extra).toFixed(2) + "px";
            });

            if (!anyOverflow(bodyColumns)) {
                return;
            }

            extra *= 0.75;
        }

        /* could not add space safely: restore */
        gaps.forEach(function (section, index) {
            section.style.marginBottom = baseMargins[index].toFixed(2) + "px";
        });
    }


    /* -----------------------------------------------------
       RUN
    ----------------------------------------------------- */

    function run() {

        markOrientation();

        /* 1. reset to original CSS sizes */
        clearFill();

        if (headerColumn) {
            applyScale(headerColumn, 1);
        }

        applyAll(bodyColumns, 1);

        /* 2. header text */
        if (headerColumn) {
            fitHeader(headerColumn);
        }

        /* 3. one shared scale for both columns */
        fitShared(bodyColumns);

        /* 4. fill remaining space */
        if (fillEnabled) {
            bodyColumns.forEach(fillColumn);
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

    /* photo / signature load can change the layout */
    Array.prototype.forEach.call(sheet.querySelectorAll("img"), function (img) {

        if (!img.complete) {
            img.addEventListener("load", schedule);
            img.addEventListener("error", schedule);
        }
    });

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

    /* address text is filled later by the location lookup */
    if (window.MutationObserver) {

        var observer = new MutationObserver(schedule);

        ["resumeAddress", "resumeAddressLeft", "resumeFooterAddress"].forEach(function (id) {

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