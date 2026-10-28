/* ================================================================
   1TRAINEE - COMMON RESUME PRINT + PDF DOWNLOAD
   ---------------------------------------------------------------
   AUTO ORIENTATION (LANDSCAPE + PORTRAIT)

   The visible resume is NEVER resized or modified.
   PDF generation uses a separate hidden A4 rendering copy.

   Landscape sheet -> A4 landscape PDF (297mm x 210mm)
   Portrait sheet  -> A4 portrait PDF  (210mm x 297mm)

   Mobile / tablet zoom rules never affect the PDF.
================================================================ */

(function () {

    "use strict";


    /* ============================================================
       CONFIG
    ============================================================ */

    const CONFIG = {
        selector: "#resumeSheet",
        format: "a4",
        scale: 2.5,
        quality: 0.95,
        filename: "Resume"
    };


    /*
     * Fixed render sizes (CSS px).
     * Landscape: 1200 x 848   (A4 ratio 297:210)
     * Portrait : 794  x 1123  (A4 at 96 DPI, same as the sheet CSS)
     */
    const RENDER = {
        landscape: { width: 1200, height: 848 },
        portrait: { width: 794, height: 1123 }
    };


    /*
     * Window width given to html2canvas. Large enough so
     * mobile @media (max-width) rules never apply.
     */
    const RENDER_WINDOW_WIDTH = 1200;


    /* ============================================================
       A4 SIZE (mm)
    ============================================================ */

    const A4 = {
        landscape: { width: 297, height: 210 },
        portrait: { width: 210, height: 297 }
    };


    /* ============================================================
       GET RESUME
    ============================================================ */

    function getSheet() {
        return document.querySelector(CONFIG.selector);
    }


    /* ============================================================
       DETECT ORIENTATION
       ------------------------------------------------------------
       Uses the sheet's CSS width/height (not affected by zoom
       or transforms), so it works on mobile too.
    ============================================================ */

    function getOrientation(sheet) {

        const cs = window.getComputedStyle(sheet);

        const w = parseFloat(cs.width) || sheet.offsetWidth;
        const h = parseFloat(cs.height) || sheet.offsetHeight;

        return h > w ? "portrait" : "landscape";
    }


    /* ============================================================
       FILE NAME
    ============================================================ */

    function getFileName() {

        let name =
            window.resumeFileName ||
            window.candidateName ||
            document.body.dataset.candidateName ||
            CONFIG.filename;

        name = String(name).trim();

        if (!name) {
            name = CONFIG.filename;
        }

        name = name
            .replace(/[<>:"/\\|?*\x00-\x1F]/g, "")
            .replace(/\s+/g, " ")
            .replace(/\.pdf$/i, "");

        return name + "_Resume.pdf";
    }


    /* ============================================================
       WAIT FOR FONTS
    ============================================================ */

    async function waitForFonts() {

        if (document.fonts && document.fonts.ready) {

            try {
                await document.fonts.ready;
            }
            catch (error) {
                console.warn("Font loading skipped.", error);
            }
        }
    }


    /* ============================================================
       WAIT FOR IMAGES
    ============================================================ */

    async function waitForImages(container) {

        const images = container.querySelectorAll("img");

        if (!images.length) {
            return;
        }

        const pending = [];

        images.forEach(function (img) {

            if (img.complete && img.naturalWidth > 0) {
                return;
            }

            pending.push(

                new Promise(function (resolve) {

                    let finished = false;

                    function finish() {

                        if (finished) {
                            return;
                        }

                        finished = true;

                        img.removeEventListener("load", finish);
                        img.removeEventListener("error", finish);

                        resolve();
                    }

                    img.addEventListener("load", finish, { once: true });
                    img.addEventListener("error", finish, { once: true });
                })
            );
        });

        if (pending.length) {

            await Promise.race([
                Promise.all(pending),
                new Promise(function (resolve) {
                    setTimeout(resolve, 5000);
                })
            ]);
        }
    }


    /* ============================================================
       WAIT FOR PAINT
    ============================================================ */

    function waitForPaint() {

        return new Promise(function (resolve) {

            requestAnimationFrame(function () {
                requestAnimationFrame(function () {
                    resolve();
                });
            });
        });
    }


    /* ============================================================
       BUTTON LOADING
    ============================================================ */

    function setLoading(button, loading) {

        if (!button) {
            return;
        }

        if (loading) {

            if (!button.dataset.oldHtml) {
                button.dataset.oldHtml = button.innerHTML;
            }

            button.disabled = true;

            button.innerHTML =
                '<i class="fa-solid fa-spinner fa-spin"></i>';
        }
        else {

            button.disabled = false;

            if (button.dataset.oldHtml) {
                button.innerHTML = button.dataset.oldHtml;
                delete button.dataset.oldHtml;
            }
        }
    }


    /* ============================================================
       CREATE PDF RENDER CONTAINER
       ------------------------------------------------------------
       Hidden copy of the resume at a fixed size that matches the
       detected orientation. The visible resume is not touched.
    ============================================================ */

    function createPDFRenderContainer(sheet) {

        const orientation = getOrientation(sheet);

        const renderWidth = RENDER[orientation].width;
        const renderHeight = RENDER[orientation].height;


        const container = document.createElement("div");

        container.id = "resumePdfRenderContainer";

        container.setAttribute("data-resume-pdf-render", "true");


        /* Completely independent from page layout */

        container.style.position = "fixed";
        container.style.left = "-100000px";
        container.style.top = "0";
        container.style.overflow = "hidden";
        container.style.background = "#ffffff";
        container.style.zIndex = "-1";
        container.style.pointerEvents = "none";


        /* Deep copy of the resume */

        const clone = sheet.cloneNode(true);

        clone.removeAttribute("id");

        clone.setAttribute("data-pdf-resume", "true");


        /* Force exact render size on container + clone */

        ["width", "minWidth", "maxWidth"].forEach(function (key) {
            container.style[key] = renderWidth + "px";
            clone.style[key] = renderWidth + "px";
        });

        ["height", "minHeight", "maxHeight"].forEach(function (key) {
            container.style[key] = renderHeight + "px";
            clone.style[key] = renderHeight + "px";
        });


        /* IMPORTANT: ignore the mobile zoom rules */

        clone.style.zoom = "1";

        clone.style.margin = "0";
        clone.style.padding = clone.style.padding || "0";
        clone.style.position = "relative";
        clone.style.left = "0";
        clone.style.top = "0";
        clone.style.transform = "none";
        clone.style.transformOrigin = "top left";
        clone.style.boxSizing = "border-box";
        clone.style.overflow = "hidden";
        clone.style.boxShadow = "none";


        /* Preserve theme */

        if (sheet.hasAttribute("data-theme")) {
            clone.setAttribute(
                "data-theme",
                sheet.getAttribute("data-theme")
            );
        }


        /* Hide toolbar / navigation if present inside resume */

        clone
            .querySelectorAll("[data-resume-action]")
            .forEach(function (element) {
                element.style.display = "none";
            });

        clone
            .querySelectorAll(".resume-navigation, .resume-top-toolbar")
            .forEach(function (element) {
                element.style.display = "none";
            });


        container.appendChild(clone);

        document.body.appendChild(container);


        return {
            container: container,
            clone: clone,
            width: renderWidth,
            height: renderHeight,
            orientation: orientation
        };
    }


    /* ============================================================
       REMOVE PDF RENDER CONTAINER
    ============================================================ */

    function removePDFRenderContainer(render) {

        if (!render || !render.container) {
            return;
        }

        if (render.container.parentNode) {
            render.container.parentNode.removeChild(render.container);
        }
    }


    /* ============================================================
       COPY CSS VARIABLES
    ============================================================ */

    function copyThemeVariables(source, target) {

        try {

            const computed = window.getComputedStyle(source);

            const variables = [
                "--resume-primary",
                "--resume-secondary",
                "--resume-accent",
                "--resume-text",
                "--resume-muted",
                "--resume-bg",
                "--resume-border",
                "--fit"
            ];

            variables.forEach(function (variable) {

                const value = computed.getPropertyValue(variable);

                if (value) {
                    target.style.setProperty(variable, value);
                }
            });
        }
        catch (error) {
            console.warn("Theme variables could not be copied.", error);
        }
    }


    /* ============================================================
       COPY IMPORTANT STATE
       (--fit is copied too, so the autofit result is identical)
    ============================================================ */

    function copyResumeState(source, target) {

        if (source.hasAttribute("data-theme")) {
            target.setAttribute(
                "data-theme",
                source.getAttribute("data-theme")
            );
        }

        copyThemeVariables(source, target);
    }


    /* ============================================================
       DOWNLOAD PDF
    ============================================================ */

    async function downloadResumePDF() {

        const sheet = getSheet();

        if (!sheet) {
            alert('Resume container with id="resumeSheet" was not found.');
            return;
        }

        if (typeof html2canvas !== "function") {
            alert("html2canvas is not loaded.");
            return;
        }

        if (!window.jspdf || !window.jspdf.jsPDF) {
            alert("jsPDF is not loaded.");
            return;
        }

        const button = document.querySelector(
            '[data-resume-action="download"]'
        );

        let render = null;

        try {

            setLoading(button, true);


            /* Wait for the normal page */

            await waitForFonts();
            await waitForImages(sheet);


            /* Dynamic data */

            if (
                window.resumeReadyPromise &&
                typeof window.resumeReadyPromise.then === "function"
            ) {

                try {
                    await window.resumeReadyPromise;
                }
                catch (error) {
                    console.warn("Dynamic resume data failed.", error);
                }
            }

            await waitForPaint();


            /* Create hidden copy */

            render = createPDFRenderContainer(sheet);

            copyResumeState(sheet, render.clone);

            await waitForImages(render.clone);
            await waitForFonts();
            await waitForPaint();

            void render.clone.offsetWidth;

            await waitForPaint();


            /* Final size */

            const rect = render.clone.getBoundingClientRect();

            const width = Math.round(rect.width);
            const height = Math.round(rect.height);

            if (width <= 0 || height <= 0) {
                throw new Error("PDF render has an invalid size.");
            }


            /* html2canvas */

            const canvas = await html2canvas(render.clone, {

                scale: CONFIG.scale,
                useCORS: true,
                allowTaint: false,
                backgroundColor: "#ffffff",
                logging: false,

                width: width,
                height: height,

                x: 0,
                y: 0,
                scrollX: 0,
                scrollY: 0,

                /* Wide window: mobile media queries never apply */
                windowWidth: RENDER_WINDOW_WIDTH,
                windowHeight: Math.max(render.height, 1),

                ignoreElements: function (element) {
                    return element.hasAttribute("data-resume-ignore");
                }
            });

            if (!canvas || canvas.width <= 0 || canvas.height <= 0) {
                throw new Error("Resume canvas could not be generated.");
            }


            /* PDF - orientation matches the sheet */

            const orientation = render.orientation;
            const size = A4[orientation];

            const jsPDF = window.jspdf.jsPDF;

            const pdf = new jsPDF({
                orientation: orientation,
                unit: "mm",
                format: CONFIG.format,
                compress: true,
                putOnlyUsedFonts: true
            });

            const image = canvas.toDataURL("image/jpeg", CONFIG.quality);


            /* Full A4 page, no margin */

            pdf.addImage(
                image,
                "JPEG",
                0,
                0,
                size.width,
                size.height,
                undefined,
                "FAST"
            );

            pdf.save(getFileName());
        }
        catch (error) {

            console.error("Resume PDF error:", error);

            alert("Unable to generate PDF.");
        }
        finally {

            removePDFRenderContainer(render);

            setLoading(button, false);
        }
    }


    /* ============================================================
       PRINT
    ============================================================ */

    function printResume() {

        const sheet = getSheet();

        if (!sheet) {
            alert('Resume container with id="resumeSheet" was not found.');
            return;
        }

        window.print();
    }


    /* ============================================================
       EVENTS
    ============================================================ */

    function initializeEvents() {

        document.addEventListener("click", function (event) {

            const actionButton = event.target.closest("[data-resume-action]");

            if (!actionButton) {
                return;
            }

            const action = actionButton.dataset.resumeAction;

            if (action === "download") {

                event.preventDefault();

                if (actionButton.disabled) {
                    return;
                }

                downloadResumePDF();

                return;
            }

            if (action === "print") {

                event.preventDefault();

                printResume();

                return;
            }
        });
    }


    /* ============================================================
       PUBLIC FUNCTIONS
    ============================================================ */

    window.downloadResumePDF = downloadResumePDF;

    window.printResume = printResume;


    /* ============================================================
       INITIALIZE
    ============================================================ */

    function initialize() {
        initializeEvents();
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", initialize);
    }
    else {
        initialize();
    }

})();