const commonPath = "/F_20_Service/AllRecProduct/";

var AllRecProductPage = (function () {
    "use strict";

    const query = new URLSearchParams(window.location.search);
    const state = {
        type: query.get("Type") || "All",
        date: query.get("date") || "",
        genno: query.get("genno") || "",
        records: [],
        table: null,
        selected: null,
        assignmentOptions: [],
        modal: null
    };
    const columns = EnterpriseUI.dataTableColumns;

    const service = {
        initial() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetInitialData`), { date: state.date });
            });
        },

        records(filters) {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetRecords`), filters);
            });
        },

        employees() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetEmployees`), { type: state.type });
            });
        },

        update(action, request) {
            return Helpers.withLoader(function () {
                return FetchHelpers.postJson(FetchHelpers.urlconfig(`${commonPath}${action}`), request);
            });
        }
    };

    const ui = Object.assign({}, EnterpriseUI.dom, {
        value(id) {
            return document.getElementById(id)?.value?.trim() || "";
        },

        setDate(id, value) {
            const date = EnterpriseUI.dates.parse(value);
            if (date) $(`#${id}`).datepicker("setDate", date);
        },

        notify(message, type) {
            if (type === "success") Notifications.Toastr.success(message);
            else Notifications.Toastr.error(message);
        },

        action(name, label, icon, className) {
            return {
                name: name,
                label: label,
                title: label,
                icon: icon,
                iconOnly: true,
                className: `btn btn-sm ${className || "btn-outline-primary"} all-rec-action`
            };
        },

        fields() {
            const common = [
                { title: "#", data: columns.rowNumberColumn() },
                { title: "Service center", data: columns.wrappedTextColumn("actdesc") },
                { title: "Received date", data: columns.dateColumn("recvdate") },
                { title: "Receive no.", data: columns.safeTextColumn("recvno1") },
                { title: "Warranty status", data: columns.wrappedTextColumn("wrstatusdesc") },
                { title: "IMEI", data: columns.safeTextColumn("mimei") },
                { title: "Customer", data: columns.wrappedTextColumn("custname") },
                { title: "Product", data: columns.wrappedTextColumn("prodesc") },
                { title: "Problem", data: columns.wrappedTextColumn("probdetails") },
                { title: "Probable delivery", data: columns.dateColumn("deldate") }
            ];
            if (state.type === "Transfer") {
                common.splice(5, 0,
                    { title: "Courier", data: columns.wrappedTextColumn("bcname") },
                    { title: "CN no.", data: columns.safeTextColumn("bcnno") },
                    { title: "Booking date", data: columns.dateColumn("bcdate") }
                );
                common.push({ title: "Transfer status", data: columns.safeTextColumn("trnstatus") });
                common.push({ title: "Action", data: columns.actionColumn([ui.action("transfer", "Mark transferred", "bi bi-arrow-left-right")]) });
            } else if (state.type === "AssignEng") {
                common.push({ title: "Engineer", data: columns.wrappedTextColumn("empname") });
                common.push({ title: "Action", data: columns.actionColumn([ui.action("assign", "Assign engineer", "bi bi-person-check")]) });
            } else if (state.type === "HotLine") {
                common.push({ title: "Hotline status", data: columns.wrappedTextColumn("cutstdesc") });
                common.push({ title: "Hotline message", data: columns.wrappedTextColumn("custhtmsg") });
                common.push({ title: "Action", data: columns.actionColumn([ui.action("hotline", "Process hotline", "bi bi-headset")]) });
            } else {
                common.push({ title: "Engineer", data: columns.wrappedTextColumn("empname") });
                common.push({ title: "Action", data: columns.actionColumn([ui.action("edit", "Edit receive", "bi bi-pencil-square", "btn-outline-dark")]) });
            }
            return common;
        },

        renderTable() {
            const table = document.getElementById("allRecProductTable");
            const fields = ui.fields();
            EnterpriseUI.destroyDataTable("#allRecProductTable");
            table.innerHTML = "";
            const row = table.createTHead().insertRow();
            fields.forEach(function (field) {
                const th = document.createElement("th");
                th.textContent = field.title;
                row.appendChild(th);
            });
            table.createTBody();
            state.table = EnterpriseUI.createDataTable("#allRecProductTable", {
                enterpriseTitle: document.getElementById("listHeading").textContent,
                data: state.records,
                columns: fields.map(function (field) { return field.data; }),
                responsive: false,
                autoWidth: true,
                stateSave: false,
                order: [],
                language: { emptyTable: "No service records match the selected filters." }
            });
            document.getElementById("recordCount").textContent = `${state.records.length} records`;
            requestAnimationFrame(function () { state.table.columns.adjust().draw(false); });
        },

        configureMode() {
            const settings = {
                All: ["All received products", true],
                Transfer: ["Products awaiting transfer", false],
                AssignEng: ["Assign products to engineer", false],
                HotLine: ["Hotline products", false]
            }[state.type] || ["Received products", false];
            document.getElementById("listHeading").textContent = settings[0];
            document.getElementById("btnNewReceived").hidden = !settings[1];
            document.title = settings[0];
        },

        showAssignment(row, hotline) {
            state.selected = row;
            document.getElementById("assignmentModalTitle").textContent = hotline ? "Process hotline" : "Assign engineer";
            document.getElementById("assignmentLabel").textContent = hotline ? "Hotline status" : "Engineer";
            document.getElementById("hotlineMessageField").hidden = !hotline;
            document.getElementById("txtHotlineMessage").value = row.custhtmsg || "";
            Helpers.loadSelect2WithData({
                selector: "ddlEmployee",
                data: state.assignmentOptions,
                valueField: "code",
                textField: "description",
                selectedValue: hotline ? row.custstatus : row.empid,
                placeholder: hotline ? "Select hotline status" : "Select engineer",
                dropdownParent: "#assignmentModal"
            });
            state.modal.show();
        }
    });

    function filters() {
        const genCenter = state.genno.length >= 12 ? state.genno.substring(0, 12) : "";
        const genReceive = state.genno.length > 12 ? state.genno.substring(12) : "";
        return {
            type: state.type,
            fromDate: ui.value("txtFromDate"),
            toDate: ui.value("txtToDate"),
            centerId: ui.value("ddlServiceCenter") || genCenter || "000000000000",
            receiveNo: ui.value("txtReceiveNo") || genReceive
        };
    }

    async function loadRecords() {
        try {
            state.records = await service.records(filters());
            ui.renderTable();
        } catch (error) {
            state.records = [];
            ui.renderTable();
            Notifications.handleError(error, "Service records could not be loaded.");
        }
    }

    function requestFor(row) {
        return {
            centerId: row.centrid,
            receiveNo: row.recvno || row.recvno1,
            employeeId: ui.value("ddlEmployee"),
            hotlineMessage: ui.value("txtHotlineMessage"),
            transferred: true
        };
    }

    async function handleAction(button) {
        const row = EnterpriseUI.getResponsiveRowData("#allRecProductTable", button);
        if (!row) return;
        if (button.dataset.action === "edit") {
            const params = new URLSearchParams({ Type: "Edit", centrid: row.centrid, wrRecvno: row.recvno || row.recvno1, imesimeno: row.mimei || row.simei });
            window.location.href = `/F_20_Service/RecProductEntry?${params}`;
            return;
        }
        if (button.dataset.action === "transfer") {
            try {
                await service.update("UpdateTransfer", requestFor(row));
                ui.notify("Product transfer status was updated.", "success");
                await loadRecords();
            } catch (error) {
                Notifications.handleError(error, "Transfer status could not be updated.");
            }
            return;
        }
        ui.showAssignment(row, button.dataset.action === "hotline");
    }

    async function saveAssignment() {
        if (!state.selected || !ui.value("ddlEmployee")) {
            ui.notify("Select an assignment option.", "error");
            return;
        }
        Helpers.setPending("#btnSaveAssignment", true);
        try {
            await service.update(state.type === "HotLine" ? "UpdateHotline" : "AssignEmployee", requestFor(state.selected));
            state.modal.hide();
            ui.notify("Service assignment was updated.", "success");
            await loadRecords();
        } catch (error) {
            Notifications.handleError(error, "Service assignment could not be updated.");
        } finally {
            Helpers.setPending("#btnSaveAssignment", false);
        }
    }

    function bindEvents() {
        $("#btnShow").on("click", loadRecords);
        $("#btnNewReceived").on("click", function () { window.location.href = "/F_20_Service/RecProductEntry?Type=Entry"; });
        $("#allRecProductTable").on("click", ".all-rec-action", function () { handleAction(this); });
        $("#btnSaveAssignment").on("click", saveAssignment);
    }

    async function initialize() {
        ui.configureMode();
        state.modal = bootstrap.Modal.getOrCreateInstance(document.getElementById("assignmentModal"));
        bindEvents();
        try {
            const initial = await service.initial();
            const center = state.genno.length >= 12 ? state.genno.substring(0, 12) : "000000000000";
            Helpers.loadSelect2WithData({ selector: "ddlServiceCenter", data: initial.serviceCenters, valueField: "code", textField: "description", selectedValue: center });
            ui.setDate("txtFromDate", initial.fromDate);
            ui.setDate("txtToDate", initial.toDate);
            if (state.genno.length > 12) document.getElementById("txtReceiveNo").value = state.genno.substring(12);
            if (state.type === "AssignEng" || state.type === "HotLine") state.assignmentOptions = await service.employees();
            await loadRecords();
        } catch (error) {
            Notifications.handleError(error, "Service product page could not be initialized.");
        }
    }

    return { init: initialize };
})();

document.addEventListener("DOMContentLoaded", AllRecProductPage.init);
