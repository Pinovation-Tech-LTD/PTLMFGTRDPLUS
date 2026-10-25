/*
 * PTLMFGPLUS page script template
 * Copy to: PTLMFGPLUS.WEB/wwwroot/js/J_{AreaName}/j{PageName}.js
 * Replace names, route, selectors, columns, validation, and business messages only.
 */

const commonPath = "/{AreaName}/{ControllerName}/";

var ExamplePage = (function () {
    "use strict";

    const selectors = {
        table: "#tblRecords",
        fromDate: "#txtFromDate",
        toDate: "#txtToDate",
        save: "#btnFooterSave",
        refresh: "#btnFooterRefresh"
    };

    const state = {
        records: [],
        table: null,
        selectedId: null
    };

    const {
        rowNumberColumn,
        safeTextColumn,
        wrappedTextColumn,
        numericColumn,
        dateColumn,
        actionColumn
    } = EnterpriseUI.dataTableColumns;

    const service = {
        async load(filters) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetRecords`),
                filters,
                false
            );
        },

        async save(request) {
            return FetchHelpers.postJson(
                FetchHelpers.urlconfig(`${commonPath}Save`),
                request,
                false
            );
        }
    };

    const ui = {
        getValue(selector) {
            return document.querySelector(selector)?.value?.trim() || "";
        },

        renderTable() {
            state.table = EnterpriseUI.createDataTable(selectors.table, {
                enterpriseTitle: "Records",
                data: state.records,
                order: [],
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
                            name: "edit",
                            label: "Edit record",
                            title: "Edit record",
                            icon: "bi bi-pencil",
                            iconOnly: true,
                            data: {
                                id: "id"
                            }
                        }
                    ])
                ],
                footerCallback: EnterpriseUI.dataTableFooter.create([
                    {
                        column: 4,
                        numberFormat: {
                            minimumFractionDigits: 2,
                            maximumFractionDigits: 2
                        }
                    }
                ], {
                    labelColumn: 0,
                    label: "Total"
                })
            });
        }
    };

    const validation = {
        filters() {
            const range = EnterpriseUI.dates.validateRange(
                ui.getValue(selectors.fromDate),
                ui.getValue(selectors.toDate)
            );

            if (!range.valid) {
                Notifications.Toastr.warning("Check dates", range.message);
                return null;
            }

            return {
                fromDate: EnterpriseUI.dates.serialize(range.from, "display"),
                toDate: EnterpriseUI.dates.serialize(range.to, "display")
            };
        },

        saveRequest() {
            return {
                id: state.selectedId,
                documentDate: EnterpriseUI.dates.serialize(
                    ui.getValue("#txtDocumentDate"),
                    "display"
                ),
                lines: []
            };
        }
    };

    async function loadRecords() {
        const filters = validation.filters();

        if (!filters) {
            return;
        }

        await Helpers.executeRequest({
            pending: selectors.refresh,
            fallbackMessage: "Records could not be loaded.",
            action: function () {
                return service.load(filters);
            },
            onSuccess: function (records) {
                state.records = records || [];
                ui.renderTable();
            },
            onError: function () {
                state.records = [];
                ui.renderTable();
            }
        });
    }

    async function saveRecord() {
        const request = validation.saveRequest();

        await Helpers.executeRequest({
            pending: selectors.save,
            fallbackMessage: "The record could not be saved.",
            action: function () {
                return service.save(request);
            },
            onSuccess: async function (result) {
                Notifications.Toastr.success(
                    "Saved",
                    result.message || "The record was saved."
                );
                await loadRecords();
            }
        });
    }

    function editRecord(button) {
        const row = EnterpriseUI.getResponsiveRowData(
            state.table,
            button
        );

        if (!row) {
            return;
        }

        state.selectedId = row.id;
        // Populate the page or modal through ui methods.
    }

    function bindEvents() {
        document
            .querySelector(selectors.save)
            ?.addEventListener("click", saveRecord);

        document
            .querySelector(selectors.refresh)
            ?.addEventListener("click", loadRecords);

        $(selectors.table).on(
            "click",
            "[data-action='edit']",
            function () {
                editRecord(this);
            }
        );
    }

    function configureFooter() {
        ShowFooterWithButtons();
        HideRecalculate();
        HideApprove();
    }

    function init() {
        configureFooter();
        bindEvents();
        EnterpriseUI.initialize(document);
        loadRecords();
    }

    return {
        init: init
    };
})();

document.addEventListener(
    "DOMContentLoaded",
    ExamplePage.init
);
