
/* =========================================================
   1TRAINEE - INTERNSHIP TIPS SIDEBAR NAVIGATION
   File: wwwroot/js/Tips.js

   Features:
   - Scroll to the complete section heading
   - Keep headings visible below a fixed header
   - Highlight the active sidebar link
   - Support smooth scrolling and browser history
========================================================= */

document.addEventListener("DOMContentLoaded", function () {
    const navigation = document.querySelector(".tips-quick-nav");

    if (!navigation) return;

    const links = Array.from(
        navigation.querySelectorAll('a[href^="#"]')
    );

    const sections = links
        .map(function (link) {
            const id = link.getAttribute("href").slice(1);
            return document.getElementById(id);
        })
        .filter(Boolean);

    if (!links.length || !sections.length) return;

    let scrollLocked = false;
    let unlockTimer = null;
    let ticking = false;

    /*
       Read the scroll offset from Tips.css.
       Desktop: 100px
       Mobile: 82px
    */
    function getScrollOffset() {
        const value = getComputedStyle(document.documentElement)
            .getPropertyValue("--tips-scroll-offset")
            .trim();

        return (parseFloat(value) || 100) + 8;
    }

    function setActiveLink(sectionId) {
        links.forEach(function (link) {
            const isActive =
                link.getAttribute("href") === "#" + sectionId;

            link.classList.toggle("active", isActive);

            if (isActive) {
                link.setAttribute("aria-current", "location");
            } else {
                link.removeAttribute("aria-current");
            }
        });

        sections.forEach(function (section) {
            section.classList.toggle(
                "tips-section-active",
                section.id === sectionId
            );
        });
    }

    /*
       Scroll to the section heading, rather than simply
       placing the section at the very top of the viewport.
    */
    function scrollToSection(section, updateHistory) {
        const heading =
            section.querySelector(".tips-section-heading") || section;

        const targetTop =
            heading.getBoundingClientRect().top +
            window.scrollY -
            getScrollOffset();

        scrollLocked = true;
        window.clearTimeout(unlockTimer);

        setActiveLink(section.id);

        if (updateHistory) {
            history.replaceState(
                null,
                "",
                "#" + section.id
            );
        }

        const reducedMotion = window.matchMedia(
            "(prefers-reduced-motion: reduce)"
        ).matches;

        window.scrollTo({
            top: Math.max(0, targetTop),
            behavior: reducedMotion ? "auto" : "smooth"
        });

        /*
           Release the active-section tracking lock after
           smooth scrolling has had time to finish.
        */
        unlockTimer = window.setTimeout(function () {
            scrollLocked = false;
            updateActiveSection();
        }, reducedMotion ? 50 : 1000);
    }

    /* SIDEBAR CLICK */

    links.forEach(function (link) {
        link.addEventListener("click", function (event) {
            const targetId = link.getAttribute("href").slice(1);
            const targetSection = document.getElementById(targetId);

            if (!targetSection) return;

            event.preventDefault();
            scrollToSection(targetSection, true);
        });
    });

    /* ACTIVE LINK WHILE SCROLLING */

    function updateActiveSection() {
        if (scrollLocked) return;

        const activationLine = getScrollOffset() + 25;
        let currentSection = sections[0];

        sections.forEach(function (section) {
            const rect = section.getBoundingClientRect();

            if (rect.top <= activationLine) {
                currentSection = section;
            }
        });

        /*
           Keep the final section active near the bottom
           of the article, even if the page cannot scroll
           any farther.
        */
        const pageBottom =
            window.innerHeight + window.scrollY >=
            document.documentElement.scrollHeight - 5;

        if (pageBottom) {
            currentSection = sections[sections.length - 1];
        }

        if (currentSection) {
            setActiveLink(currentSection.id);
        }
    }

    window.addEventListener("scroll", function () {
        if (ticking) return;

        ticking = true;

        window.requestAnimationFrame(function () {
            updateActiveSection();
            ticking = false;
        });
    }, { passive: true });

    window.addEventListener("resize", updateActiveSection);

    /* SUPPORT DIRECT LINKS SUCH AS /Tips#seek-feedback */

    function handleInitialHash() {
        const targetId = window.location.hash.slice(1);

        if (!targetId) {
            updateActiveSection();
            return;
        }

        const targetSection = document.getElementById(targetId);

        if (targetSection && sections.includes(targetSection)) {
            window.requestAnimationFrame(function () {
                scrollToSection(targetSection, false);
            });
        } else {
            updateActiveSection();
        }
    }

    handleInitialHash();
});
