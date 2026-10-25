(function () {
    "use strict";

    const root = document.documentElement;
    const themeButton = document.getElementById("themeToggle");
    const passwordButton = document.getElementById("passwordToggle");
    const passwordInput = document.getElementById("password");
    const loginForm = document.getElementById("loginForm");
    const submitButton = document.getElementById("loginSubmit");

    function applyTheme(theme) {
        root.setAttribute("data-theme", theme);

        if (themeButton) {
            themeButton.innerHTML = theme === "dark"
                ? '<i class="bi bi-sun-fill" aria-hidden="true"></i>'
                : '<i class="bi bi-moon-stars-fill" aria-hidden="true"></i>';
            themeButton.setAttribute("aria-label", theme === "dark" ? "Use light mode" : "Use dark mode");
        }
    }

    const savedTheme = localStorage.getItem("theme");
    const preferredTheme = window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light";
    applyTheme(savedTheme || preferredTheme);

    themeButton?.addEventListener("click", function () {
        const nextTheme = root.getAttribute("data-theme") === "dark" ? "light" : "dark";
        localStorage.setItem("theme", nextTheme);
        applyTheme(nextTheme);
    });

    passwordButton?.addEventListener("click", function () {
        const showPassword = passwordInput.type === "password";
        passwordInput.type = showPassword ? "text" : "password";
        passwordButton.innerHTML = showPassword
            ? '<i class="bi bi-eye-slash" aria-hidden="true"></i>'
            : '<i class="bi bi-eye" aria-hidden="true"></i>';
        passwordButton.setAttribute("aria-label", showPassword ? "Hide password" : "Show password");
        passwordInput.focus();
    });

    loginForm?.addEventListener("submit", function () {
        if (!loginForm.checkValidity()) {
            return;
        }

        submitButton.disabled = true;
        submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" aria-hidden="true"></span>Signing in...';
    });
})();
