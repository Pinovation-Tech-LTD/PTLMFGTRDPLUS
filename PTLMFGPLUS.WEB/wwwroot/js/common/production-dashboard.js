(function () {
    function bindDashboardCards() {
        document.querySelectorAll(".interface-kpi-card, .pm-kpi-card").forEach(card => {
            card.addEventListener("click", function () {
                document.querySelectorAll(".interface-kpi-card, .pm-kpi-card")
                    .forEach(function (item) {
                        item.classList.remove("active");
                        item.setAttribute("aria-pressed", "false");
                    });

                this.classList.add("active");
                this.setAttribute("aria-pressed", "true");
            });
        });
    }

    document.addEventListener("DOMContentLoaded", bindDashboardCards);
})();
