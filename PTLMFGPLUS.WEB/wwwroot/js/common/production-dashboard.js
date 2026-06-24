(function () {
    function bindDashboardCards() {
        document.querySelectorAll(".pm-kpi-card").forEach(card => {
            card.addEventListener("click", function () {
                document.querySelectorAll(".pm-kpi-card")
                    .forEach(x => x.classList.remove("active"));

                this.classList.add("active");
            });
        });
    }

    document.addEventListener("DOMContentLoaded", bindDashboardCards);
})();