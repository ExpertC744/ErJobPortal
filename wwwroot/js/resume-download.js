/* ================================================================
   1TRAINEE - FAST RESUME PRINT + PDF DOWNLOAD
   ---------------------------------------------------------------
   Common script for all resume templates

   Required:
       <div class="resume-sheet" id="resumeSheet">

   Optional:
       window.resumeFileName = "Candidate Name";

   PDF:
       A4
       Landscape / Portrait
       Fast generation
       Small / Medium file size
       Good resume quality
       Edge-to-edge
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

        quality: 0.90,

        filename: "Resume"

    };


    /* ============================================================
       A4
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
       GET SHEET
    ============================================================ */

    function getSheet() {

        return document.querySelector(
            CONFIG.selector
        );

    }


    /* ============================================================
       FILENAME
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
            .replace(/\s+/g, " ")
            .replace(/\.pdf$/i, "");

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
            catch (e) {
                console.warn(
                    "Font loading skipped.",
                    e
                );
            }

        }

    }


    /* ============================================================
       WAIT FOR IMAGES - FAST
    ============================================================ */

    async function waitForImages(sheet) {

        const images =
            sheet.querySelectorAll("img");

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

                    const finish = function () {

                        img.removeEventListener(
                            "load",
                            finish
                        );

                        img.removeEventListener(
                            "error",
                            finish
                        );

                        resolve();

                    };

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

            /*
               Maximum 2 seconds instead of
               waiting 5 seconds per image.
            */

            await Promise.race([

                Promise.all(pending),

                new Promise(function (resolve) {

                    setTimeout(
                        resolve,
                        2000
                    );

                })

            ]);

        }

    }


    /* ============================================================
       FAST RENDER WAIT
    ============================================================ */

    function waitForPaint() {

        return new Promise(function (resolve) {

            requestAnimationFrame(function () {

                requestAnimationFrame(resolve);

            });

        });

    }


    /* ============================================================
       SAVE STYLES
    ============================================================ */

    function saveStyles(sheet) {

        return {

            width: sheet.style.width,

            height: sheet.style.height,

            minWidth: sheet.style.minWidth,

            minHeight: sheet.style.minHeight,

            maxWidth: sheet.style.maxWidth,

            maxHeight: sheet.style.maxHeight,

            transform: sheet.style.transform,

            transformOrigin:
                sheet.style.transformOrigin,

            margin: sheet.style.margin,

            boxShadow: sheet.style.boxShadow,

            position: sheet.style.position,

            left: sheet.style.left,

            top: sheet.style.top,

            overflow: sheet.style.overflow,

            display: sheet.style.display

        };

    }


    /* ============================================================
       APPLY PDF STYLES
    ============================================================ */

    function applyPDFStyles(
        sheet,
        size
    ) {

        sheet.style.width =
            size.width + "mm";

        sheet.style.height =
            size.height + "mm";

        sheet.style.minWidth =
            size.width + "mm";

        sheet.style.minHeight =
            size.height + "mm";

        sheet.style.maxWidth =
            size.width + "mm";

        sheet.style.maxHeight =
            size.height + "mm";

        /*
           Remove responsive transforms.
        */

        sheet.style.transform =
            "none";

        sheet.style.transformOrigin =
            "top left";

        /*
           Remove preview spacing.
        */

        sheet.style.margin =
            "0";

        /*
           Remove browser preview shadow.
        */

        sheet.style.boxShadow =
            "none";

        sheet.style.position =
            "relative";

        sheet.style.left =
            "0";

        sheet.style.top =
            "0";

        sheet.style.overflow =
            "hidden";

    }


    /* ============================================================
       RESTORE STYLES
    ============================================================ */

    function restoreStyles(
        sheet,
        old
    ) {

        sheet.style.width =
            old.width;

        sheet.style.height =
            old.height;

        sheet.style.minWidth =
            old.minWidth;

        sheet.style.minHeight =
            old.minHeight;

        sheet.style.maxWidth =
            old.maxWidth;

        sheet.style.maxHeight =
            old.maxHeight;

        sheet.style.transform =
            old.transform;

        sheet.style.transformOrigin =
            old.transformOrigin;

        sheet.style.margin =
            old.margin;

        sheet.style.boxShadow =
            old.boxShadow;

        sheet.style.position =
            old.position;

        sheet.style.left =
            old.left;

        sheet.style.top =
            old.top;

        sheet.style.overflow =
            old.overflow;

        sheet.style.display =
            old.display;

    }


    /* ============================================================
       TOOLBAR
    ============================================================ */

    function createToolbar() {

        if (
            document.querySelector(
                ".resume-common-toolbar"
            )
        ) {
            return;
        }


        const toolbar =
            document.createElement("div");

        toolbar.className =
            "resume-common-toolbar";


        toolbar.innerHTML = `

            <button
                type="button"
                class="resume-common-btn resume-print-btn"
                data-common-print>

                <i class="fa-solid fa-print"></i>

                <span>Print</span>

            </button>


            <button
                type="button"
                class="resume-common-btn resume-download-btn"
                data-common-download>

                <i class="fa-solid fa-file-pdf"></i>

                <span>Download PDF</span>

            </button>

        `;


        const wrapper =
            document.querySelector(
                ".resume-page-wrapper"
            );


        if (wrapper) {

            wrapper.parentNode.insertBefore(
                toolbar,
                wrapper
            );

        }
        else {

            document.body.insertBefore(
                toolbar,
                document.body.firstChild
            );

        }


        addToolbarCSS();

    }


    /* ============================================================
       TOOLBAR CSS
    ============================================================ */

    function addToolbarCSS() {

        if (
            document.getElementById(
                "resume-common-toolbar-style"
            )
        ) {
            return;
        }


        const style =
            document.createElement("style");


        style.id =
            "resume-common-toolbar-style";


        style.textContent = `

            .resume-common-toolbar {

                width: 297mm;

                max-width:
                    calc(100% - 30px);

                margin:
                    14px auto 10px;

                display:
                    flex;

                justify-content:
                    flex-end;

                align-items:
                    center;

                gap: 8px;

                position:
                    relative;

                z-index:
                    9999;

            }


            .resume-common-btn {

                min-height:
                    36px;

                padding:
                    8px 15px;

                border:
                    1px solid #d9e0e4;

                border-radius:
                    6px;

                background:
                    #ffffff;

                color:
                    #263b47;

                font-family:
                    Arial,
                    sans-serif;

                font-size:
                    11px;

                font-weight:
                    700;

                cursor:
                    pointer;

                display:
                    inline-flex;

                align-items:
                    center;

                justify-content:
                    center;

                gap:
                    7px;

                box-shadow:
                    0 2px 8px
                    rgba(20,40,50,.08);

            }


            .resume-print-btn {

                background:
                    #263b47;

                border-color:
                    #263b47;

                color:
                    #ffffff;

            }


            .resume-download-btn {

                background:
                    #00796b;

                border-color:
                    #00796b;

                color:
                    #ffffff;

            }


            .resume-common-btn:disabled {

                opacity:
                    .65;

                cursor:
                    not-allowed;

            }


            @media print {

                .resume-common-toolbar {

                    display:
                        none !important;

                }

            }


            @media (max-width:1200px) {

                .resume-common-toolbar {

                    width:100%;

                    padding-left:12px;

                    padding-right:12px;

                }

            }

        `;


        document.head.appendChild(style);

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

            button.disabled =
                true;

            button.innerHTML =
                '<i class="fa-solid fa-spinner fa-spin"></i>' +
                '<span>Generating...</span>';

        }
        else {

            button.disabled =
                false;

            if (button.dataset.oldHtml) {

                button.innerHTML =
                    button.dataset.oldHtml;

            }

        }

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
                "[data-common-download]"
            );


        let oldStyles =
            null;


        try {

            setLoading(
                button,
                true
            );


            /* ====================================================
               WAIT ONLY ONCE
            ==================================================== */

            await waitForFonts();

            await waitForImages(sheet);

            await waitForPaint();


            /* ====================================================
               DYNAMIC TEMPLATE DATA
            ==================================================== */

            if (
                window.resumeReadyPromise &&
                typeof window.resumeReadyPromise.then ===
                "function"
            ) {

                try {

                    await window.resumeReadyPromise;

                }
                catch (e) {

                    console.warn(
                        "Dynamic resume data failed.",
                        e
                    );

                }

            }


            /* ====================================================
               PAGE SIZE
            ==================================================== */

            const size =
                String(
                    CONFIG.orientation
                ).toLowerCase() ===
                    "portrait"
                    ? A4.portrait
                    : A4.landscape;


            /* ====================================================
               SAVE + APPLY STYLES
            ==================================================== */

            oldStyles =
                saveStyles(sheet);


            applyPDFStyles(
                sheet,
                size
            );


            /*
               Force reflow.
            */

            void sheet.offsetWidth;


            await waitForPaint();


            /* ====================================================
               GET REAL SIZE
            ==================================================== */

            const rect =
                sheet.getBoundingClientRect();


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
                    "Invalid resume size."
                );

            }


            /* ====================================================
               CAPTURE
            ==================================================== */

            const canvas =
                await html2canvas(

                    sheet,

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

                        windowWidth:
                            width,

                        windowHeight:
                            height,

                        ignoreElements:
                            function (el) {

                                return el.hasAttribute(
                                    "data-resume-ignore"
                                );

                            }

                    }

                );


            /* ====================================================
               RESTORE PAGE
            ==================================================== */

            restoreStyles(
                sheet,
                oldStyles
            );

            oldStyles =
                null;


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
                        true

                });


            /* ====================================================
               SMALL JPEG
            ==================================================== */

            const image =
                canvas.toDataURL(
                    "image/jpeg",
                    CONFIG.quality
                );


            /* ====================================================
               FULL A4 PAGE
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
               DOWNLOAD
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

            if (oldStyles) {

                restoreStyles(
                    sheet,
                    oldStyles
                );

            }


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
       EVENTS
    ============================================================ */

    function initializeEvents() {

        document.addEventListener(
            "click",
            function (event) {

                const download =
                    event.target.closest(
                        "[data-common-download]"
                    );


                if (download) {

                    event.preventDefault();


                    if (
                        download.disabled
                    ) {

                        return;

                    }


                    downloadResumePDF();

                    return;

                }


                const print =
                    event.target.closest(
                        "[data-common-print]"
                    );


                if (print) {

                    event.preventDefault();

                    printResume();

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

        createToolbar();

        initializeEvents();

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