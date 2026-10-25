const commonPath = "/F_11_Pro/ProdReq/";

var ProdReqPage = (function () {
    "use strict";

    const query = new URLSearchParams(window.location.search);
    const state = {
        requestType: query.get("Type") || query.get("type") || "Entry",
        generationNo: query.get("genno") || "",
        requestedBatch: query.get("actcode") || "",
        budgets: [],
        batches: [],
        previousDprs: [],
        products: [],
        materials: [],
        initializing: true,
        editing: false,
        selectedDprNo: ""
    };

    const service = {
        loadInitialData() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}GetInitialData`),
                    { type: state.requestType, genno: state.generationNo }
                );
            });
        },

        loadPreviousDprs(date, budgetNo) {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}GetPreviousDprs`),
                    { date: date, budgetNo: budgetNo }
                );
            });
        },

        openRequisition(date, budgetNo, dprNo) {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}OpenRequisition`),
                    {
                        type: state.requestType,
                        date: date,
                        budgetNo: budgetNo,
                        dprNo: dprNo
                    }
                );
            });
        },

        generateMaterials(budgetNo, products) {
            return Helpers.withLoader(function () {
                return FetchHelpers.postJson(
                    FetchHelpers.urlconfig(`${commonPath}GenerateMaterials`),
                    {
                        type: state.requestType,
                        budgetNo: budgetNo,
                        products: products
                    }
                );
            });
        },

        saveRequisition(request) {
            return Helpers.withLoader(function () {
                return FetchHelpers.postJson(
                    FetchHelpers.urlconfig(`${commonPath}Save`),
                    request
                );
            });
        }
    };

    const ui = Object.assign({}, EnterpriseUI.dom, {
        initializeDate() {
            const today = new Date();

            EnterpriseUI.dates.initializeFilters({
                txtReqDate: {
                    value: today,
                    datepicker: { endDate: today }
                }
            });
        },

        renderSelects(budgetNo, batchCode) {
            ui.loadSelect(
                "ddlBudget",
                state.budgets,
                "pbmno",
                "pbmno1",
                "Select production budget",
                budgetNo
            );
            ui.loadSelect(
                "ddlBatch",
                state.batches,
                "actcode",
                "actdesc",
                "Select batch",
                batchCode
            );
        },

        renderPreviousDprs(selectedValue) {
            ui.loadSelect(
                "ddlPreviousDpr",
                state.previousDprs,
                "preqno",
                "preqno1",
                "New DPR",
                selectedValue
            );
        },

        setDate(value) {
            const date = EnterpriseUI.dates.parse(value);

            if (date) {
                $("#txtReqDate").datepicker("setDate", date);
            }
        },

        setSelectValue(id, value) {
            $(`#${id}`)
                .val(value || "")
                .trigger("change.select2");
        },

        setHeaderDisabled(disabled) {
            ui.setDisabled(
                ["txtReqDate", "ddlBudget", "ddlBatch", "ddlPreviousDpr"],
                disabled
            );
        },

        showWorkspace(visible) {
            ui.setVisible("reqWorkspace", visible);
            document.getElementById("btnOpen").innerHTML = visible
                ? '<i class="bi bi-plus-lg me-1" aria-hidden="true"></i>New'
                : '<i class="bi bi-folder2-open me-1" aria-hidden="true"></i>Open';
        },

        number(value) {
            return EnterpriseUI.parseNumber(value);
        },

        formatNumber(value, decimals) {
            const precision = decimals ?? 0;

            return EnterpriseUI.formatNumber(value, {
                minimumFractionDigits: precision,
                maximumFractionDigits: precision,
                emptyValue: "0"
            });
        },

        renderProducts() {
            const tbody = document.querySelector("#productReqTable tbody");

            tbody.innerHTML = state.products.map(function (item, index) {
                const summary = String(item.rsircode || "")
                    .toUpperCase()
                    .endsWith("AAAA");

                return `
                    <tr class="${summary ? "summary-row" : ""}">
                        <td class="text-end">${index + 1}.</td>
                        <td class="description">${EnterpriseUI.encodeText(item.rsirdesc)}</td>
                        <td>${EnterpriseUI.encodeText(item.rsirunit)}</td>
                        <td class="text-end">${ui.formatNumber(item.bgdqty)}</td>
                        <td class="text-end">${ui.formatNumber(item.issqty)}</td>
                        <td class="text-end">${ui.formatNumber(item.bbqty)}</td>
                        <td>
                            <input type="text"                                  
                                   class="form-control ui-size-xs text-end product-req-qty w-100"
                                   data-index="${index}"
                                   value="${ui.number(item.cresqty)}"
                                   aria-label="Requisition quantity" />
                        </td>
                        <td class="text-center">
                            <button type="button"
                                    class="btn btn-link text-danger btn-sm delete-product-row"
                                    data-index="${index}"
                                    title="Remove product"
                                    aria-label="Remove product">
                                <i class="bi bi-trash" aria-hidden="true"></i>
                            </button>
                        </td>
                    </tr>
                `;
            }).join("");
        },

        renderMaterials() {
            const tbody = document.querySelector("#materialReqTable tbody");
            let group = null;
            let product = null;

            tbody.innerHTML = state.materials.map(function (item, index) {
                const groupText = item.grp === group ? "" : item.grpdesc;
                const productText = item.procode === product ? "" : item.prodesc;

                group = item.grp;
                product = item.procode;

                return `
                    <tr>
                        <td class="text-end">${index + 1}.</td>
                        <td class="description">
                            ${groupText ? `<strong>${EnterpriseUI.encodeText(groupText)}</strong>` : ""}
                            ${productText ? `<span class="product-name">${EnterpriseUI.encodeText(productText)}</span>` : ""}
                            <span class="resource-name">${EnterpriseUI.encodeText(item.rsirdesc)}</span>
                        </td>
                        <td class="description">${EnterpriseUI.encodeText(item.spcfdesc)}</td>
                        <td>${EnterpriseUI.encodeText(item.rsirunit)}</td>
                        <td class="text-end">${ui.formatNumber(item.uresqty, 4)}</td>
                        <td class="text-end">${ui.formatNumber(item.bgdqty, 4)}</td>
                        <td class="text-end">${ui.formatNumber(item.issqty, 4)}</td>
                        <td class="text-end">${ui.formatNumber(item.bbqty, 4)}</td>
                        <td>
                            <input type="number"
                                   min="0"
                                   step="0.000001"
                                   class="form-control ui-size-xs text-end material-req-qty"
                                   data-index="${index}"
                                   value="${ui.number(item.cresqty)}"
                                   aria-label="Material requisition quantity" />
                        </td>
                        <td class="text-end ${ui.number(item.stockqty) < 1 ? "stock-short" : ""}">${ui.formatNumber(item.stockqty, 4)}</td>
                        <td class="text-end">${ui.formatNumber(item.balqty, 4)}</td>
                    </tr>
                `;
            }).join("");
        },

        renderTotals() {
            function sum(field) {
                return state.products.reduce(function (total, item) {
                    return total + ui.number(item[field]);
                }, 0);
            }

            ui.setText("productBudgetTotal", ui.formatNumber(sum("bgdqty")));
            ui.setText("productIssuedTotal", ui.formatNumber(sum("issqty")));
            ui.setText("productBalanceTotal", ui.formatNumber(sum("bbqty")));
            ui.setText("productReqTotal", ui.formatNumber(sum("cresqty")));
        },

       
        renderWorkspace() {
            ui.renderProducts();
            ui.renderMaterials();
            ui.renderTotals();
        },

        showError(error, fallback) {
            Notifications.Toastr.error(
                error?.objError || error?.message || fallback || "The operation failed."
            );
        }
    });

    function hasLockedBatch() {
        return Boolean(
            state.requestedBatch
            && state.requestedBatch !== "000000000000"
        );
    }

    async function loadInitialData() {
        const result = await service.loadInitialData();

        state.budgets = result?.budgets || [];
        state.batches = result?.batches || [];

        const budgetNo = state.budgets.some(function (item) {
            return item.pbmno === state.generationNo;
        }) ? state.generationNo : state.budgets[0]?.pbmno;
        const batchCode = state.batches.some(function (item) {
            return item.actcode === state.requestedBatch;
        }) ? state.requestedBatch : state.batches[0]?.actcode;

        ui.renderSelects(budgetNo, batchCode);

        if (hasLockedBatch() && batchCode === state.requestedBatch) {
            ui.setDisabled("ddlBatch", true);
        }

        await loadPreviousDprs();
    }

    async function loadPreviousDprs() {
        const date = ui.getValue("txtReqDate");
        const budgetNo = ui.getValue("ddlBudget");

        if (!date || !budgetNo) {
            state.previousDprs = [];
            ui.renderPreviousDprs(null);
            return;
        }

        state.previousDprs = await service.loadPreviousDprs(date, budgetNo) || [];

        const selected = state.previousDprs.some(function (item) {
            return item.preqno === state.selectedDprNo;
        }) ? state.selectedDprNo : null;

        ui.renderPreviousDprs(selected);
    }

    function applyPreviousDpr() {
        state.selectedDprNo = ui.getValue("ddlPreviousDpr");

        const selected = state.previousDprs.find(function (item) {
            return item.preqno === state.selectedDprNo;
        });

        if (!selected) {
            ui.setValue("txtDprNo", "");
            return;
        }

        ui.setDate(selected.pbdate);
        ui.setSelectValue("ddlBudget", selected.pbmno);
        ui.setSelectValue("ddlBatch", selected.actcode);
        ui.setValue("txtDprNo", selected.preqno);
    }

    async function openRequisition() {
        if (state.editing) {
            resetPage();
            return;
        }

        const budgetNo = ui.getValue("ddlBudget");
        const batchCode = ui.getValue("ddlBatch");

        if (!budgetNo || !batchCode) {
            Notifications.Toastr.error(
                !budgetNo ? "Select a production budget." : "Select a batch number."
            );
            return;
        }

        const result = await service.openRequisition(
            ui.getValue("txtReqDate"),
            budgetNo,
            state.selectedDprNo || ui.getValue("ddlPreviousDpr")
        );

        state.products = result?.products || [];
        state.materials = result?.materials || [];
        state.editing = true;
        ui.setValue("txtDprNo", result?.dprNo);
        ui.setHeaderDisabled(true);
        ui.showWorkspace(true);
        ui.renderWorkspace();
    }

    function resetPage() {
        state.editing = false;
        state.products = [];
        state.materials = [];
        state.selectedDprNo = "";
        ui.setValue("txtDprNo", "");
        ui.showWorkspace(false);
        ui.setHeaderDisabled(false);
        ui.setSelectValue("ddlPreviousDpr", "");

        if (hasLockedBatch()) {
            ui.setDisabled("ddlBatch", true);
        }
    }

    function validateBalance() {
        const invalid = state.products.some(function (item) {
            return ui.number(item.cresqty) > ui.number(item.bbqty);
        });

        if (invalid) {
            Notifications.Toastr.error("Requisition quantity is larger than budget balance.");
        }

        return !invalid;
    }

    async function generateMaterials() {
        if (!validateBalance()) {
            return;
        }

        Helpers.setPending("#btnGenerate", true);

        try {
            const analysis = await service.generateMaterials(
                ui.getValue("ddlBudget"),
                state.products
            ) || [];
            const quantities = new Map(analysis.map(function (item) {
                return [
                    `${item.procode}|${item.rsircode}|${item.spcfcod}`,
                    ui.number(item.fresqty)
                ];
            }));

            state.materials.forEach(function (item) {
                const key = `${item.procode}|${item.rsircode}|${item.spcfcod}`;

                if (quantities.has(key)) {
                    item.cresqty = quantities.get(key);
                }
            });

            ui.renderMaterials();
            Notifications.Toastr.success("Material requisition generated successfully.");
        }
        catch (error) {
            ui.showError(error, "Material analysis could not be generated.");
        }
        finally {
            Helpers.setPending("#btnGenerate", false);
        }
    }

    async function saveRequisition() {
        if (!validateBalance()) {
            return;
        }

        Helpers.setPending("#btnFooterSave", true);

        try {
            await service.saveRequisition({
                type: state.requestType,
                date: ui.getValue("txtReqDate"),
                dprNo: ui.getValue("txtDprNo"),
                budgetNo: ui.getValue("ddlBudget"),
                batchCode: ui.getValue("ddlBatch"),
                products: state.products,
                materials: state.materials
            });

            Notifications.Toastr.success("Production requisition saved successfully.");

            const refreshed = await service.openRequisition(
                ui.getValue("txtReqDate"),
                ui.getValue("ddlBudget"),
                ui.getValue("txtDprNo")
            );

            state.products = refreshed?.products || [];
            state.materials = refreshed?.materials || [];
            ui.renderWorkspace();
        }
        catch (error) {
            ui.showError(error, "Production requisition could not be saved.");
        }
        finally {
            Helpers.setPending("#btnFooterSave", false);
        }
    }

    function moveProductFocus(input, direction) {
        const fields = Array.from(document.querySelectorAll(".product-req-qty"));
        const next = fields[fields.indexOf(input) + direction];

        if (next) {
            next.focus();
            next.select();
        }
    }

    function reportLoadError(error) {
        ui.showError(error, "Previous DPRs could not be loaded.");
    }

    function bindEvents() {
        $("#txtReqDate").on("changeDate", function () {
            $(this).datepicker("hide");

            if (!state.initializing && !state.editing) {
                loadPreviousDprs().catch(reportLoadError);
            }
        });

        $("#ddlBudget").on("change", function () {
            if (!state.initializing && !state.editing) {
                loadPreviousDprs().catch(reportLoadError);
            }
        });

        $("#ddlPreviousDpr").on("change", applyPreviousDpr);
        $("#btnOpen").on("click", function () {
            openRequisition().catch(function (error) {
                ui.showError(error, "Requisition could not be opened.");
            });
        });
        $("#btnGenerate").on("click", generateMaterials);
        $("#btnFooterRecalculate").on("click", ui.renderTotals);
        $("#btnFooterSave").on("click", saveRequisition);

        $(document).on("input", ".product-req-qty", function () {
            state.products[$(this).data("index")].cresqty = ui.number(this.value);
            ui.renderTotals();
        });
        $(document).on("input", ".material-req-qty", function () {
            state.materials[$(this).data("index")].cresqty = ui.number(this.value);
        });
        $(document).on("keydown", ".product-req-qty", function (event) {
            if (event.key === "Enter" || event.key === "ArrowDown") {
                event.preventDefault();
                moveProductFocus(this, 1);
            }
            else if (event.key === "ArrowUp") {
                event.preventDefault();
                moveProductFocus(this, -1);
            }
        });
        $(document).on("click", ".delete-product-row", function () {
            state.products.splice($(this).data("index"), 1);
            ui.renderProducts();
            ui.renderTotals();
        });
    }

    function init() {
        EnterpriseUI.initialize(document);
        ui.initializeDate();
        bindEvents();
        ShowFooterWithButtons();
        HideRefresh();
        HideCancel();

        loadInitialData()
            .catch(function (error) {
                ui.showError(error, "Production requisition setup could not be loaded.");
            })
            .finally(function () {
                state.initializing = false;
            });
    }

    return { init: init };
})();

document.addEventListener("DOMContentLoaded", ProdReqPage.init);
