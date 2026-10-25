/*
 * PTLMFGPLUS interface-page starter.
 * Copy to wwwroot/js/J_{AreaName}/j{PageName}.js and replace placeholders.
 * Required order: state -> service -> ui -> workflows -> init -> bindEvents.
 */

const commonPath = "/{AreaName}/{ControllerName}/";

var InterfaceTemplatePage = (function () {
    "use strict";

    const state = {
        interfaceData: null,
        selectedInterface: "pending",
        tables: {},
        isLoading: false
    };

    const service = {
        async loadInterfaceData(fromDate, toDate) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetInterfaceData`),
                {
                    fromDate: fromDate,
                    toDate: toDate
                },
                false
            );
        }
    };

    const ui = Object.assign({}, EnterpriseUI.dom, {
        initializeDates() {
            EnterpriseUI.dates.initializeFilters([
                { txtFromDate: 30 },
                { txtToDate: 0 }
            ]);
        },

        setSelectedCard(key) {
            document.querySelectorAll(".interface-kpi-card").forEach(function (card) {
                const selected = card.dataset.interface === key;

                card.classList.toggle("active", selected);
                card.setAttribute("aria-pressed", String(selected));
            });
        },

        hideTableContainers() {
            document.querySelectorAll(".interface-table-container").forEach(function (container) {
                container.classList.add("d-none");
            });
        },

        ensureTableFooter(tableSelector, columnCount) {
            const table = document.querySelector(tableSelector);

            if (!table || table.tFoot) {
                return;
            }

            const footer = table.createTFoot();
            const row = footer.insertRow();

            for (let index = 0; index < columnCount; index += 1) {
                const cell = document.createElement("th");

                cell.scope = "col";
                cell.textContent = index === 0 ? "Total" : "";
                row.appendChild(cell);
            }
        },

        renderCounters() {
            const summary = state.interfaceData?.summary || {};

            ui.setText("pendingCount", summary.pendingCount);
            ui.setText("completedCount", summary.completedCount);
        },

        renderTable(key) {
            const configuration = tableConfigurations[key];

            if (!configuration) {
                return;
            }

            const source = state.interfaceData?.[configuration.source];
            const records = Array.isArray(source) ? source : [];

            ui.hideTableContainers();
            document
                .querySelector(configuration.containerSelector)
                ?.classList.remove("d-none");
            ui.ensureTableFooter(
                configuration.tableSelector,
                configuration.columns.length
            );

            state.tables[key] = EnterpriseUI.createDataTable(
                configuration.tableSelector,
                {
                    enterpriseTitle: configuration.title,
                    data: records,
                    columns: configuration.columns,
                    order: [],
                    autoWidth: false,
                    deferRender: true,
                    processing: true,
                    footerCallback: EnterpriseUI.dataTableFooter.create(
                        configuration.totals,
                        {
                            labelColumn: 0,
                            label: "Total"
                        }
                    ),
                    language: {
                        emptyTable: "No records match the selected filters."
                    }
                }
            );
        },

        render() {
            ui.renderCounters();
            ui.setSelectedCard(state.selectedInterface);
            ui.renderTable(state.selectedInterface);
        }
    });

    const {
        rowNumberColumn,
        safeTextColumn,
        wrappedTextColumn,
        numericColumn,
        dateColumn,
        actionColumn
    } = EnterpriseUI.dataTableColumns;

    const tableConfigurations = {
        pending: {
            containerSelector: "#pendingTableContainer",
            tableSelector: "#pendingTable",
            title: "Pending records",
            source: "pendingRecords",
            columns: [
                rowNumberColumn(),
                safeTextColumn("documentNo"),
                dateColumn("documentDate"),
                wrappedTextColumn("description"),
                numericColumn("quantity", {
                    numberFormat: {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }
                }),
                actionColumn([
                    {
                        name: "open",
                        label: "Open record",
                        title: "Open record",
                        icon: "bi bi-box-arrow-up-right",
                        iconOnly: true,
                        className: "btn btn-outline-primary btn-sm interface-action-button"
                    }
                ])
            ],
            totals: [
                {
                    column: 4,
                    numberFormat: {
                        minimumFractionDigits: 2,
                        maximumFractionDigits: 2
                    }
                }
            ]
        },

        completed: {
            containerSelector: "#completedTableContainer",
            tableSelector: "#completedTable",
            title: "Completed records",
            source: "completedRecords",
            columns: [
                rowNumberColumn(),
                safeTextColumn("documentNo"),
                dateColumn("documentDate"),
                wrappedTextColumn("description"),
                numericColumn("quantity")
            ],
            totals: [
                {
                    column: 4
                }
            ]
        }
    };

    function validateFilters() {
        const range = EnterpriseUI.dates.validateRange(
            ui.getValue("txtFromDate"),
            ui.getValue("txtToDate")
        );

        if (!range.valid) {
            Notifications.Toastr.warning("Check dates", range.message);
            return null;
        }

        return {
            fromDate: EnterpriseUI.dates.serialize(range.from, "display"),
            toDate: EnterpriseUI.dates.serialize(range.to, "display")
        };
    }

    async function loadInterfaceData() {
        if (state.isLoading) {
            return;
        }

        const filters = validateFilters();

        if (!filters) {
            return;
        }

        state.isLoading = true;

        await Helpers.executeRequest({
            pending: "#btnRun, #btnFooterRefresh",
            fallbackMessage: "Interface data could not be loaded.",
            action: function () {
                return service.loadInterfaceData(
                    filters.fromDate,
                    filters.toDate
                );
            },
            onSuccess: function (result) {
                state.interfaceData = result || {};
                ui.render();
            },
            onError: function () {
                state.interfaceData = {};
                ui.render();
            },
            onFinally: function () {
                state.isLoading = false;
            }
        });
    }

    function selectInterface(key) {
        if (!tableConfigurations[key]) {
            return;
        }

        state.selectedInterface = key;
        ui.setSelectedCard(key);

        if (state.interfaceData) {
            ui.renderTable(key);
        }
    }

    function openRecord(button) {
        const table = state.tables[state.selectedInterface];
        const row = EnterpriseUI.getResponsiveRowData(table, button);

        if (!row?.id) {
            Notifications.Toastr.warning(
                "Record unavailable",
                "The selected record could not be opened."
            );
            return;
        }

        const query = new URLSearchParams({ id: row.id });

        window.open(
            `/{AreaName}/{EntryController}/Index?${query.toString()}`,
            "_blank",
            "noopener"
        );
    }

    function configureFooter() {
        ShowFooterWithButtons();
        HideSave();
        HideRecalculate();
        HideCancel();
        HideApprove();
    }

    function init() {
        configureFooter();
        bindEvents();
        EnterpriseUI.initialize(document);
        ui.initializeDates();
        loadInterfaceData();
    }

    function bindEvents() {
        document.getElementById("btnRun")?.addEventListener(
            "click",
            loadInterfaceData
        );

        document.getElementById("btnFooterRefresh")?.addEventListener(
            "click",
            loadInterfaceData
        );

        document.querySelectorAll(".interface-kpi-card").forEach(function (card) {
            card.addEventListener("click", function () {
                selectInterface(this.dataset.interface);
            });
        });

        $(document)
            .off("click.interfaceTemplate", ".interface-action-button")
            .on(
                "click.interfaceTemplate",
                ".interface-action-button[data-action='open']",
                function () {
                    openRecord(this);
                }
            );
    }

    return {
        init: init
    };
})();

document.addEventListener(
    "DOMContentLoaded",
    InterfaceTemplatePage.init
);
