/* ================================================================
   1TRAINEE - COMMON RESUME PRINT + PDF DOWNLOAD
   ---------------------------------------------------------------
   MOBILE / TABLET SAFE PDF VERSION

   IMPORTANT:
   ---------------------------------------------------------------
   The visible resume is NEVER resized or modified.

   PDF generation uses a separate hidden A4 rendering copy.

   Therefore:

   Desktop  -> A4 layout
   Tablet   -> A4 layout
   Mobile   -> A4 layout

   Responsive mobile CSS will NOT break the PDF.

   PDF:
       A4 Landscape
       297mm × 210mm
       No margin
       No extra corner spacing
       No mobile reflow
       No tablet reflow
       Exact desktop/A4 resume structure
================================================================ */

(function () {

    "use strict";


    /* ============================================================
       CONFIG
    ============================================================ */

    const CONFIG = {

        selector: "#resumeSheet",

        orientation: "landscape",

        format: "a4",

        scale: 2.5,

        quality: 0.95,

        filename: "Resume",

        /*
         * Fixed desktop rendering width.
         *
         * 297mm at 96 DPI ≈ 1122.52px
         *
         * We use 1200px as the CSS rendering viewport.
         * This prevents mobile/tablet media queries from
         * changing the resume layout.
         */
        renderWidth: 1200,

        renderHeight: 848

    };


    /* ============================================================
       A4 SIZE
    ============================================================ */

    const A4 = {

        landscape: {
            width: 297,
            height: 210
        },

        portrait: {
            width: 210,
            height: 297
        }

    };


    /* ============================================================
       GET RESUME
    ============================================================ */

    function getSheet() {

        return document.querySelector(
            CONFIG.selector
        );

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

            .replace(
                /[<>:"/\\|?*\x00-\x1F]/g,
                ""
            )

            .replace(
                /\s+/g,
                " "
            )

            .replace(
                /\.pdf$/i,
                ""
            );


        return name + "_Resume.pdf";

    }


    /* ============================================================
       WAIT FOR FONTS
    ============================================================ */

    async function waitForFonts() {

        if (
            document.fonts &&
            document.fonts.ready
        ) {

            try {

                await document.fonts.ready;

            }
            catch (error) {

                console.warn(
                    "Font loading skipped.",
                    error
                );

            }

        }

    }


    /* ============================================================
       WAIT FOR IMAGES
    ============================================================ */

    async function waitForImages(
        container
    ) {

        const images =
            container.querySelectorAll("img");


        if (!images.length) {

            return;

        }


        const pending = [];


        images.forEach(function (img) {

            if (
                img.complete &&
                img.naturalWidth > 0
            ) {

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


                        img.removeEventListener(
                            "load",
                            finish
                        );


                        img.removeEventListener(
                            "error",
                            finish
                        );


                        resolve();

                    }


                    img.addEventListener(
                        "load",
                        finish,
                        { once: true }
                    );


                    img.addEventListener(
                        "error",
                        finish,
                        { once: true }
                    );

                })

            );

        });


        if (pending.length) {

            await Promise.race([

                Promise.all(pending),

                new Promise(function (resolve) {

                    setTimeout(
                        resolve,
                        5000
                    );

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

    function setLoading(
        button,
        loading
    ) {

        if (!button) {

            return;

        }


        if (loading) {

            if (!button.dataset.oldHtml) {

                button.dataset.oldHtml =
                    button.innerHTML;

            }


            button.disabled = true;


            button.innerHTML =
                '<i class="fa-solid fa-spinner fa-spin"></i>';

        }
        else {

            button.disabled = false;


            if (button.dataset.oldHtml) {

                button.innerHTML =
                    button.dataset.oldHtml;

                delete button.dataset.oldHtml;

            }

        }

    }


    /* ============================================================
       CREATE PDF RENDER CONTAINER
       ------------------------------------------------------------
       This is the most important part.

       The actual resume on the screen is NOT changed.

       A separate hidden rendering area is created with:

           width  = 1200px
           height = 848px

       This gives html2canvas a desktop-like rendering
       environment even when the user is on a mobile device.
    ============================================================ */

    function createPDFRenderContainer(
        sheet
    ) {

        const container =
            document.createElement("div");


        container.id =
            "resumePdfRenderContainer";


        container.setAttribute(
            "data-resume-pdf-render",
            "true"
        );


        /*
         * Completely independent from page layout.
         */

        container.style.position =
            "fixed";

        container.style.left =
            "-100000px";

        container.style.top =
            "0";

        container.style.width =
            CONFIG.renderWidth + "px";

        container.style.height =
            CONFIG.renderHeight + "px";

        container.style.minWidth =
            CONFIG.renderWidth + "px";

        container.style.maxWidth =
            CONFIG.renderWidth + "px";

        container.style.overflow =
            "hidden";

        container.style.background =
            "#ffffff";

        container.style.zIndex =
            "-1";

        container.style.pointerEvents =
            "none";


        /*
         * Create a deep copy of the resume.
         */

        const clone =
            sheet.cloneNode(true);


        clone.removeAttribute("id");


        clone.setAttribute(
            "data-pdf-resume",
            "true"
        );


        /*
         * Force desktop/A4 resume dimensions.
         */

        const size =
            String(
                CONFIG.orientation
            ).toLowerCase() ===
                "portrait"

                ? A4.portrait

                : A4.landscape;


        /*
         * A4 ratio.
         */

        const aspectRatio =
            size.width /
            size.height;


        const renderWidth =
            CONFIG.renderWidth;


        const renderHeight =
            Math.round(
                renderWidth /
                aspectRatio
            );


        container.style.height =
            renderHeight + "px";


        container.style.minHeight =
            renderHeight + "px";


        container.style.maxHeight =
            renderHeight + "px";


        /*
         * Force the cloned resume itself.
         */

        clone.style.width =
            renderWidth + "px";

        clone.style.minWidth =
            renderWidth + "px";

        clone.style.maxWidth =
            renderWidth + "px";


        clone.style.height =
            renderHeight + "px";

        clone.style.minHeight =
            renderHeight + "px";

        clone.style.maxHeight =
            renderHeight + "px";


        clone.style.margin =
            "0";

        clone.style.padding =
            clone.style.padding || "0";


        clone.style.position =
            "relative";


        clone.style.left =
            "0";

        clone.style.top =
            "0";


        clone.style.transform =
            "none";


        clone.style.transformOrigin =
            "top left";


        clone.style.boxSizing =
            "border-box";


        clone.style.overflow =
            "hidden";


        clone.style.boxShadow =
            "none";


        /*
         * Make sure theme is preserved.
         */

        if (
            sheet.hasAttribute(
                "data-theme"
            )
        ) {

            clone.setAttribute(
                "data-theme",
                sheet.getAttribute(
                    "data-theme"
                )
            );

        }


        /*
         * Prevent PDF toolbar/navigation
         * elements from appearing if any
         * somehow exist inside the resume.
         */

        clone
            .querySelectorAll(
                "[data-resume-action]"
            )
            .forEach(function (element) {

                element.style.display =
                    "none";

            });


        clone
            .querySelectorAll(
                ".resume-navigation, .resume-top-toolbar"
            )
            .forEach(function (element) {

                element.style.display =
                    "none";

            });


        container.appendChild(clone);


        document.body.appendChild(
            container
        );


        return {

            container: container,

            clone: clone,

            width: renderWidth,

            height: renderHeight

        };

    }


    /* ============================================================
       REMOVE PDF RENDER CONTAINER
    ============================================================ */

    function removePDFRenderContainer(
        render
    ) {

        if (
            !render ||
            !render.container
        ) {

            return;

        }


        if (
            render.container.parentNode
        ) {

            render.container.parentNode.removeChild(
                render.container
            );

        }

    }


    /* ============================================================
       COPY CSS VARIABLES
       ------------------------------------------------------------
       Theme variables are copied from the real resume to the
       PDF clone so the selected theme remains identical.
    ============================================================ */

    function copyThemeVariables(
        source,
        target
    ) {

        try {

            const computed =
                window.getComputedStyle(
                    source
                );


            const variables = [

                "--resume-primary",

                "--resume-secondary",

                "--resume-accent",

                "--resume-text",

                "--resume-muted",

                "--resume-bg",

                "--resume-border"

            ];


            variables.forEach(function (variable) {

                const value =
                    computed.getPropertyValue(
                        variable
                    );


                if (value) {

                    target.style.setProperty(
                        variable,
                        value
                    );

                }

            });

        }
        catch (error) {

            console.warn(
                "Theme variables could not be copied.",
                error
            );

        }

    }


    /* ============================================================
       COPY IMPORTANT INLINE STATE
    ============================================================ */

    function copyResumeState(
        source,
        target
    ) {

        /*
         * Copy theme.
         */

        if (
            source.hasAttribute(
                "data-theme"
            )
        ) {

            target.setAttribute(
                "data-theme",
                source.getAttribute(
                    "data-theme"
                )
            );

        }


        /*
         * Copy CSS custom properties.
         */

        copyThemeVariables(
            source,
            target
        );

    }


    /* ============================================================
       DOWNLOAD PDF
    ============================================================ */

    async function downloadResumePDF() {

        const sheet =
            getSheet();


        if (!sheet) {

            alert(
                'Resume container with id="resumeSheet" was not found.'
            );

            return;

        }


        if (
            typeof html2canvas !==
            "function"
        ) {

            alert(
                "html2canvas is not loaded."
            );

            return;

        }


        if (
            !window.jspdf ||
            !window.jspdf.jsPDF
        ) {

            alert(
                "jsPDF is not loaded."
            );

            return;

        }


        const button =
            document.querySelector(
                '[data-resume-action="download"]'
            );


        let render = null;


        try {

            setLoading(
                button,
                true
            );


            /* ====================================================
               WAIT FOR NORMAL PAGE
            ==================================================== */

            await waitForFonts();

            await waitForImages(
                sheet
            );


            /* ====================================================
               DYNAMIC DATA
            ==================================================== */

            if (
                window.resumeReadyPromise &&
                typeof window.resumeReadyPromise.then ===
                "function"
            ) {

                try {

                    await window.resumeReadyPromise;

                }
                catch (error) {

                    console.warn(
                        "Dynamic resume data failed.",
                        error
                    );

                }

            }


            await waitForPaint();


            /* ====================================================
               CREATE DESKTOP PDF COPY
            ==================================================== */

            render =
                createPDFRenderContainer(
                    sheet
                );


            /*
             * Copy current theme.
             */

            copyResumeState(
                sheet,
                render.clone
            );


            /*
             * Wait for cloned images.
             */

            await waitForImages(
                render.clone
            );


            /*
             * Wait for cloned fonts/layout.
             */

            await waitForFonts();

            await waitForPaint();


            /*
             * Force browser layout calculation.
             */

            void render.clone.offsetWidth;


            await waitForPaint();


            /* ====================================================
               GET FINAL PDF RENDER SIZE
            ==================================================== */

            const rect =
                render.clone.getBoundingClientRect();


            const width =
                Math.round(
                    rect.width
                );


            const height =
                Math.round(
                    rect.height
                );


            if (
                width <= 0 ||
                height <= 0
            ) {

                throw new Error(
                    "PDF render has an invalid size."
                );

            }


            /* ====================================================
               HTML2CANVAS
               ----------------------------------------------------
               IMPORTANT:

               windowWidth is intentionally LARGE.

               This prevents mobile media queries such as:

                   @media(max-width:768px)

               from changing the resume layout.
            ==================================================== */

            const canvas =
                await html2canvas(

                    render.clone,

                    {

                        scale:
                            CONFIG.scale,


                        useCORS:
                            true,


                        allowTaint:
                            false,


                        backgroundColor:
                            "#ffffff",


                        logging:
                            false,


                        width:
                            width,


                        height:
                            height,


                        x:
                            0,


                        y:
                            0,


                        scrollX:
                            0,


                        scrollY:
                            0,


                        /*
                         * VERY IMPORTANT
                         *
                         * Do NOT use:
                         *
                         * document.documentElement.clientWidth
                         *
                         * because on mobile it can be
                         * 390px / 430px etc.
                         *
                         * Use desktop rendering width.
                         */

                        windowWidth:
                            CONFIG.renderWidth,


                        windowHeight:
                            CONFIG.renderHeight,


                        ignoreElements:
                            function (element) {

                                return element.hasAttribute(
                                    "data-resume-ignore"
                                );

                            }

                    }

                );


            /* ====================================================
               CHECK CANVAS
            ==================================================== */

            if (
                !canvas ||
                canvas.width <= 0 ||
                canvas.height <= 0
            ) {

                throw new Error(
                    "Resume canvas could not be generated."
                );

            }


            /* ====================================================
               PDF SIZE
            ==================================================== */

            const size =
                String(
                    CONFIG.orientation
                ).toLowerCase() ===
                    "portrait"

                    ? A4.portrait

                    : A4.landscape;


            /* ====================================================
               CREATE PDF
            ==================================================== */

            const jsPDF =
                window.jspdf.jsPDF;


            const pdf =
                new jsPDF({

                    orientation:
                        CONFIG.orientation,

                    unit:
                        "mm",

                    format:
                        "a4",

                    compress:
                        true,

                    putOnlyUsedFonts:
                        true

                });


            /* ====================================================
               CANVAS → IMAGE
            ==================================================== */

            const image =
                canvas.toDataURL(
                    "image/jpeg",
                    CONFIG.quality
                );


            /* ====================================================
               FULL A4 PAGE
               ----------------------------------------------------
               No margin
               No padding
               No extra corner space
            ==================================================== */

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


            /* ====================================================
               SAVE
            ==================================================== */

            pdf.save(
                getFileName()
            );

        }
        catch (error) {

            console.error(
                "Resume PDF error:",
                error
            );


            alert(
                "Unable to generate PDF."
            );

        }
        finally {

            /*
             * Always remove temporary desktop
             * rendering copy.
             */

            removePDFRenderContainer(
                render
            );


            setLoading(
                button,
                false
            );

        }

    }


    /* ============================================================
       PRINT
    ============================================================ */

    function printResume() {

        const sheet =
            getSheet();


        if (!sheet) {

            alert(
                'Resume container with id="resumeSheet" was not found.'
            );

            return;

        }


        window.print();

    }


    /* ============================================================
       MASTER LAYOUT EVENTS
    ============================================================ */

    function initializeEvents() {

        document.addEventListener(
            "click",
            function (event) {

                const actionButton =
                    event.target.closest(
                        "[data-resume-action]"
                    );


                if (!actionButton) {

                    return;

                }


                const action =
                    actionButton.dataset.resumeAction;


                /* =================================================
                   DOWNLOAD
                ================================================= */

                if (
                    action === "download"
                ) {

                    event.preventDefault();


                    if (
                        actionButton.disabled
                    ) {

                        return;

                    }


                    downloadResumePDF();

                    return;

                }


                /* =================================================
                   PRINT
                ================================================= */

                if (
                    action === "print"
                ) {

                    event.preventDefault();


                    printResume();

                    return;

                }

            }
        );

    }


    /* ============================================================
       PUBLIC FUNCTIONS
    ============================================================ */

    window.downloadResumePDF =
        downloadResumePDF;


    window.printResume =
        printResume;


    /* ============================================================
       INITIALIZE
    ============================================================ */

    function initialize() {
        initializeEvents()
    }


    if (
        document.readyState ===
        "loading"
    ) {

        document.addEventListener(
            "DOMContentLoaded",
            initialize
        );

    }
    else {

        initialize();

    }


})();