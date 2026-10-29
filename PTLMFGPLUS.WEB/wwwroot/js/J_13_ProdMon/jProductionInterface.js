/* Area/Controller */
const commonPath = "/F_13_ProdMon/ProductionInterface/";
const commonPrintPath = "/F_13_ProdMon/ReportProdMon/";
const urlParams = new URLSearchParams(window.location.search);

var ProductionInterfacePage = (function () {
    "use strict";

    const type = urlParams.get("Type") || urlParams.get("type") || "FG";
    const tableContainers = [
        "tableContainerReq", "tableContainerReqApp", "tableContainerMaterialissue",
        "tableContainerIssueAppr", "tableContainermatTransfered", "tableContainerProentry",
        "tableContainerQcEntry", "tableContainerFgReceive", "tableContainerWhReceive",
        "tableContainerCompletedProd", "tableContainerRework"
    ];
    const state = { interfaceData: null, selectedInterface: "requests" };
    const {
        safeTextColumn: textColumn,
        wrappedTextColumn,
        numericColumn,
        dateColumn,
        rowNumberColumn,
        actionColumn: sharedActionColumn
    } = EnterpriseUI.dataTableColumns;

    const service = {
        loadInterfaceData(frmdate, todate) {
            return Helpers.withLoader(() => FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetInterfaceInfo`),
                { frmdate, todate, type }
            ));
        }
        // printProductionBudget(pbmno, type) {
        //     return Helpers.withLoader(() => FetchHelpers.getForm(
        //         FetchHelpers.urlconfig(`${commonPrintPath}ProdbudgetPrint`),
        //         {pbmno,type }
        //     ));
        // }
    };

    function actionColumn(actions) {
        return sharedActionColumn(actions.map(function (action) {
            return {
                name: action.key,
                label: action.title,
                title: action.title,
                icon: `bi ${action.icon}`,
                iconOnly: true,
                className: `btn btn-sm ${action.style || "btn-outline-secondary"} interface-action-button`
            };
        }), {
            className: "interface-actions-cell"
        });
    }

    const printAction = { key: "print", title: "Print", icon: "bi-printer", style: "btn-outline-primary" };
    const entryAction = { key: "entry", title: "Entry", icon: "bi-check-lg", style: "btn-outline-success" };
    const editAction = { key: "edit", title: "Edit", icon: "bi-pencil-square", style: "btn-outline-dark" };
    const deleteAction = { key: "delete", title: "Delete", icon: "bi-trash", style: "btn-outline-danger" };

    const tables = {
        requests: {
            container: "tableContainerReq",
            table: "productionRequestTable",
            source: "item0",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("pbmno1"),
                    dateColumn("bgddat"),
                    wrappedTextColumn("prodesc"),
                    numericColumn("itemcount"),
                    numericColumn("bgdwqty"),
                    numericColumn("bgdamt"),
                    dateColumn("sdate"),
                    dateColumn("enddate"),
                    textColumn("pbmststus"),
                    wrappedTextColumn("actdesc"),
                    actionColumn([printAction])              


                ]
        },
        requisitions: {
            container: "tableContainerReqApp",
            table: "productionRequisitionTable",
            source: "item0",
            filter: item => item.pbmststus === "Requsition",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("pbmno1"),
                    dateColumn("bgddat"),
                    wrappedTextColumn("prodesc"),
                    numericColumn("itemcount"),
                    numericColumn("bgdwqty"),
                    numericColumn("bgdamt"),
                    numericColumn("bgdbal"),
                    wrappedTextColumn("actdesc"),
                    actionColumn([printAction, entryAction, editAction, deleteAction])
                ]
        },
        issues: {
            container: "tableContainerMaterialissue",
            table: "productionIssueTable",
            source: "item1",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("pbmno1"),
                    textColumn("preqno1"),
                    dateColumn("pbdate"),
                    wrappedTextColumn("batchdesc"),
                    numericColumn("fgreqqty"),
                    numericColumn("trescount"),
                    numericColumn("rcount"),
                    textColumn("isstatus"),
                    actionColumn(
                        [printAction, entryAction, editAction, deleteAction,
                            { key: "start", title: "Start production", icon: "bi-play-fill", style: "btn-outline-success" }
                        ]
                    )]
        },
        approvals: {
            container: "tableContainerIssueAppr",
            table: "productionIssueApprovalTable",
            source: "item6",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("misuno1"),
                    dateColumn("misudate"),
                    textColumn("preqno1"),
                    wrappedTextColumn("actdesc"),
                    wrappedTextColumn("batchdesc"),
                    numericColumn("fgreqqty"),
                    numericColumn("fgisuqty"),
                    numericColumn("isuqty"),
                    actionColumn([printAction,
                        { key: "approve", title: "Approve", icon: "bi-check-circle", style: "btn-outline-success" }
                    ])]
        },
        process: {
            container: "tableContainermatTransfered",
            table: "productionProcessTable",
            source: "item5",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("pbno"),
                    textColumn("preqno"),
                    dateColumn("pbdate"),
                    wrappedTextColumn("batchdesc"),
                    wrappedTextColumn("rsirdesc"),
                    numericColumn("itemcount"),
                    numericColumn("acqty"),
                    numericColumn("processbal"),
                    actionColumn([entryAction])
                ]
        },
        production: {
            container: "tableContainerProentry",
            table: "productionEntryTable",
            source: "item2",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("pbmno1"),
                    dateColumn("pbdate"),
                    wrappedTextColumn("prodesc"),
                    wrappedTextColumn("batchdesc"),
                    numericColumn("itemcount"),
                    numericColumn("acqty"),
                    numericColumn("balqty"),
                    numericColumn("proamt"),
                    textColumn("proatatus"),
                    actionColumn(
                        [printAction, entryAction, editAction,
                            { key: "proceed", title: "Proceed", icon: "bi-arrow-right-circle", style: "btn-outline-primary" }
                        ])
                ]
        },
        qc: {
            container: "tableContainerQcEntry",
            table: "productionQcTable",
            source: "item3",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("pbmno1"),
                    textColumn("prodid"),
                    dateColumn("prodate"),
                    wrappedTextColumn("batchdesc"),
                    numericColumn("itemcount"),
                    numericColumn("balqty"),
                    numericColumn("proamt"),
                    textColumn("pstatus"),
                    actionColumn([printAction, entryAction, editAction, deleteAction])
                ]
        },
        floorReceive: {
            container: "tableContainerFgReceive",
            table: "productionFloorReceiveTable",
            source: "item4",
            filter: item => !item.qcapprove,
            columns:
                [
                    rowNumberColumn(),
                    textColumn("prodid"),
                    dateColumn("prodate"),
                    textColumn("grrno"),
                    wrappedTextColumn("batchdesc"),
                    numericColumn("itemcount"),
                    numericColumn("proqty"),
                    numericColumn("proamt"),
                    textColumn("rcvtype"),
                    actionColumn([
                        { key: "print-production", title: "Print production entry", icon: "bi-printer", style: "btn-outline-primary" },
                        { key: "print-qc", title: "Print QC entry", icon: "bi-file-earmark-text", style: "btn-outline-info" },
                        entryAction, deleteAction])
                ]
        },
        warehouseReceive: {
            container: "tableContainerWhReceive",
            table: "productionWarehouseReceiveTable",
            source: "item4",
            filter: item => Boolean(item.qcapprove),
            columns:
                [
                    rowNumberColumn(),
                    textColumn("prodid"),
                    dateColumn("prodate"),
                    textColumn("grrno"),
                    wrappedTextColumn("batchdesc"),
                    numericColumn("itemcount"),
                    numericColumn("proqty"),
                    numericColumn("proamt"),
                    textColumn("rcvtype"),
                    actionColumn(
                        [printAction, entryAction, editAction, deleteAction])
                ]
        },
        completed: {
            container: "tableContainerCompletedProd",
            table: "productionCompletedTable",
            source: "item0",
            filter: item => item.pbmststus === "Complete",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("pbmno1"),
                    dateColumn("bgddat"),
                    numericColumn("itemcount"),
                    numericColumn("bgdwqty"),
                    numericColumn("bgdamt"),
                    textColumn("pbmststus"),
                    actionColumn([printAction, entryAction, editAction])
                ]
        },
        rework: {
            container: "tableContainerRework",
            table: "productionReworkTable",
            source: "item8",
            columns:
                [
                    rowNumberColumn(),
                    textColumn("preqno"),
                    dateColumn("rpbdate"),
                    wrappedTextColumn("batchdesc"),
                    numericColumn("itemqty"),
                    numericColumn("balqty"),
                    actionColumn([entryAction])
                ]
        }
    };

    const ui = {
        getValue(id) {
            return document.getElementById(id)?.value?.trim() || "";
        },

        hideTables() {
            tableContainers.forEach(function (id) {
                const container = document.getElementById(id);

                if (container) {
                    container.style.display = "none";
                }
            });
        },

        getInterfaceTitle(key) {
            const card = document.querySelector(`[data-interface="${key}"]`);

            return card?.querySelector(".interface-kpi-title")?.textContent?.trim() || "";
        },

        getNumericColumnIndexes(columns) {
            return columns.reduce(function (indexes, column, index) {
                if (column.className?.split(/\s+/).includes("interface-number-cell")) {
                    indexes.push(index);
                }

                return indexes;
            }, []);
        },

        ensureTableFooter(selector, columns) {
            const table = document.querySelector(selector);

            if (!table || table.tFoot) {
                return;
            }

            const numericIndexes = ui.getNumericColumnIndexes(columns);

            if (!numericIndexes.length) {
                return;
            }

            const footer = table.createTFoot();
            const row = footer.insertRow();

            columns.forEach(function (_column, index) {
                const cell = document.createElement("th");

                cell.scope = "col";
                cell.className = numericIndexes.includes(index)
                    ? "text-end interface-total-cell"
                    : "";
                cell.textContent = index === 0 ? "Total" : "";
                row.appendChild(cell);
            });
        },

        createFooterDefinitions(columns) {
            return ui.getNumericColumnIndexes(columns).map(function (index) {
                return {
                    column: index,
                    format: function (total) {
                        return EnterpriseUI.formatNumber(total, {
                            minimumFractionDigits: Number.isInteger(total) ? 0 : 2,
                            maximumFractionDigits: 6
                        });
                    }
                };
            });
        },

        renderSelectedTable(key) {
            const config = tables[key];

            if (!config) {
                return;
            }

            const source = state.interfaceData && Array.isArray(state.interfaceData[config.source])
                ? state.interfaceData[config.source]
                : [];
            const data = config.filter ? source.filter(config.filter) : source;
            const tableTitle = ui.getInterfaceTitle(key);
            const selector = `#${config.table}`;

            ui.hideTables();

            const container = document.getElementById(config.container);

            if (container) {
                container.style.display = "block";
            }

            if ($.fn.DataTable.isDataTable(selector)) {
                const existing = $(selector).DataTable();

                existing.clear().rows.add(data).draw(false);
                existing.columns.adjust();
                EnterpriseUI.setDataTableTitle(existing, tableTitle);
                return;
            }

            ui.ensureTableFooter(selector, config.columns);

            EnterpriseUI.createDataTable(selector, {
                enterpriseTitle: tableTitle,
                data: data,
                columns: config.columns,
                deferRender: true,
                processing: true,
                autoWidth: false,
                order: [],
                footerCallback: EnterpriseUI.dataTableFooter.create(
                    ui.createFooterDefinitions(config.columns),
                    { labelColumn: 0, label: "Total" }
                ),
                language: {
                    emptyTable: "No data found",
                    processing: "Loading..."
                }
            });
        },

        setCounter(id, value) {
            const element = document.getElementById(id);

            if (element) {
                element.textContent = Number(value || 0);
            }
        },

        renderCounters() {
            const summary = state.interfaceData?.item7?.[0] || {};

            ui.setCounter("reqQty", summary.reqqty);
            ui.setCounter("reqApprQty", summary.proreqqty);
            ui.setCounter("materialIssueQty", summary.issueqty);
            ui.setCounter("issueApprQty", summary.isuapqty);
            ui.setCounter("mattTransQty", summary.procsqty);
            ui.setCounter("ProEntryProdIntQty", summary.prodqty);
            ui.setCounter("QcProdIntQty", summary.qcqty);
            ui.setCounter("FgReceiveProdIntQty", summary.florcv);
            ui.setCounter("WhReceiveProdIntQty", summary.strecqty);
            ui.setCounter("CompletedProdIntQty", summary.compqty);
            ui.setCounter("ReworkQty", summary.rework);
        },

        setSelectedCard(key) {
            document.querySelectorAll(".interface-kpi-card").forEach(function (card) {
                const isSelected = card.dataset.interface === key;

                card.classList.toggle("active", isSelected);
                card.setAttribute("aria-pressed", String(isSelected));
            });
        },

        initializeDateFilters() {
            EnterpriseUI.dates.initializeFilters([
                { txtfrmdate: "First" },
                { txttodate: 0 }
            ]);
        }
    };

    async function loadData() {
        try {
            const result = await service.loadInterfaceData(
                ui.getValue("txtfrmdate"),
                ui.getValue("txttodate")
            );

            state.interfaceData = result || {};
            ui.renderCounters();
            ui.renderSelectedTable(state.selectedInterface);
        }
        catch (error) {
            state.interfaceData = {};
            ui.renderCounters();
            ui.renderSelectedTable(state.selectedInterface);
            Notifications.handleError(
                error,
                "Production interface data could not be loaded."
            );
        }
    }

    function selectTable(key) {
        if (!tables[key]) {
            return;
        }

        state.selectedInterface = key;
        ui.setSelectedCard(key);

        if (state.interfaceData) {
            ui.renderSelectedTable(key);
        }
    }

    function budgetEntryType() {
        if (type.toLowerCase() === "semifg") return "EntrySemi";
        if (type.toLowerCase() === "presemifg") return "PreEntrySemi";
        return "Entry";
    }



    function openRequisitionEntry(button) {
        const row = EnterpriseUI.getResponsiveRowData(
            "#productionRequisitionTable",
            button
        );
        if (!row?.pbmno || !row?.actcode) {
            Notifications.Toastr.error("Budget or WIP information is missing for this requisition.");
            return;
        }

        const params = new URLSearchParams({
            Type: budgetEntryType(),
            genno: row.pbmno,
            actcode: row.actcode
        });
        window.open(`/F_11_Pro/ProdReq?${params.toString()}`, "_blank");
    }
    function printRequestProduction(button) {
        const row = EnterpriseUI.getResponsiveRowData(
            "#productionRequestTable",
            button
        );
        if (!row?.pbmno || !row?.actcode) {
            Notifications.Toastr.error("Budget or WIP information is missing for this requisition.");
            return;
        }

        const params = new URLSearchParams({
            Type: type,
            pbmno: row.pbmno,
            
        });
        window.open(`${commonPrintPath}ProdbudgetPrint?${params.toString()}`, "_blank");
    }

    function openMaterialIssueEntry(button) {
        const row = EnterpriseUI.getResponsiveRowData(
            "#productionIssueTable",
            button
        );
        const requisitionNo = row?.preqno || row?.actcode;
        const issueDate = row?.pbdate;
        if (!requisitionNo || !issueDate) {
            Notifications.Toastr.error("Requisition number or date is missing for material issue.");
            return;
        }
        const genno = EnterpriseUI.dates.serialize(issueDate, "display") || issueDate;
        const params = new URLSearchParams({ Type: "Entry", actcode: requisitionNo, genno });
        window.open(`/F_07_Inv/PBMatIssueSingle?${params.toString()}`, "_blank");
    }

    function init() {
        bindEvents();
        EnterpriseUI.initialize(document);
        ui.initializeDateFilters();
        loadData();
    }

    function bindEvents() {
        const cardBindings = {
            requestsDiv: "requests",
            reqApprovalDiv: "requisitions",
            materialIssueDiv: "issues",
            issueAppDiv: "approvals",
            matTransferedDiv: "process",
            ProEntryProdDiv: "production",
            QcEntryDiv: "qc",
            FgReceiveProdDiv: "floorReceive",
            WhReceiveProdDiv: "warehouseReceive",
            CompletedProdDiv: "completed",
            ReworkDiv: "rework"
        };

        Object.entries(cardBindings).forEach(function ([id, key]) {
            document.getElementById(id)?.addEventListener("click", function () {
                selectTable(key);
            });
        });

        document.getElementById("btnOk")?.addEventListener("click", loadData);

        document.getElementById("requisitionEntry")?.addEventListener("click", function () {
            window.open(
                `/F_13_ProdMon/ProdBudget/ProdBudgetIndex?type=${budgetEntryType()}`,
                "_blank"
            );
        });

        $("#productionRequisitionTable").on(
            "click",
            '.interface-action-button[data-action="entry"]',
            function () {
                openRequisitionEntry(this);
            }
        );
        $("#productionRequestTable").on(
            "click",
            '.interface-action-button[data-action="print"]',
            function () {
                printRequestProduction(this);
            }
        );

        $("#productionIssueTable").on(
            "click",
            '.interface-action-button[data-action="entry"]',
            function () {
                openMaterialIssueEntry(this);
            }
        );
    }

    return {
        init: init
    };
})();

document.addEventListener(
    "DOMContentLoaded",
    ProductionInterfacePage.init
);
