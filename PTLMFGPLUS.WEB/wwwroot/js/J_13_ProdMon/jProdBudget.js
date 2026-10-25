const commonPath = "/F_13_ProdMon/ProdBudget/";

var ProdBudgetPage = (function () {
    "use strict";

    const entryType = new URLSearchParams(window.location.search).get("type") || "Entry";

    const state = {
        previousBudgets: [],
        products: [],
        budget: [],
        log: [],
        resources: [],
        processes: [],
        report: [],
        engineCodes: []
    };

    const service = {
        loadPreviousBudgets() {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetPreviousBudgetList`),
                { type: entryType },
                false
            );
        },

        generateBudgetNumber(date) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetProdBudgetNo`),
                { date: date },
                false
            );
        },

        loadProducts(date) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetProductList`),
                { type: entryType, date: date },
                false
            );
        },

        loadBudget(batchNumber, date) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetShowProductList`),
                { type: entryType, batchno: batchNumber, date: date },
                false
            );
        },

        deleteProduct(batchNumber, productCode) {
            return FetchHelpers.postForm(
                FetchHelpers.urlconfig(`${commonPath}DeleteProduct`),
                { batchno: batchNumber, productCode: productCode },
                false
            );
        },

        saveBudget(request) {
            return FetchHelpers.postJson(
                FetchHelpers.urlconfig(`${commonPath}FinalUpdateButtonClick`),
                request,
                false
            );
        },

        loadMaterials(batchNumber, date) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetBudgetResources`),
                { batchno: batchNumber, date: date },
                false
            );
        },

        saveMaterials(request) {
            return FetchHelpers.postJson(
                FetchHelpers.urlconfig(`${commonPath}SaveBudgetResources`),
                request,
                false
            );
        },

        loadReportProducts(batchNumber) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetReportProducts`),
                { batchno: batchNumber },
                false
            );
        },

        loadReport(batchNumber, productCode) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetBudgetReport`),
                { batchno: batchNumber, productCode: productCode },
                false
            );
        },

        uploadImei(form) {
            return FetchHelpers.request(
                "POST",
                FetchHelpers.urlconfig(`${commonPath}UploadImei`),
                form,
                null,
                false
            );
        },

        removeImei(batchNumber) {
            return FetchHelpers.postForm(
                FetchHelpers.urlconfig(`${commonPath}RemoveImei`),
                { batchno: batchNumber },
                false
            );
        },

        loadEngineCodes(batchNumber, assigned) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetEngineCodes`),
                { batchno: batchNumber, assigned: assigned },
                false
            );
        },

        assignEngineCodes(request) {
            return FetchHelpers.postJson(
                FetchHelpers.urlconfig(`${commonPath}AssignEngineCodes`),
                request,
                false
            );
        }
    };

    const ui = Object.assign({}, EnterpriseUI.dom, {
        getBatchNumber() {
            return ui.getValue("txtBpn").replaceAll("-", "");
        },

        formatBudgetNumber(value) {
            const text = String(value || "");

            return text.length > 9
                ? `${text.slice(0, 9)}-${text.slice(9)}`
                : text;
        },

        initializeDates() {
            const today = new Date();

            EnterpriseUI.dates.initializeFilters([
                { txtbgddate: { value: 0, datepicker: { endDate: today } } },
                { txtfrmdate: { value: 0 } },
                { txttodate: { value: 0 } }
            ]);
        },

        applyPreviousBudget(budget) {
            if (!budget) {
                return false;
            }

            $("#txtbgddate").datepicker("setDate", EnterpriseUI.dates.parse(budget.bgddat));
            $("#txtfrmdate").datepicker("setDate", EnterpriseUI.dates.parse(budget.sdate));
            $("#txttodate").datepicker("setDate", EnterpriseUI.dates.parse(budget.edate));
            ui.setValue("txtBpn", ui.formatBudgetNumber(budget.pbmno));
            return true;
        },

        showWorkspace(editing) {
            ui.setVisible("budgetWorkspace", true);
            ui.setText("btnOk", "New");
            ui.setDisabled(
                ["txtbgddate", "txtfrmdate", "txttodate", "ddlPreProlist"],
                true
            );
            ui.setVisible("previousBudget", editing);
        },


        resetStateAndUI() {
            state.budget = [];
            state.products = [];
            state.log = [];
            state.resources = [];
            state.processes = [];
            state.report = [];
            state.engineCodes = [];
            ui.renderBudget();
            ui.renderImeiCounter();
            ui.renderResources();
            ui.renderProcesses();
            ui.renderReport();
        },

        resetWorkspace() {
            ui.setVisible("budgetWorkspace", false);
            ui.setVisible("previousBudget", true);
            ui.setText("btnOk", "Open");
            ui.setDisabled(
                ["txtbgddate", "txtfrmdate", "txttodate", "ddlPreProlist"],
                false
            );
            ui.setValue("txtBpn", "");
            ui.setValue("txtBatchName", "");
            ui.setValue("ddlPreProlist", "");
            ui.resetStateAndUI();
            document.querySelector("[data-budget-tab].active")?.classList.remove("active");
            document.querySelectorAll(".budget-panel").forEach(function (panel) {
                panel.style.display = "none";
            });
            $("#ddlPreProlist").trigger("change.select2");
        },




        renderProductLookups() {
            const groups = Array.from(
                new Map(
                    state.products.map(function (product) {
                        const groupCode = product.rescod1;
                        return [groupCode, {
                            code: groupCode,
                            text: product.resdesc1
                        }];
                    })
                ).values()
            );

            ui.loadSelect("ddlGroupList", groups, "code", "text", "Select group");

            if (groups.length) {
                $("#ddlGroupList")
                    .val(groups[0].code)
                    .trigger("change");
            }
        },

        renderProductOptions() {
            const group = ui.getValue("ddlGroupList");
            const products = state.products.filter(function (product) {
                return product.rescod1 === group;
            });

            ui.loadSelect(
                "ddlProductList",
                products,
                "prodcode",
                "proddesc",
                "Select product",
                null
            );
        },

        renderBudget() {
            const body = document.querySelector("#productBudgetTable tbody");

            if (!body) {
                return;
            }

            const hasRows = state.budget.length > 0;

            body.innerHTML = "";
            ui.setText("bgdwqtyTotal", "0");
            ui.setVisible("productBudgetDiv", hasRows);

            if (!hasRows) {
                return;
            }

            body.innerHTML = state.budget.map(function (item, index) {
                return `
                    <tr>
                        <td class="text-end">${index + 1}.</td>
                        <td>
                            <button type="button"
                                    class="btn btn-link text-danger btn-sm delete-product"
                                    data-index="${index}"
                                    title="Delete product"
                                    aria-label="Delete product">
                                <i class="bi bi-trash" aria-hidden="true"></i>
                            </button>
                        </td>
                        <td class="wrap-cell">${EnterpriseUI.encodeText(item.proddesc1)}</td>
                        <td class="wrap-cell">${EnterpriseUI.encodeText(item.proddesc)}</td>
                        <td>${EnterpriseUI.encodeText(item.produnit)}</td>
                        <td class="text-end">${EnterpriseUI.formatNumber(item.stqty)}</td>
                        <td>
                            <input class="form-control form-control-sm budget-qty text-end w-100"
                                   type="number"
                                   min="0"
                                   step="1"
                                   data-index="${index}"
                                   value="${EnterpriseUI.parseNumber(item.bgdwqty)}" />
                        </td>
                    </tr>
                `;
            }).join("");

            ui.renderBudgetTotal();
        },

        renderBudgetTotal() {
            const total = state.budget.reduce(function (sum, item) {
                return sum + EnterpriseUI.parseNumber(item.bgdwqty);
            }, 0);

            ui.setText("bgdwqtyTotal", EnterpriseUI.formatNumber(total));
        },

        renderImeiCounter() {
            const count = state.log[0]?.imei;

            ui.setText("btnImeiCounter", count ? `COUNTER: ${count}` : "COUNTER");
        },

        renderResources() {
            const body = document.querySelector("#resourceTable tbody");
            let previousOrder = null;

            if (!body) {
                return;
            }

            body.innerHTML = state.resources.map(function (item, index) {
                const orderText = item.odcode === previousOrder ? "" : item.oddesc;
                const selectable = String(item.rsircode || "").startsWith("0101999");

                previousOrder = item.odcode;

                return `
                    <tr>
                        <td class="text-end">${index + 1}</td>
                        <td>${EnterpriseUI.encodeText(item.scode)}</td>
                        <td class="wrap-cell">
                            ${orderText ? `<strong>${EnterpriseUI.encodeText(orderText)}</strong><br />` : ""}
                            <span class="resource-item-text">${EnterpriseUI.encodeText(item.rsirdesc)}</span>
                        </td>
                        <td class="wrap-cell">${EnterpriseUI.encodeText(item.spcfdesc)}</td>
                        <td>${EnterpriseUI.encodeText(item.rsirunit)}</td>
                        <td class="text-end ${EnterpriseUI.parseNumber(item.stockqty) < 1 ? "stock-short" : ""}">
                            ${EnterpriseUI.formatNumber(item.stockqty, { minimumFractionDigits: 3, maximumFractionDigits: 3 })}
                        </td>
                        <td class="text-end">${EnterpriseUI.formatNumber(item.bgdqty, { maximumFractionDigits: 6 })}</td>
                        <td>
                            <input type="text"                                  
                                   class="form-control form-control-sm resource-rate text-end"
                                   data-index="${index}"
                                   value="${EnterpriseUI.parseNumber(item.bgdrat).toFixed(4)}" />
                        </td>
                        <td class="resource-amount text-end"></td>
                        <td class="text-center">
                            ${selectable
                        ? `<input type="checkbox" class="form-check-input resource-select" data-index="${index}" ${item.selected ? "checked" : ""} />`
                        : ""}
                        </td>
                    </tr>
                `;
            }).join("");

            ui.renderResourceTotals();
        },

        renderProcesses() {
            const body = document.querySelector("#processTable tbody");

            if (!body) {
                return;
            }
            console.log(state.processes);
            body.innerHTML = state.processes.map(function (item, index) {
                return `
                    <tr>
                        <td class="text-end">${index + 1}</td>
                        <td class="text-center">
                            <input type="checkbox"
                                   class="form-check-input process-active"
                                   data-index="${index}"
                                   ${item.active ? "checked" : ""} />
                        </td>
                        <td class="wrap-cell">${EnterpriseUI.encodeText(item.processdesc)}</td>
                    </tr>
                `;
            }).join("");
        },

        renderResourceTotals() {
            state.resources.forEach(function (item) {
                item.bgdam = EnterpriseUI.parseNumber(item.bgdqty)
                    * EnterpriseUI.parseNumber(item.bgdrat);
            });

            const total = state.resources.reduce(function (sum, item) {
                return sum + EnterpriseUI.parseNumber(item.bgdam);
            }, 0);

            document.querySelectorAll(".resource-amount").forEach(function (element, index) {
                element.textContent = EnterpriseUI.formatNumber(
                    state.resources[index]?.bgdam,
                    { minimumFractionDigits: 3, maximumFractionDigits: 3 }
                );
            });

            ui.setText(
                "resourceAmountTotal",
                EnterpriseUI.formatNumber(total, {
                    minimumFractionDigits: 3,
                    maximumFractionDigits: 3
                })
            );
        },

        renderReport() {
            const body = document.querySelector("#budgetReportTable tbody");
            let previousGroup = null;
            let previousProduct = null;

            if (!body) {
                return;
            }

            body.innerHTML = state.report.map(function (item, index) {
                const group = item.grp === previousGroup ? "" : item.grpdesc;
                const product = item.procode === previousProduct ? "" : item.prodesc;
                const summaryLabels = ["total", "grand total", "gross amount", "gross profit %"];
                const labels = [item.grpdesc, item.prodesc, item.rsirdesc, item.spcfdesc]
                    .map(function (label) {
                        return String(label || "").trim().toLowerCase();
                    });
                const summary = String(item.rsircode || "").toUpperCase().endsWith("AAAA")
                    || labels.some(function (label) {
                        return summaryLabels.includes(label);
                    });

                previousGroup = item.grp;
                previousProduct = item.procode;

                return `
                    <tr class="${summary ? "report-summary-row" : ""}">
                        <td class="text-end">${index + 1}</td>
                        <td class="wrap-cell">
                            ${group ? `<strong>${EnterpriseUI.encodeText(group)}</strong><br />` : ""}
                            ${product ? `<strong>${EnterpriseUI.encodeText(product)}</strong><br />` : ""}
                            ${EnterpriseUI.encodeText(item.rsirdesc)}
                        </td>
                        <td class="wrap-cell">${EnterpriseUI.encodeText(item.spcfdesc)}</td>
                        <td>${EnterpriseUI.encodeText(item.rsirunit)}</td>
                        <td class="text-end">${EnterpriseUI.formatNumber(item.bgdqty, { maximumFractionDigits: 6 })}</td>
                        <td class="text-end">${EnterpriseUI.formatNumber(item.bgdrate, { maximumFractionDigits: 6 })}</td>
                        <td class="text-end">${EnterpriseUI.formatNumber(item.bgdam, { minimumFractionDigits: 3, maximumFractionDigits: 3 })}</td>
                        <td class="text-end">${EnterpriseUI.formatNumber(item.percnt, { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</td>
                    </tr>
                `;
            }).join("");
        },

        renderEngineCodes(assigned) {
            const body = document.querySelector("#engineCodeTable tbody");

            if (!body) {
                return;
            }

            body.innerHTML = state.engineCodes.map(function (item, index) {
                return `
                    <tr>
                        <td class="text-end">${index + 1}</td>
                        <td class="wrap-cell">${EnterpriseUI.encodeText(item.resdesc)}</td>
                        <td>${EnterpriseUI.encodeText(item.chassis)}</td>
                        <td>${EnterpriseUI.encodeText(item.engine)}</td>
                        <td>${EnterpriseUI.encodeText(item.slnum)}</td>
                        <td class="text-end">${EnterpriseUI.formatNumber(item.items)}</td>
                        <td class="text-center">
                            ${assigned ? "" : `<input type="checkbox" class="form-check-input engine-select" data-index="${index}" />`}
                        </td>
                    </tr>
                `;
            }).join("");

            const total = state.engineCodes.reduce(function (sum, item) {
                return sum + EnterpriseUI.parseNumber(item.items);
            }, 0);

            ui.setText("engineQtyTotal", EnterpriseUI.formatNumber(total));
            ui.setVisible("btnAssignEngineCodes", !assigned);
            bootstrap.Modal
                .getOrCreateInstance(document.getElementById("engineCodeModal"))
                .show();
        },

        activateTab(name) {
            document.querySelectorAll("[data-budget-tab]").forEach(function (tab) {
                tab.classList.toggle("active", tab.dataset.budgetTab === name);
            });

            document.querySelectorAll(".budget-panel").forEach(function (panel) {
                panel.style.display = "none";
            });

            ui.setVisible(`${name}Panel`, true);
        },

        printTable(tableId, title) {
            const table = document.getElementById(tableId);

            if (!table || !table.tBodies[0]?.rows.length) {
                Notifications.Toastr.warning("Nothing to print", "There is no data to print.");
                return;
            }

            const popup = window.open("", "_blank", "width=1200,height=800");

            if (!popup) {
                Notifications.Toastr.warning("Pop-up blocked", "Allow pop-ups to print this table.");
                return;
            }

            popup.document.write(`
                <!doctype html>
                <html lang="en">
                    <head>
                        <title>${EnterpriseUI.encodeText(title)}</title>
                        <style>
                            body { font-family: Arial, sans-serif; padding: 20px; }
                            h2 { font-size: 18px; }
                            p { font-size: 12px; }
                            table { width: 100%; border-collapse: collapse; font-size: 10px; }
                            th, td { border: 1px solid #777; padding: 4px; }
                            .report-summary-row td { font-weight: 700; }
                            input, button { display: none; }
                        </style>
                    </head>
                    <body>
                        <h2>${EnterpriseUI.encodeText(title)}</h2>
                        <p>
                            Budget No: ${EnterpriseUI.encodeText(ui.getValue("txtBpn"))}
                            &nbsp; Date: ${EnterpriseUI.encodeText(ui.getValue("txtbgddate"))}
                        </p>
                        ${table.outerHTML}
                    </body>
                </html>
            `);
            popup.document.close();
            popup.focus();
            popup.print();
        }
    });

    async function loadPreviousBudgets() {
        await Helpers.executeRequest({
            fallbackMessage: "Previous budgets could not be loaded.",
            action: service.loadPreviousBudgets,
            onSuccess: function (items) {
                state.previousBudgets = items || [];
                ui.loadSelect(
                    "ddlPreProlist",
                    state.previousBudgets,
                    "pbmno",
                    "pbmno1",
                    "New budget"
                );
            }
        });
    }

    function applyPreviousBudget() {
        const selected = state.previousBudgets.find(function (budget) {
            return budget.pbmno === ui.getValue("ddlPreProlist");
        });

        return ui.applyPreviousBudget(selected);
    }

    async function openBudget() {
        const workspace = document.getElementById("budgetWorkspace");

        if (workspace?.style.display !== "none") {
            ui.resetWorkspace();
            return;
        }

        await Helpers.executeRequest({
            pending: "#btnOk",
            fallbackMessage: "Budget could not be opened.",
            action: async function () {
                ui.showWorkspace();
                const editing = applyPreviousBudget();
                if (!editing) {
                    const numbers = await service.generateBudgetNumber(
                        ui.getValue("txtbgddate")
                    );

                    if (!numbers?.length) {
                        throw new Error("Budget number could not be generated.");
                    }

                    ui.setValue("txtBpn", ui.formatBudgetNumber(numbers[0].bpno));
                }
                //ui.activateTab("products");
            }
        });
    }

    //function resetWorkspace() {
    //    state.budget = [];
    //    state.log = [];
    //    state.resources = [];
    //    state.processes = [];
    //    state.report = [];
    //    ui.resetWorkspace();
    //}

    async function loadProductSelection(showLoader) {
        const action = async function () {
            return Promise.all([
                service.loadProducts(ui.getValue("txtbgddate")),
                service.loadBudget(ui.getBatchNumber(), ui.getValue("txtbgddate"))
            ]);
        };
        const applyResult = function (result) {
            state.products = result[0] || [];
            state.budget = result[1]?.item1 || [];
            state.log = result[1]?.item2 || [];
            if (state.budget.length) {
                ui.setValue("txtBatchName", state.budget[0].bactdesc);
            }

            ui.renderImeiCounter();
            ui.renderProductLookups();
            ui.renderBudget();
            ui.setVisible("btnSelectAll", !state.budget.length);
        };

        if (showLoader === false) {
            applyResult(await action());
            return;
        }

        await Helpers.executeRequest({
            fallbackMessage: "Product selection could not be loaded.",
            action: action,
            onSuccess: applyResult
        });
    }

    function addProduct(product) {
        if (!product || state.budget.some(function (item) {
            return item.prodcode === product.prodcode;
        })) {
            return;
        }

        state.budget.push({
            scode: product.sirtdes,
            prodcode1: product.prodcode1,
            proddesc1: product.proddesc1,
            prodcode: product.prodcode,
            proddesc: product.proddesc,
            produnit: product.produnit,
            targetqty: EnterpriseUI.parseNumber(product.targetqty),
            stqty: EnterpriseUI.parseNumber(product.stqty),
            nproqty: EnterpriseUI.parseNumber(product.nproqty),
            bgdwqty: product.comcod === "8305"
                ? EnterpriseUI.parseNumber(product.stdqty)
                : 0
        });
    }

    function selectProduct() {
        const product = state.products.find(function (item) {
            return item.prodcode === ui.getValue("ddlProductList");
        });

        if (!product) {
            Notifications.Toastr.warning("Product required", "Select a product.");
            return;
        }

        addProduct(product);

        if (!ui.getValue("txtBatchName")) {
            ui.setValue("txtBatchName", `${product.resdesc1}-XXXX`);
        }

        ui.setDisabled(["ddlGroupList"], true);
        ui.renderBudget();
    }

    function selectAllProducts() {
        const group = ui.getValue("ddlGroupList");

        state.products
            .filter(function (product) {
                return String(product.rescod1 ?? "").trim() === group;
            })
            .forEach(addProduct);

        ui.setDisabled(["ddlGroupList"], true);
        ui.renderBudget();
    }

    async function deleteProduct(index) {
        const item = state.budget[index];

        if (!item) {
            return;
        }

        const removeFromState = function () {
            state.budget.splice(index, 1);
            ui.setDisabled(["ddlGroupList"], Boolean(state.budget.length));
            ui.renderBudget();
        };

        if (!item.pbmno) {
            removeFromState();
            return;
        }

        await Helpers.executeRequest({
            fallbackMessage: "Product could not be deleted.",
            action: function () {
                return service.deleteProduct(ui.getBatchNumber(), item.prodcode);
            },
            onSuccess: removeFromState
        });
    }

    async function saveBudget() {
        if (!state.budget.some(function (item) {
            return EnterpriseUI.parseNumber(item.bgdwqty) > 0;
        })) {
            Notifications.Toastr.warning("Quantity required", "Enter at least one budget quantity.");
            return;
        }

        const batchName = ui.getValue("txtBatchName");

        if (!batchName || batchName.toUpperCase().includes("XXXX")) {
            Notifications.Toastr.warning("Batch name required", "Complete the batch name.");
            return;
        }

        await Helpers.executeRequest({
            pending: "#btnFinalUpdate",
            fallbackMessage: "Budget could not be saved.",
            action: function () {
                return service.saveBudget({
                    type: entryType,
                    date: ui.getValue("txtbgddate"),
                    pbnno: ui.getBatchNumber(),
                    sDate: ui.getValue("txtfrmdate"),
                    tDate: ui.getValue("txttodate"),
                    batchName: batchName,
                    items: state.budget,
                    log: state.log
                });
            },
            onSuccess: async function () {
                Notifications.Toastr.success("Saved", "Budget saved successfully.");
                await loadProductSelection(false);
            }
        });
    }

    async function loadMaterials() {
        await Helpers.executeRequest({
            fallbackMessage: "Material input could not be loaded.",
            action: function () {
                return service.loadMaterials(
                    ui.getBatchNumber(),
                    ui.getValue("txtbgddate")
                );
            },
            onSuccess: function (result) {
                state.resources = (result?.resources || []).map(function (item) {
                    return {
                        ...item,
                        selected: item.selected
                            || (item.comcod === "8401"
                                && String(item.rsircode || "").startsWith("0101999"))
                    };
                });
                state.processes = result?.processes || [];
                ui.renderResources();
                ui.renderProcesses();
            }
        });
    }

    async function saveResources() {
        await Helpers.executeRequest({
            pending: "#btnSaveResources",
            fallbackMessage: "Material input could not be saved.",
            action: function () {
                return service.saveMaterials({
                    batchNo: ui.getBatchNumber(),
                    resources: state.resources,
                    processes: state.processes
                });
            },
            onSuccess: function () {
                Notifications.Toastr.success("Saved", "Material input saved successfully.");
            }
        });
    }

    async function loadReportProducts() {
        await Helpers.executeRequest({
            fallbackMessage: "Report products could not be loaded.",
            action: function () {
                return service.loadReportProducts(ui.getBatchNumber());
            },
            onSuccess: function (products) {
                ui.loadSelect(
                    "ddlReportProduct",
                    products || [],
                    "prodcode",
                    "proddesc",
                    "All products"
                );
            }
        });
    }

    async function loadReport() {
        await Helpers.executeRequest({
            pending: "#btnShowReport",
            fallbackMessage: "Budget report could not be loaded.",
            action: function () {
                return service.loadReport(
                    ui.getBatchNumber(),
                    ui.getValue("ddlReportProduct")
                );
            },
            onSuccess: function (report) {
                state.report = report || [];
                ui.renderReport();
            },
            onError: function () {
                state.report = [];
                ui.renderReport();
            }
        });
    }

    function exportReport() {
        if (!state.report.length) {
            Notifications.Toastr.warning("Report required", "Load the report first.");
            return;
        }

        const headers = [
            "Group", "Product", "Resource", "Specification", "Unit",
            "Budgeted Qty", "Rate", "Budgeted Amount", "%"
        ];
        const rows = state.report.map(function (item) {
            return [
                item.grpdesc, item.prodesc, item.rsirdesc, item.spcfdesc,
                item.rsirunit, item.bgdqty, item.bgdrate, item.bgdam, item.percnt
            ];
        });
        const csv = [headers, ...rows].map(function (row) {
            return row.map(function (value) {
                return `"${String(value ?? "").replaceAll('"', '""')}"`;
            }).join(",");
        }).join("\r\n");
        const url = URL.createObjectURL(
            new Blob([csv], { type: "text/csv;charset=utf-8" })
        );
        const link = document.createElement("a");

        link.href = url;
        link.download = `ProductionBudget-${ui.getBatchNumber()}.csv`;
        link.click();
        URL.revokeObjectURL(url);
    }

    async function uploadImei() {
        const file = document.getElementById("imeiFile")?.files?.[0];

        if (!file) {
            Notifications.Toastr.warning("File required", "Select an IMEI workbook.");
            return;
        }

        const form = new FormData();

        form.append("batchno", ui.getBatchNumber());
        form.append("file", file);

        await Helpers.executeRequest({
            pending: "#imeiFile",
            fallbackMessage: "IMEI upload failed.",
            action: function () {
                return service.uploadImei(form);
            },
            onSuccess: function (result) {
                ui.setText("btnImeiCounter", `COUNTER: ${result.count}`);
                Notifications.Toastr.success(
                    "Uploaded",
                    `${result.count} IMEI rows uploaded successfully.`
                );
            }
        });
    }

    async function removeImei() {
        const confirmed = await Swal.fire({
            title: "Remove IMEI list?",
            text: "This will remove the uploaded IMEI list for the current budget.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Remove",
            cancelButtonText: "Cancel"
        });

        if (!confirmed.isConfirmed) {
            return;
        }

        await Helpers.executeRequest({
            pending: "#btnRemoveImei",
            fallbackMessage: "IMEI list could not be removed.",
            action: function () {
                return service.removeImei(ui.getBatchNumber());
            },
            onSuccess: function () {
                ui.setText("btnImeiCounter", "COUNTER");
                Notifications.Toastr.success("Removed", "IMEI list removed successfully.");
            }
        });
    }

    async function showEngineCodes(assigned) {
        await Helpers.executeRequest({
            fallbackMessage: "Chassis data could not be loaded.",
            action: function () {
                return service.loadEngineCodes(ui.getBatchNumber(), assigned);
            },
            onSuccess: function (items) {
                state.engineCodes = items || [];
                ui.renderEngineCodes(assigned);
            }
        });
    }

    async function assignEngineCodes() {
        const items = Array.from(
            document.querySelectorAll(".engine-select:checked")
        ).map(function (checkbox) {
            return state.engineCodes[Number(checkbox.dataset.index)];
        }).filter(Boolean);

        if (!items.length) {
            Notifications.Toastr.warning("Chassis required", "Select at least one chassis.");
            return;
        }

        await Helpers.executeRequest({
            pending: "#btnAssignEngineCodes",
            fallbackMessage: "Chassis assignment failed.",
            action: function () {
                return service.assignEngineCodes({
                    batchNo: ui.getBatchNumber(),
                    items: items
                });
            },
            onSuccess: function () {
                Notifications.Toastr.success("Assigned", "Chassis assigned successfully.");
                bootstrap.Modal
                    .getInstance(document.getElementById("engineCodeModal"))
                    ?.hide();
            }
        });
    }

    async function activateTab(name) {
        ui.activateTab(name);
        if (name == "products") {   
            await loadProductSelection(true);
        }
        if (name === "materials") {
            await loadMaterials();
        }

        if (name === "reports") {
            await loadReportProducts();
        }
    }

    function configureFooter() {
        HideFooterWithButtons();
        HideRefresh();
        HideRecalculate();
        HideSave();
        HideCancel();
    }

    function init() {
        configureFooter();
        bindEvents();
        EnterpriseUI.initialize(document);
        ui.initializeDates();
        ui.renderBudget();
        loadPreviousBudgets();
    }

    function bindEvents() {
        document.getElementById("btnOk")?.addEventListener("click", openBudget);
        document.getElementById("ddlPreProlist")?.addEventListener(
            "change",
            applyPreviousBudget
        );
        document.getElementById("btnSelect")?.addEventListener("click", selectProduct);
        document.getElementById("btnSelectAll")?.addEventListener("click", selectAllProducts);
        document.getElementById("btnTotal")?.addEventListener("click", ui.renderBudgetTotal);
        document.getElementById("btnFinalUpdate")?.addEventListener("click", saveBudget);
        document.getElementById("btnPrintBudget")?.addEventListener("click", function () {
            ui.printTable("productBudgetTable", "Production Budget Information");
        });
        document.getElementById("btnResourceTotal")?.addEventListener(
            "click",
            ui.renderResourceTotals
        );
        document.getElementById("btnSaveResources")?.addEventListener("click", saveResources);
        document.getElementById("btnShowReport")?.addEventListener("click", loadReport);
        document.getElementById("btnExportReport")?.addEventListener("click", exportReport);
        document.getElementById("btnPrintReport")?.addEventListener("click", function () {
            ui.printTable("budgetReportTable", "Production Budget Report");
        });
        document.getElementById("imeiFile")?.addEventListener("change", uploadImei);
        document.getElementById("btnRemoveImei")?.addEventListener("click", removeImei);
        document.getElementById("btnFreeChassis")?.addEventListener("click", function () {
            showEngineCodes(false);
        });
        document.getElementById("btnAssignedChassis")?.addEventListener("click", function () {
            showEngineCodes(true);
        });
        document.getElementById("btnAssignEngineCodes")?.addEventListener(
            "click",
            assignEngineCodes
        );

        document.querySelectorAll("[data-budget-tab]").forEach(function (tab) {
            tab.addEventListener("click", function () {
                activateTab(this.dataset.budgetTab);
            });
        });

        $(document)
            .off("change.prodBudgetGroup", "#ddlGroupList")
            .on("change.prodBudgetGroup", "#ddlGroupList", function () {
                ui.renderProductOptions();
            })
            .off("input.prodBudget", ".budget-qty")
            .on("input.prodBudget", ".budget-qty", function () {
                const item = state.budget[Number(this.dataset.index)];

                if (item) {
                    item.bgdwqty = EnterpriseUI.parseNumber(this.value);
                    ui.renderBudgetTotal();
                }
            })
            .off("click.prodBudget", ".delete-product")
            .on("click.prodBudget", ".delete-product", function () {
                deleteProduct(Number(this.dataset.index));
            })
            .off("input.prodBudget", ".resource-rate")
            .on("input.prodBudget", ".resource-rate", function () {
                const item = state.resources[Number(this.dataset.index)];

                if (item) {
                    item.bgdrat = EnterpriseUI.parseNumber(this.value);
                    ui.renderResourceTotals();
                }
            })
            .off("change.prodBudget", ".resource-select")
            .on("change.prodBudget", ".resource-select", function () {
                const item = state.resources[Number(this.dataset.index)];

                if (item) {
                    item.selected = this.checked;
                }
            })
            .off("change.prodBudget", ".process-active")
            .on("change.prodBudget", ".process-active", function () {
                const item = state.processes[Number(this.dataset.index)];

                if (item) {
                    item.active = this.checked;
                }
            });
    }

    return {
        init: init
    };
})();

document.addEventListener(
    "DOMContentLoaded",
    ProdBudgetPage.init
);
