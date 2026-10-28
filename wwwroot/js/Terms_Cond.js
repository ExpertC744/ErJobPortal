
document.addEventListener("DOMContentLoaded", function () {

    const progressBar = document.getElementById("termsProgress");
    const backToTop = document.getElementById("termsTop");
    const tocLinks = document.querySelectorAll(".toc-link");
    const sections = document.querySelectorAll(".terms-section");

    /* =====================================================
       SMOOTH SCROLL + ACTIVE TOC
    ====================================================== */

    tocLinks.forEach(function (link) {

        link.addEventListener("click", function (event) {

            event.preventDefault();

            const targetId = link.getAttribute("href");
            const target = document.querySelector(targetId);

            if (!target) return;

            // Remove active color from all previous tabs
            tocLinks.forEach(function (tocLink) {
                tocLink.classList.remove("active");
            });

            // Add active color to the clicked tab immediately
            link.classList.add("active");

            const headerOffset = 90;

            const targetPosition =
                target.getBoundingClientRect().top +
                window.scrollY -
                headerOffset;

            window.scrollTo({
                top: targetPosition,
                behavior: "smooth"
            });

        });

    });


    /* =====================================================
       UPDATE ACTIVE TOC ON SCROLL
    ====================================================== */

    function updateActiveSection() {

        if (!sections.length || !tocLinks.length) return;

        let currentSection = "";
        const scrollPosition = window.scrollY + 140;

        sections.forEach(function (section) {

            const sectionTop =
                section.getBoundingClientRect().top + window.scrollY;

            if (scrollPosition >= sectionTop) {
                currentSection = section.id;
            }

        });

        // Remove active from EVERY link before applying the new one
        tocLinks.forEach(function (link) {

            link.classList.remove("active");

            if (
                currentSection &&
                link.getAttribute("href") === "#" + currentSection
            ) {
                link.classList.add("active");
            }

        });

    }


    /* =====================================================
       SCROLL PROGRESS
    ====================================================== */

    function updateProgress() {

        if (!progressBar) return;

        const scrollTop = window.scrollY;

        const documentHeight =
            document.documentElement.scrollHeight -
            document.documentElement.clientHeight;

        if (documentHeight <= 0) {
            progressBar.style.width = "0%";
            return;
        }

        const progress = (scrollTop / documentHeight) * 100;

        progressBar.style.width =
            Math.min(100, Math.max(0, progress)) + "%";

    }


    /* =====================================================
       BACK TO TOP
    ====================================================== */

    function updateBackToTop() {

        if (!backToTop) return;

        if (window.scrollY > 450) {
            backToTop.classList.add("show");
        } else {
            backToTop.classList.remove("show");
        }

    }

    if (backToTop) {

        backToTop.addEventListener("click", function () {

            window.scrollTo({
                top: 0,
                behavior: "smooth"
            });

        });

    }


    /* =====================================================
       OPTIMIZED SCROLL EVENT
    ====================================================== */

    let ticking = false;

    window.addEventListener("scroll", function () {

        if (ticking) return;

        window.requestAnimationFrame(function () {

            updateActiveSection();
            updateProgress();
            updateBackToTop();

            ticking = false;

        });

        ticking = true;

    }, { passive: true });


    /* =====================================================
       INITIALIZE
    ====================================================== */

    updateActiveSection();
    updateProgress();
    updateBackToTop();

});
