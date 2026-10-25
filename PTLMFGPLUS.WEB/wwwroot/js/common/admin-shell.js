(function () {
    "use strict";

    const body = document.body;
    const sidebarToggle = document.getElementById("sidebarToggle");
    const sidebarBackdrop = document.getElementById("sidebarBackdrop");
    const themeToggle = document.getElementById("themeToggle");
    const profileButton = document.getElementById("profileButton");
    const profileMenu = document.getElementById("profileMenu");
    const reportFormat = document.getElementById("DDPrintOpt");
    const printButton = document.getElementById("lnkPrint");

    function isMobile() {
        return window.matchMedia("(max-width: 991.98px)").matches;
    }

    function applyTheme(theme) {
        document.documentElement.setAttribute("data-theme", theme);
        localStorage.setItem("theme", theme);

        if (themeToggle) {
            const isDark = theme === "dark";
            themeToggle.innerHTML = isDark
                ? '<i class="bi bi-sun-fill" aria-hidden="true"></i>'
                : '<i class="bi bi-moon-stars-fill" aria-hidden="true"></i>';
            themeToggle.setAttribute("aria-label", isDark ? "Use light mode" : "Use dark mode");
        }
    }

    function closeMobileSidebar() {
        body.classList.remove("sidebar-mobile-open");
    }

    sidebarToggle?.addEventListener("click", function () {
        if (isMobile()) {
            body.classList.toggle("sidebar-mobile-open");
            return;
        }

        const willCollapse = !body.classList.contains("sidebar-collapsed");
        if (willCollapse) {
            document.querySelectorAll(".nav-group.open").forEach(function (group) {
                group.classList.remove("open");
                group.querySelector(".nav-group-toggle")?.setAttribute("aria-expanded", "false");
            });
        }

        body.classList.toggle("sidebar-collapsed", willCollapse);
        localStorage.setItem("sidebar-collapsed", willCollapse.toString());
    });

    sidebarBackdrop?.addEventListener("click", closeMobileSidebar);

    document.querySelectorAll(".nav-group-toggle").forEach(function (button) {
        button.addEventListener("click", function () {
            const group = button.closest(".nav-group");
            const isOpen = group.classList.toggle("open");
            button.setAttribute("aria-expanded", isOpen.toString());
        });
    });

    document.querySelectorAll(".nav-child-link, .nav-item-link").forEach(function (link) {
        link.addEventListener("click", closeMobileSidebar);
    });

    themeToggle?.addEventListener("click", function () {
        const currentTheme = document.documentElement.getAttribute("data-theme") || "light";
        applyTheme(currentTheme === "dark" ? "light" : "dark");
    });

    printButton?.addEventListener("click", function () {
        const format = reportFormat?.value || "PDF";
        const printEvent = new CustomEvent("enterprise:print", {
            cancelable: true,
            detail: {
                format: format
            }
        });

        document.dispatchEvent(printEvent);
        if (!printEvent.defaultPrevented) {
            window.print();
        }
    });

    window.enterpriseReport = {
        getFormat: function () {
            return reportFormat?.value || "PDF";
        }
    };

    profileButton?.addEventListener("click", function (event) {
        event.stopPropagation();
        const isOpen = profileMenu.classList.toggle("show");
        profileButton.setAttribute("aria-expanded", isOpen.toString());
    });

    document.addEventListener("click", function (event) {
        if (!profileMenu?.contains(event.target)) {
            profileMenu?.classList.remove("show");
            profileButton?.setAttribute("aria-expanded", "false");
        }
    });

    if (!isMobile() && localStorage.getItem("sidebar-collapsed") === "true") {
        body.classList.add("sidebar-collapsed");
    }

    applyTheme(localStorage.getItem("theme") || "light");
})();
