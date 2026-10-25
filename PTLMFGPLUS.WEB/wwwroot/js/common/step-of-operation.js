(function () {
    "use strict";

    const moduleSelector = $("#moduleSelector");
    const operationFilter = document.getElementById("operationFilter");
    const noResults = document.getElementById("operationNoResults");

    if (moduleSelector.length) {
        if (moduleSelector.hasClass("select2-hidden-accessible")) {
            moduleSelector.select2("destroy");
        }

        moduleSelector.select2({
            width: "100%",
            placeholder: "Select a module"
        });

        moduleSelector.on("change", function () {
            const moduleId = this.value;
            if (moduleId) {
                window.location.assign(`${appPath}StepOfOperation/${encodeURIComponent(moduleId)}`);
            }
        });
    }

    operationFilter?.addEventListener("input", function () {
        const term = operationFilter.value.trim().toLowerCase();
        let visibleItems = 0;

        document.querySelectorAll("[data-operation-section]").forEach(function (section) {
            let sectionItems = 0;

            section.querySelectorAll("[data-operation-link]").forEach(function (link) {
                const isVisible = !term || link.dataset.operationName.includes(term);
                link.hidden = !isVisible;
                sectionItems += isVisible ? 1 : 0;
                visibleItems += isVisible ? 1 : 0;
            });

            section.hidden = sectionItems === 0;
        });

        if (noResults) {
            noResults.hidden = visibleItems > 0;
        }
    });
    $("#div-footer").show();
})();
