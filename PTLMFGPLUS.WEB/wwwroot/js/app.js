// ================================
// SIDEBAR COLLAPSE
// ================================
const sidebarToggle = document.getElementById("sidebarToggle");
const sidebar = document.getElementById("sidebar");

sidebarToggle.addEventListener("click", () => {

    sidebar.classList.toggle("collapsed");

    document.body.classList.toggle("sidebar-collapsed");
});


// ================================
// SUBMENU TOGGLE
// ================================
const submenuToggles = document.querySelectorAll(".submenu-toggle");

submenuToggles.forEach(toggle => {

    toggle.addEventListener("click", function () {

        const parent = this.parentElement;

        parent.classList.toggle("open");
    });

});


// ================================
// PROFILE DROPDOWN
// ================================
const profileBtn = document.getElementById("profileBtn");
const profileMenu = document.getElementById("profileMenu");

profileBtn.addEventListener("click", function (e) {

    e.stopPropagation();

    profileMenu.classList.toggle("show");
});


// CLOSE DROPDOWN WHEN CLICK OUTSIDE
document.addEventListener("click", function () {

    profileMenu.classList.remove("show");
});