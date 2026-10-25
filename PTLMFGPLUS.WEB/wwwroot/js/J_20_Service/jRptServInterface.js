const commonPath = "/F_20_Service/RptServInterface/";

var ServiceInterfacePage = (function () {
    "use strict";

    const state = {
        records: [],
        summary: {},
        selectedStage: "receive",
        table: null
    };
    const columns = EnterpriseUI.dataTableColumns;

    const service = {
        loadInitialData() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}GetInitialData`)
                );
            });
        },

        loadInterfaceData(filters) {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}GetInterfaceInfo`),
                    filters
                );
            });
        }
    };

    function actionColumn(actions) {
        return columns.actionColumn(
            actions.map(function (action) {
                return {
                    name: action.name,
                    label: action.label,
                    title: action.label,
                    icon: action.icon,
                    iconOnly: true,
                    visible: action.visible,
                    className: `btn btn-sm ${action.className || "btn-outline-primary"} interface-action-button`
                };
            }),
            { className: "interface-actions-cell" }
        );
    }

    function field(title, definition) {
        return { title: title, definition: definition };
    }

    const rowNumber = field("#", columns.rowNumberColumn());
    const center = field("Service center", columns.wrappedTextColumn("actdesc"));
    const receivedDate = field("Received date", columns.dateColumn("recvdate"));
    const receiveNo = field("Receive no.", columns.safeTextColumn("recvno1"));
    const warranty = field("Warranty status", columns.wrappedTextColumn("wrstatusdesc"));
    const replacementWarranty = field(
        "Warranty status",
        columns.wrappedTextColumn("rwrstatusdesc")
    );
    const imei = field("IMEI", columns.safeTextColumn("mimei"));
    const customer = field("Customer", columns.wrappedTextColumn("custname"));
    const product = field("Product details", columns.wrappedTextColumn("prodesc"));
    const problem = field("Problem details", columns.wrappedTextColumn("problemlist"));
    const deliveryDate = field("Probable delivery", dueDateColumn("deldate"));
    const engineer = field("Engineer", columns.wrappedTextColumn("empname"));

    const stages = {
        receive: {
            filter: function () {
                return true;
            },
            fields: [
                rowNumber,
                center,
                receivedDate,
                receiveNo,
                warranty,
                field("Courier", columns.wrappedTextColumn("bcname")),
                field("CN no.", columns.safeTextColumn("bcnno")),
                field("Booking date", columns.dateColumn("bcdate")),
                imei,
                customer,
                field("Phone", columns.safeTextColumn("custmob")),
                product,
                field("Product problem", columns.wrappedTextColumn("probdetails")),
                deliveryDate,
                field("Current status", columns.safeTextColumn("fstatus")),
                field("Engineer", columns.wrappedTextColumn("empname")),
                field("Actions", actionColumn([
                    {
                        name: "edit-receive",
                        label: "Edit receive",
                        icon: "bi bi-pencil-square",
                        className: "btn-outline-dark",
                        visible: function (row) {
                            return Boolean(
                                String(row.centrid || "").trim()
                                && String(row.recvno || row.recvno1 || "").trim()
                            );
                        }
                    }
                ]))
            ]
        },
        transfer: {
            filter: function (row) {
                return isEmpty(row.trnstatus);
            },
            fields: commonWorkflowFields([
                field("Transfer status", columns.safeTextColumn("trnstatus")),
                field("Actions", actionColumn([
                    {
                        name: "transfer",
                        label: "Transfer product",
                        icon: "bi bi-arrow-left-right"
                    }
                ]))
            ])
        },
        assign: {
            filter: function (row) {
                return isEmpty(row.empid) && equals(row.trnstatus, "Ok");
            },
            fields: commonWorkflowFields([
                problem,
                field("Transfer status", columns.safeTextColumn("trnstatus")),
                field("Actions", actionColumn([
                    {
                        name: "assign",
                        label: "Assign engineer",
                        icon: "bi bi-person-check"
                    }
                ]))
            ])
        },
        diagnosis: {
            filter: function (row) {
                return equals(row.digstatus, "Received")
                    && isEmpty(row.wdone)
                    && matchesEmployee(row.empid)
                    && !isEmpty(row.empid)
                    && !equals(row.retstatus, "R");
            },
            fields: diagnosticFields("Diagnosis", "diagnose", "Diagnosis entry")
        },
        hotline: {
            filter: function (row) {
                return !isEmpty(row.custstatus)
                    && !equals(row.retstatus, "R")
                    && !equals(row.fstatus, "Delivery");
            },
            fields: commonWorkflowFields([
                field("Phone", columns.safeTextColumn("custmob")),
                problem,
                engineer,
                field("Current status", columns.safeTextColumn("cutstdesc")),
                field("Hotline message", columns.wrappedTextColumn("custhtmsg")),
                field("Actions", actionColumn([
                    {
                        name: "hotline",
                        label: "Hotline",
                        icon: "bi bi-telephone"
                    },
                    {
                        name: "return",
                        label: "Return to customer",
                        icon: "bi bi-arrow-return-left",
                        className: "btn-outline-secondary"
                    }
                ]))
            ])
        },
        approved: {
            filter: function (row) {
                return equals(row.digstatus, "Diagnosis")
                    && EnterpriseUI.parseNumber(row.balqty) !== 0
                    && equals(row.notappved, "NotOk");
            },
            fields: commonWorkflowFields([
                replacementWarranty,
                engineer,
                field("Actions", actionColumn([
                    {
                        name: "approve",
                        label: "Approve diagnosis",
                        icon: "bi bi-check-circle",
                        className: "btn-outline-success",
                        visible: function (row) {
                            return !isSentinelDate(row.digndate);
                        }
                    }
                ]))
            ])
        },
        doadap: {
            filter: function (row) {
                return !isEmpty(row.rpprocode)
                    && isEmpty(row.rimei)
                    && equals(row.approved, "Ok");
            },
            fields: commonWorkflowFields([
                field("Replacement IMEI", columns.safeTextColumn("appimei")),
                field("Replacement product", columns.wrappedTextColumn("represdesc")),
                field("Actions", actionColumn([
                    {
                        name: "replacement",
                        label: "Replacement entry",
                        icon: "bi bi-arrow-repeat",
                        visible: function (row) {
                            return isEmpty(row.appimei);
                        }
                    }
                ]))
            ])
        },
        issue: {
            filter: function (row) {
                return equals(row.approved, "Ok")
                    && isEmpty(row.wdone)
                    && EnterpriseUI.parseNumber(row.balqty) !== 0
                    && isEmpty(row.notappved)
                    && isEmpty(row.rpprocode);
            },
            fields: commonWorkflowFields([
                engineer,
                field("Current status", columns.safeTextColumn("curstatus")),
                field("Actions", actionColumn([
                    {
                        name: "issue",
                        label: "Issue parts",
                        icon: "bi bi-box-arrow-up-right"
                    }
                ]))
            ])
        },
        engineerCheck: {
            filter: function (row) {
                return equals(row.digstatus, "Diagnosis")
                    && isEmpty(row.wdone)
                    && matchesEmployee(row.empid)
                    && isEmpty(row.notappved)
                    && !isEmpty(row.empid)
                    && equals(row.approved, "Ok")
                    && EnterpriseUI.parseNumber(row.balqty) === 0;
            },
            fields: diagnosticFields("Current status", "edit-diagnosis", "Edit diagnosis")
        },
        qc: {
            filter: function (row) {
                return equals(row.wdone, "Ok") && isEmpty(row.qcdone);
            },
            fields: commonWorkflowFields([
                replacementWarranty,
                engineer,
                field("Actions", actionColumn([
                    {
                        name: "qc",
                        label: "QC check",
                        icon: "bi bi-clipboard-check",
                        className: "btn-outline-success",
                        visible: function (row) {
                            return !isSentinelDate(row.digndate);
                        }
                    }
                ]))
            ])
        },
        bill: {
            filter: function (row) {
                return equals(row.qcdone, "Ok") && isEmpty(row.billst);
            },
            fields: commonWorkflowFields([
                field("Replacement IMEI", columns.safeTextColumn("rimei")),
                replacementWarranty,
                engineer,
                field("Actions", actionColumn([
                    {
                        name: "bill",
                        label: "Create bill",
                        icon: "bi bi-receipt"
                    }
                ]))
            ])
        },
        delivery: {
            filter: function (row) {
                return (equals(row.qcdone, "Ok") && equals(row.billst, "Ok"))
                    || equals(row.retstatus, "R");
            },
            fields: [
                rowNumber,
                center,
                receivedDate,
                receiveNo,
                replacementWarranty,
                field("Courier", columns.wrappedTextColumn("bcname")),
                field("CN no.", columns.safeTextColumn("bcnno")),
                field("Booking date", columns.dateColumn("bcdate")),
                imei,
                customer,
                field("Customer address", columns.wrappedTextColumn("custadd")),
                product,
                field("Delivery date", columns.dateColumn("delvdat")),
                field("Delivery CN", columns.safeTextColumn("dcnno")),
                engineer,
                field("Current status", columns.safeTextColumn("fstatus")),
                field("Return status", columns.safeTextColumn("retstatus"))
            ]
        }
    };

    const ui = Object.assign({}, EnterpriseUI.dom, {
        initializeFilters(initialData) {
            EnterpriseUI.dates.initializeFilters([
                { txtFromDate: initialData.defaultFromDate },
                { txtToDate: initialData.defaultToDate }
            ], { overwrite: true });

            ui.loadSelect(
                "ddlServiceCenter",
                initialData.serviceCenters || [],
                "actcode",
                "actdesc",
                "Select service center",
                "000000000000"
            );
            ui.loadSelect(
                "ddlServiceType",
                initialData.serviceTypes || [],
                "gcod",
                "gdesc",
                "Select service type",
                "0000000"
            );
        },

        getFilters() {
            return {
                fromDate: ui.getValue("txtFromDate"),
                toDate: ui.getValue("txtToDate"),
                centerCode: ui.getValue("ddlServiceCenter"),
                serviceType: ui.getValue("ddlServiceType"),
                imei: ui.getValue("txtImei")
            };
        },

        renderCounters() {
            const summary = state.summary || {};
            const mappings = {
                receiveCount: "recv",
                transferCount: "trnsqty",
                assignCount: "engasig",
                diagnosisCount: "engdig",
                hotlineCount: "hotline",
                approvedCount: "approved",
                doaDapCount: "doadap",
                issueCount: "issue",
                engineerCheckCount: "jobinpro",
                qcCount: "qc",
                billCount: "bill",
                deliveryCount: "delqty"
            };

            Object.entries(mappings).forEach(function ([id, property]) {
                ui.setText(id, EnterpriseUI.parseNumber(summary[property]));
            });
        },

        selectCard(stage) {
            document.querySelectorAll("[data-interface]").forEach(function (card) {
                const selected = card.dataset.interface === stage;

                card.classList.toggle("active", selected);
                card.setAttribute("aria-pressed", String(selected));
            });
        },

        renderTable() {
            const config = stages[state.selectedStage];
            const table = document.getElementById("serviceInterfaceTable");
            const data = state.records.filter(config.filter);
            const title = document.querySelector(
                `[data-interface="${state.selectedStage}"] .interface-kpi-title`
            )?.textContent?.trim() || "Service records";

            EnterpriseUI.destroyDataTable("#serviceInterfaceTable");
            table.innerHTML = "";

            const head = table.createTHead();
            const row = head.insertRow();

            config.fields.forEach(function (item) {
                const heading = document.createElement("th");

                heading.scope = "col";
                heading.textContent = item.title;
                row.appendChild(heading);
            });

            table.createTBody();
            state.table = EnterpriseUI.createDataTable(
                "#serviceInterfaceTable",
                {
                    enterpriseTitle: title,
                    data: data,
                    columns: config.fields.map(function (item) {
                        return item.definition;
                    }),
                    deferRender: true,
                    processing: true,
                    autoWidth: true,
                    responsive: false,
                    stateSave: false,
                    order: [],
                    language: {
                        emptyTable: "No service records match the selected filters.",
                        processing: "Loading service records..."
                    }
                }
            );

            window.requestAnimationFrame(function () {
                if (state.table) {
                    state.table.columns.adjust().draw(false);
                }
            });
        },

        showError(error, fallback) {
            Notifications.handleError(error, fallback);
        }
    });

    function commonWorkflowFields(additionalFields) {
        return [
            rowNumber,
            center,
            receivedDate,
            receiveNo,
            imei,
            product,
            deliveryDate
        ].concat(additionalFields || []);
    }

    function diagnosticFields(statusTitle, actionName, actionLabel) {
        return commonWorkflowFields([
            customer,
            problem,
            engineer,
            field(statusTitle, columns.safeTextColumn("digstatus")),
            field("Actions", actionColumn([
                {
                    name: actionName,
                    label: actionLabel,
                    icon: "bi bi-tools"
                }
            ]))
        ]);
    }

    function dueDateColumn(property) {
        return columns.dateColumn(property, {
            createdCell: function (cell, value, row) {
                const date = EnterpriseUI.dates.parse(value);
                const today = EnterpriseUI.dates.parse(new Date());

                if (!date || !today || equals(row.fstatus, "Delivery")) {
                    return;
                }

                const days = Math.floor((today - date) / 86400000);

                cell.classList.toggle("is-due-soon", days >= 0 && days <= 3);
            }
        });
    }

    function normalize(value) {
        return String(value || "").trim().toLowerCase();
    }

    function equals(value, expected) {
        return normalize(value) === normalize(expected);
    }

    function isEmpty(value) {
        return normalize(value) === "";
    }

    function isSentinelDate(value) {
        const date = EnterpriseUI.dates.parse(value);

        return !date || date.getFullYear() === 1900;
    }

    function matchesEmployee(employeeId) {
        const pattern = String(state.summary?.empid || "").trim();

        if (!pattern || pattern === "%") {
            return true;
        }

        const expected = pattern.replaceAll("%", "");

        return String(employeeId || "").includes(expected);
    }

    function validateFilters(filters) {
        const validation = EnterpriseUI.dates.validateRange(
            filters.fromDate,
            filters.toDate,
            {
                requiredMessage: "From date and To date are required.",
                invalidMessage: "Enter dates in dd-MMM-yyyy format."
            }
        );

        if (!validation.valid) {
            Notifications.Toastr.error(validation.message);
        }

        return validation.valid;
    }

    async function loadInterfaceData() {
        const filters = ui.getFilters();

        if (!validateFilters(filters)) {
            return;
        }

        try {
            const result = await service.loadInterfaceData(filters);

            state.records = result?.records || [];
            state.summary = result?.summary || {};
            ui.renderCounters();
            ui.renderTable();
        }
        catch (error) {
            state.records = [];
            state.summary = {};
            ui.renderCounters();
            ui.renderTable();
            ui.showError(error, "Service interface data could not be loaded.");
        }
    }

    function selectStage(stage) {
        if (!stages[stage]) {
            return;
        }

        state.selectedStage = stage;
        ui.selectCard(stage);
        ui.renderTable();
    }

    function routeForAction(action, row) {
        const date = EnterpriseUI.dates.serialize(row.recvdate, "display") || row.recvdate;
        const common = {
            centrid: row.centrid,
            wrRecvno: row.recvno || row.recvno1,
            imesimeno: row.mimei || row.simei
        };
        const routes = {
            "edit-receive": ["/F_20_Service/RecProductEntry", { Type: "Edit", ...common }],
            transfer: ["/F_20_Service/AllRecProduct", { Type: "Transfer", date: date, genno: `${row.centrid}${row.recvno}` }],
            assign: ["/F_20_Service/AllRecProduct", { Type: "AssignEng", date: date, genno: `${row.centrid}${row.recvno}` }],
            diagnose: ["/F_20_Service/DiagProductEntry", { Type: "DiagEnrty", ...common }],
            hotline: ["/F_20_Service/AllRecProduct", { Type: "HotLine", date: date, genno: `${row.centrid}${row.recvno}` }],
            return: ["/F_20_Service/CustProReturn", { Type: "Entry", actcode: row.centrid, genno: row.recvno, date: date }],
            approve: ["/F_20_Service/DiagProductEntry", { Type: "Approved", ...common }],
            replacement: ["/F_20_Service/DiagProductEntry", { Type: "ProRep", ...common }],
            issue: ["/F_20_Service/ExecutionProduct", { Type: "Issue", actcode: row.centrid, genno: row.recvno }],
            "edit-diagnosis": ["/F_20_Service/DiagProductEntry", { Type: "Edit", ...common }],
            qc: ["/F_20_Service/DiagProductEntry", { Type: "QC", ...common }],
            bill: ["/F_20_Service/ServBill", { Type: "All", date: date, genno: `${row.centrid}${row.recvno}` }]
        };

        return routes[action] || null;
    }

    function openAction(button) {
        const row = EnterpriseUI.getResponsiveRowData(
            "#serviceInterfaceTable",
            button
        );
        const route = routeForAction(button.dataset.action, row || {});

        if (!route) {
            Notifications.Toastr.error("This service action is not available.");
            return;
        }

        window.open(`${route[0]}?${new URLSearchParams(route[1]).toString()}`, "_blank");
    }

    function bindEvents() {
        document.querySelectorAll("[data-interface]").forEach(function (card) {
            card.addEventListener("click", function () {
                selectStage(card.dataset.interface);
            });
        });

        document.getElementById("btnLoad")?.addEventListener("click", loadInterfaceData);
        document.getElementById("txtImei")?.addEventListener("keydown", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();
                loadInterfaceData();
            }
        });
        document.getElementById("btnNewReceive")?.addEventListener("click", function () {
            window.open("/F_20_Service/RecProductEntry?Type=Entry", "_blank");
        });
        $("#serviceInterfaceTable").on(
            "click",
            ".interface-action-button",
            function () {
                openAction(this);
            }
        );
    }

    async function loadInitialData() {
        try {
            const initialData = await service.loadInitialData();

            ui.initializeFilters(initialData || {});
            await loadInterfaceData();
        }
        catch (error) {
            ui.showError(error, "Service interface filters could not be loaded.");
        }
    }

    function init() {
        EnterpriseUI.initialize(document);
        bindEvents();
        loadInitialData();
    }

    return { init: init };
})();

document.addEventListener("DOMContentLoaded", ServiceInterfacePage.init);
