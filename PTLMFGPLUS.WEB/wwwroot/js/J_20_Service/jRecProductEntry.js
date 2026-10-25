const commonPath = "/F_20_Service/RecProductEntry/";

var RecProductEntryPage = (function () {
    "use strict";

    const query = new URLSearchParams(window.location.search);
    const state = {
        type: query.get("Type") || "Entry",
        requestedReceiveNo: query.get("wrRecvno") || "",
        requestedCenterId: query.get("centrid") || "",
        requestedImei: query.get("imesimeno") || "",
        initial: null,
        product: null,
        lines: [],
        saving: false
    };

    const service = {
        loadInitialData() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetInitialData`));
            });
        },

        getReceiveNo(date, centerId) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetReceiveNo`),
                { date: date, centerId: centerId }
            );
        },

        getProduct(imei, editing) {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}GetProduct`),
                    { imei: imei, editing: editing }
                );
            });
        },

        getEdit() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}GetEdit`),
                    {
                        receiveNo: state.requestedReceiveNo,
                        centerId: state.requestedCenterId,
                        imei: state.requestedImei
                    }
                );
            });
        },

        save(request) {
            return Helpers.withLoader(function () {
                return FetchHelpers.postJson(FetchHelpers.urlconfig(`${commonPath}Save`), request);
            });
        }
    };

    const ui = Object.assign({}, EnterpriseUI.dom, {
        value(id) {
            return document.getElementById(id)?.value?.trim() || "";
        },

        setValue(id, value) {
            const element = document.getElementById(id);
            if (element) element.value = value ?? "";
        },

        notify(title, message, type) {
            if (typeof Notifications === "function") {
                Notifications(title, message, type);
                return;
            }
            toastr[type === "warning" ? "warning" : type === "success" ? "success" : "error"](message);
        },

        select(id, items, valueField, textField, placeholder, selectedValue) {
            Helpers.loadSelect2WithData({
                selector: id,
                data: items || [],
                valueField: valueField,
                textField: textField,
                selectedValue: selectedValue || "",
                placeholder: placeholder
            });
        },

        setSelect(id, value) {
            $(`#${id}`).val(value || "").trigger("change.select2");
        },

        showWorkspace(show) {
            const badge = document.getElementById("productStatus");
            badge.className = show ? "badge text-bg-success" : "badge text-bg-secondary";
            badge.textContent = show ? "Loaded" : "Not loaded";
        },

        applyCompanyLayout() {
            const vehicleCompany = state.initial.companyCode === "8421";

            document.getElementById("vehicleFields").hidden = !vehicleCompany;
            document.getElementById("bookingDateField").hidden = vehicleCompany;
            document.getElementById("courierFields").hidden = vehicleCompany;
        },

        setDate(id, value) {
            const parsed = EnterpriseUI.dates.parse(value);
            if (parsed) $(`#${id}`).datepicker("setDate", parsed);
        },

        renderLines() {
            const problems = state.lines.filter(function (item) { return item.grp === "A"; });
            const accessories = state.lines.filter(function (item) { return item.grp === "B"; });
            document.querySelector("#problemTable tbody").innerHTML = problems.length
                ? problems.map(function (item) {
                    const index = state.lines.indexOf(item);
                    return `<tr><td>${EnterpriseUI.encodeText(item.gdesc)}</td><td><input class="form-control ui-size-xs problem-remarks" data-index="${index}" value="${EnterpriseUI.encodeText(item.remarks || "")}"></td><td><button type="button" class="btn btn-outline-danger ui-size-xs remove-line" data-index="${index}" aria-label="Remove problem"><i class="bi bi-trash" aria-hidden="true"></i></button></td></tr>`;
                }).join("")
                : '<tr><td colspan="3" class="text-center text-muted">No problems added.</td></tr>';
            document.querySelector("#accessoryTable tbody").innerHTML = accessories.length
                ? accessories.map(function (item) {
                    const index = state.lines.indexOf(item);
                    return `<tr><td>${EnterpriseUI.encodeText(item.gdesc)}</td><td><button type="button" class="btn btn-outline-danger ui-size-xs remove-line" data-index="${index}" aria-label="Remove accessory"><i class="bi bi-trash" aria-hidden="true"></i></button></td></tr>`;
                }).join("")
                : '<tr><td colspan="2" class="text-center text-muted">No accessories added.</td></tr>';
        },

        renderProduct(product) {
            state.product = product;
            ui.setValue("txtImei", product.mimei || product.simei);
            ui.setValue("txtProduct", product.proddesc);
            ui.setValue("txtSalesCenter", product.actdesc);
            ui.setValue("txtDistributor", product.custname);
            ui.setValue("txtDistributorAddress", product.distadd);
            ui.setValue("txtSalesDate", EnterpriseUI.dates.format(product.sdeldate));
            ui.setDate("txtActivationDate", product.activedate || state.initial.activationDate);
            ui.setValue("txtOdometer", product.odometer || "");
            ui.setValue("txtRegistration", product.regno);
            const serviceType = product.nextservtype || product.rwarststus;
            if (serviceType) ui.setSelect("ddlServiceType", serviceType);
            if (state.initial.companyCode === "8421") {
                ui.setSelect("ddlCustomer", product.custid);
                ui.setValue("txtCustomerName", product.actualowner || product.custname);
                ui.setValue("txtMobile", product.actualownerphone);
                ui.setValue("txtAddress", product.actualowneradd || product.distadd);
            }
            ui.showWorkspace(true);
        }
    });

    async function updateReceiveNo() {
        if (state.type.toLowerCase() === "edit") return;
        const centerId = ui.value("ddlServiceCenter");
        const date = ui.value("txtEntryDate");
        if (!centerId || !date) return;
        try {
            const result = await service.getReceiveNo(date, centerId);
            ui.setValue("txtReceiveNo", result.receiveNo);
        } catch (error) {
            ui.notify("Error", error?.objError || "Receive number could not be generated.", "warning");
        }
    }

    async function findProduct() {
        const imei = ui.value("txtImei");
        if (!imei) {
            ui.notify("Required", "Enter an IMEI or serial number.", "warning");
            return;
        }
        try {
            const response = await service.getProduct(imei, state.type.toLowerCase() === "edit");
            ui.renderProduct(response.product);
        } catch (error) {
            ui.showWorkspace(false);
            ui.notify("Error", error?.objError || "Product information could not be loaded.", "warning");
        }
    }

    function addLine(group, selectId) {
        const select = document.getElementById(selectId);
        const code = select.value;
        if (!code) return;
        if (state.lines.some(function (item) { return item.grp === group && item.gcod === code; })) {
            ui.notify("Duplicate", "This item is already added.", "warning");
            return;
        }
        state.lines.push({ grp: group, gcod: code, gdesc: select.options[select.selectedIndex].text, remarks: "" });
        ui.renderLines();
    }

    function requestModel() {
        document.querySelectorAll(".problem-remarks").forEach(function (input) {
            state.lines[Number(input.dataset.index)].remarks = input.value.trim();
        });
        return {
            type: state.type,
            previousReceiveNo: state.requestedReceiveNo,
            receiveNo: ui.value("txtReceiveNo"),
            centerId: ui.value("ddlServiceCenter"),
            imei: ui.value("txtImei"),
            receiveDate: ui.value("txtEntryDate"),
            probableDeliveryDate: ui.value("txtProbableDate"),
            activationDate: ui.value("txtActivationDate"),
            bookingDate: ui.value("txtBookingDate"),
            serviceType: ui.value("ddlServiceType"),
            customerId: ui.value("ddlCustomer"),
            customerName: ui.value("txtCustomerName"),
            customerMobile: ui.value("txtMobile"),
            customerMobile2: ui.value("txtMobile2"),
            customerEmail: ui.value("txtEmail"),
            customerAddress: ui.value("txtAddress"),
            customerComplaint: ui.value("txtComplaint"),
            missingItems: ui.value("txtMissing"),
            transportCode: ui.value("ddlTransport"),
            consignmentNo: ui.value("txtConsignment"),
            odometer: ui.value("txtOdometer"),
            registrationNo: ui.value("txtRegistration"),
            mechanicId: ui.value("ddlMechanic"),
            advance: EnterpriseUI.parseNumber(ui.value("txtAdvance")),
            lines: state.lines
        };
    }

    async function save() {
        if (state.saving || !state.product) {
            if (!state.product) ui.notify("Required", "Load product information before saving.", "warning");
            return;
        }
        state.saving = true;
        Helpers.setPending("#btnFooterSave", true);
        try {
            await service.save(requestModel());
            ui.notify("Saved", `Receive ${ui.value("txtReceiveNo")} was saved.`, "success");
            state.type = "Edit";
            state.requestedReceiveNo = ui.value("txtReceiveNo");
        } catch (error) {
            ui.notify("Not saved", error?.objError || "The receive record could not be saved.", "warning");
        } finally {
            state.saving = false;
            Helpers.setPending("#btnFooterSave", false);
        }
    }

    function bindEvents() {
        $("#ddlServiceCenter").on("change", updateReceiveNo);
        $("#txtEntryDate").on("changeDate change", updateReceiveNo);
        $("#btnFindProduct").on("click", findProduct);
        $("#txtImei").on("change", findProduct);
        $("#txtImei").on("keydown", function (event) { if (event.key === "Enter") { event.preventDefault(); findProduct(); } });
        $("#ddlCustomer").on("change", function () {
            const customer = state.initial.customers.find(function (item) { return item.sircode === ui.value("ddlCustomer"); });
            if (!customer || customer.sircode === "000000000000") return;
            ui.setValue("txtCustomerName", customer.sirdesc);
            ui.setValue("txtMobile", customer.phone);
            ui.setValue("txtAddress", customer.addre);
            ui.setValue("txtEmail", customer.email);
        });
        $("#btnAddProblem").on("click", function () { addLine("A", "ddlProblem"); });
        $("#btnAddAccessory").on("click", function () { addLine("B", "ddlAccessory"); });
        $(document).on("click", ".remove-line", function () { state.lines.splice(Number(this.dataset.index), 1); ui.renderLines(); });
        $("#btnFooterSave").on("click", save);
        $("#btnFooterRefresh").on("click", function () { window.location.reload(); });
        $("#btnFooterCancel").on("click", function () { window.location.href = "/F_20_Service/RptServInterface?Type=All"; });
    }

    async function loadEdit() {
        const response = await service.getEdit();
        const header = response.header;
        state.lines = response.lines || [];
        ui.setSelect("ddlServiceCenter", header.centrid || state.requestedCenterId);
        ui.setValue("txtReceiveNo", header.recvno || state.requestedReceiveNo);
        ui.setDate("txtEntryDate", header.recvdate);
        ui.setDate("txtProbableDate", header.deldate);
        ui.setDate("txtBookingDate", header.bcdate || state.initial.bookingDate);
        ui.setValue("txtImei", header.mimei || state.requestedImei);
        ui.setSelect("ddlServiceType", header.wrcode);
        ui.setSelect("ddlCustomer", header.custid);
        ui.setSelect("ddlTransport", header.bcsircode);
        ui.setSelect("ddlMechanic", header.empid);
        ui.setValue("txtCustomerName", header.custname);
        ui.setValue("txtMobile", header.custmob);
        ui.setValue("txtMobile2", header.custmob2);
        ui.setValue("txtEmail", header.custmail);
        ui.setValue("txtAddress", header.custadd);
        ui.setValue("txtComplaint", header.custop);
        ui.setValue("txtMissing", header.hsmissing);
        ui.setValue("txtConsignment", header.bcnno);
        ui.setValue("txtOdometer", header.odometer);
        ui.setValue("txtRegistration", header.regno);
        ui.setValue("txtAdvance", header.advance || 0);
        ui.renderProduct(response.product);
        ui.renderLines();
        document.getElementById("ddlServiceCenter").disabled = true;
        document.getElementById("txtImei").readOnly = true;
    }

    async function loadInitialData() {
        try {
            state.initial = await service.loadInitialData();
            ui.applyCompanyLayout();
            ui.select("ddlServiceCenter", state.initial.serviceCenters, "code", "description", "Select service center", state.requestedCenterId);
            ui.select("ddlServiceType", state.initial.serviceTypes, "code", "description", "Select service type");
            ui.select("ddlProblem", state.initial.problems, "code", "description", "Select problem");
            ui.select("ddlAccessory", state.initial.accessories, "code", "description", "Select accessory");
            ui.select("ddlTransport", state.initial.transports, "code", "description", "Select courier", "000000000000");
            ui.select("ddlMechanic", state.initial.mechanics, "code", "description", "Select engineer");
            ui.select("ddlCustomer", state.initial.customers, "sircode", "sirdesc", "Select customer", "000000000000");
            ui.setDate("txtEntryDate", state.initial.entryDate);
            ui.setDate("txtProbableDate", state.initial.probableDeliveryDate);
            ui.setDate("txtActivationDate", state.initial.activationDate);
            ui.setDate("txtBookingDate", state.initial.bookingDate);
            ui.renderLines();
            if (state.type.toLowerCase() === "edit") await loadEdit();
            else await updateReceiveNo();
        } catch (error) {
            ui.notify("Error", error?.objError || "Receive entry could not be initialized.", "warning");
        }
    }

    function init() {
        ShowFooterWithButtons();
        HideRecalculate();
        bindEvents();
        loadInitialData();
    }

    return { init: init };
})();

document.addEventListener("DOMContentLoaded", RecProductEntryPage.init);
