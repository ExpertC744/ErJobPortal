/* ==========================================================================
   home.js  —  wwwroot/js/home.js
   Hero carousel, scroll-reveal and animated counters for the Home page.
   No dependencies.
   ========================================================================== */
(function () {
	"use strict";

	var prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

	/* ======================================================================
	   1. HERO CAROUSEL
	   - Autoplay uses a real timer (does not depend on CSS animation events)
	   - Pauses on mouse hover (desktop), keyboard focus and hidden tab
	   - Arrows, dots, keyboard arrows and touch swipe all work
	   ====================================================================== */
	function initHeroSlider() {
		var slider = document.querySelector(".modern-hero-slider");
		if (!slider) return;

		var slides = slider.querySelectorAll(".hero-slide");
		var dots = slider.querySelectorAll(".hero-dot");
		var nextBtn = slider.querySelector(".hero-next");
		var prevBtn = slider.querySelector(".hero-prev");
		var currentLabel = slider.querySelector(".hero-current");

		if (slides.length < 2) return;

		var AUTOPLAY_MS = 5500;      // keep equal to the dot animation (5.5s) in CSS
		var current = 0;
		var timer = null;
		var paused = false;
		var autoplay = !prefersReducedMotion;

		if (autoplay) slider.classList.add("is-autoplay");

		function clearTimer() {
			if (timer) {
				window.clearTimeout(timer);
				timer = null;
			}
		}

		function startTimer() {
			clearTimer();
			if (!autoplay || paused) return;

			timer = window.setTimeout(function () {
				show(current + 1);
			}, AUTOPLAY_MS);
		}

		function restartDotProgress() {
			var bar = dots[current] && dots[current].querySelector("span");
			if (!bar) return;

			bar.style.animation = "none";
			void bar.offsetWidth; // force reflow so the animation restarts
			bar.style.animation = "";
		}

		function show(index) {
			index = (index + slides.length) % slides.length;

			slides.forEach(function (slide, i) {
				var active = i === index;
				slide.classList.toggle("active", active);
				slide.setAttribute("aria-hidden", active ? "false" : "true");
			});

			dots.forEach(function (dot, i) {
				var active = i === index;
				dot.classList.toggle("active", active);
				dot.setAttribute("aria-current", active ? "true" : "false");
			});

			current = index;

			if (currentLabel) {
				currentLabel.textContent = String(index + 1).padStart(2, "0");
			}

			restartDotProgress();
			startTimer();
		}

		function setPaused(value) {
			if (paused === value) return;
			paused = value;
			slider.classList.toggle("is-paused", value);

			if (value) {
				clearTimer();
			} else {
				// Resume with a fresh full interval and a fresh progress bar
				restartDotProgress();
				startTimer();
			}
		}

		/* ---- controls ---- */
		if (nextBtn) nextBtn.addEventListener("click", function () { show(current + 1); });
		if (prevBtn) prevBtn.addEventListener("click", function () { show(current - 1); });

		dots.forEach(function (dot, i) {
			dot.addEventListener("click", function () { show(i); });
		});

		/* ---- pause: mouse hover (only on devices that really hover) ---- */
		if (window.matchMedia("(hover: hover)").matches) {
			slider.addEventListener("mouseenter", function () { setPaused(true); });
			slider.addEventListener("mouseleave", function () { setPaused(false); });
		}

		/* ---- pause: keyboard focus only (clicking an arrow must NOT pause) ---- */
		slider.addEventListener("focusin", function (event) {
			var el = event.target;
			if (el.matches && el.matches(":focus-visible")) setPaused(true);
		});
		slider.addEventListener("focusout", function () { setPaused(false); });

		/* ---- pause: hidden tab ---- */
		document.addEventListener("visibilitychange", function () {
			setPaused(document.hidden);
		});

		/* ---- keyboard arrows (only when focus is inside the slider) ---- */
		slider.addEventListener("keydown", function (event) {
			var tag = (event.target.tagName || "").toLowerCase();
			if (tag === "input" || tag === "textarea") return;

			if (event.key === "ArrowRight") show(current + 1);
			if (event.key === "ArrowLeft") show(current - 1);
		});

		/* ---- touch swipe: left = next, right = previous ---- */
		var startX = 0;

		slider.addEventListener("touchstart", function (event) {
			startX = event.changedTouches[0].screenX;
		}, { passive: true });

		slider.addEventListener("touchend", function (event) {
			var difference = startX - event.changedTouches[0].screenX;
			if (Math.abs(difference) < 50) return;

			show(difference > 0 ? current + 1 : current - 1);
		}, { passive: true });

		show(0);
	}

	/* ======================================================================
	   2. SCROLL REVEAL
	   ====================================================================== */
	function initReveal() {
		var items = document.querySelectorAll(".reveal");
		if (!items.length) return;

		if (prefersReducedMotion || !("IntersectionObserver" in window)) {
			items.forEach(function (el) { el.classList.add("is-visible"); });
			return;
		}

		// Only hide elements once JS is able to reveal them again.
		document.documentElement.classList.add("reveal-ready");

		var observer = new IntersectionObserver(function (entries) {
			entries.forEach(function (entry) {
				if (!entry.isIntersecting) return;
				entry.target.classList.add("is-visible");
				observer.unobserve(entry.target);
			});
		}, { threshold: 0.12, rootMargin: "0px 0px -6% 0px" });

		items.forEach(function (el) { observer.observe(el); });
	}

	/* ======================================================================
	   3. COUNTERS (start when the stats scroll into view, run once)
	   ====================================================================== */
	function animateCounter(el, delay) {
		var target = Number(el.getAttribute("data-target")) || 0;

		if (target <= 0 || prefersReducedMotion) {
			el.textContent = target.toLocaleString();
			return;
		}

		var duration = 2200;

		window.setTimeout(function () {
			var start = performance.now();

			function tick(now) {
				var progress = Math.min((now - start) / duration, 1);
				var eased = 1 - Math.pow(1 - progress, 4); // ease-out quart

				el.textContent = Math.floor(target * eased).toLocaleString();

				if (progress < 1) {
					requestAnimationFrame(tick);
					return;
				}

				el.textContent = target.toLocaleString();

				if (el.animate) {
					el.animate(
						[{ transform: "scale(1)" }, { transform: "scale(1.15)" }, { transform: "scale(1)" }],
						{ duration: 450, easing: "ease-out" }
					);
				}
			}

			requestAnimationFrame(tick);
		}, delay);
	}

	function initCounters() {
		var counters = document.querySelectorAll(".counter");
		if (!counters.length) return;

		var dashboard = document.querySelector(".platform-dashboard");

		function run() {
			counters.forEach(function (counter, index) {
				animateCounter(counter, index * 180);
			});
			if (dashboard) dashboard.classList.add("counting");
		}

		if (!dashboard || !("IntersectionObserver" in window)) {
			run();
			return;
		}

		var observer = new IntersectionObserver(function (entries) {
			if (entries[0].isIntersecting) {
				observer.disconnect();
				run();
			}
		}, { threshold: 0.25 });

		observer.observe(dashboard);
	}

	/* ======================================================================
	   INIT — each block is isolated so one failure never stops the others
	   ====================================================================== */
	function safely(fn) {
		try { fn(); } catch (e) { if (window.console) console.error("[home-index]", e); }
	}

	function init() {
		safely(initHeroSlider);
		safely(initReveal);
		safely(initCounters);
	}

	if (document.readyState === "loading") {
		document.addEventListener("DOMContentLoaded", init);
	} else {
		init();
	}
})();