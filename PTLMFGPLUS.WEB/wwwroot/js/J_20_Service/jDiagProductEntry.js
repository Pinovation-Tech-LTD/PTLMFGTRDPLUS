const commonPath = "/F_20_Service/DiagProductEntry/";

var DiagProductEntryPage = (function () {
    "use strict";

    const query = new URLSearchParams(window.location.search);
    const state = {
        type: query.get("Type") || "DiagEnrty",
        centerId: query.get("centrid") || "",
        receiveNo: query.get("wrRecvno") || "",
        imei: query.get("imesimeno") || "",
        data: null,
        problems: [],
        parts: [],
        saving: false
    };

    const service = {
        load() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}Load`),
                    {
                        centerId: state.centerId,
                        receiveNo: state.receiveNo,
                        imei: state.imei,
                        date: ui.value("txtDiagnosisDate")
                    }
                );
            });
        },

        getSpareOptions(date) {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetSpareOptions`),
                {
                    centerId: state.centerId,
                    date: date
                }
            );
        },

        post(action, request) {
            return Helpers.withLoader(function () {
                return FetchHelpers.postJson(
                    FetchHelpers.urlconfig(`${commonPath}${action}`),
                    request
                );
            });
        }
    };

    const ui = {
        value(id) {
            return document.getElementById(id)?.value?.trim() || "";
        },

        setValue(id, value) {
            const element = document.getElementById(id);
            if (element) {
                element.value = value ?? "";
            }
        },

        setDate(id, value) {
            const parsed = EnterpriseUI.dates.parse(value);
            if (parsed) {
                $(`#${id}`).datepicker("setDate", parsed);
            }
        },

        notify(title, message, type) {
            if (typeof Notifications === "function") {
                Notifications(title, message, type);
                return;
            }
            toastr[type === "success" ? "success" : type === "warning" ? "warning" : "error"](message);
        },

        select(id, items, selectedValue, placeholder) {
            Helpers.loadSelect2WithData({
                selector: id,
                data: items || [],
                valueField: "code",
                textField: "description",
                selectedValue: selectedValue || "",
                placeholder: placeholder
            });
        },

        setMode() {
            const mode = state.type.toLowerCase();
            const editable = mode === "diagenrty" || mode === "edit";
            const approval = mode === "approved";
            const qc = mode === "qc";
            const replacement = mode === "prorep";
            const labels = {
                diagenrty: "Diagnosis entry",
                edit: "Edit diagnosis",
                approved: "Parts approval",
                qc: "Quality control",
                prorep: "Replacement request"
            };

            document.getElementById("modeBadge").textContent = labels[mode] || "Product diagnosis";
            document.getElementById("problemEntry").hidden = !editable;
            document.getElementById("partEntry").hidden = !editable;
            document.getElementById("replacementField").hidden = !(replacement || state.parts.some(function (part) {
                return part.rsircode.startsWith("4299");
            }));
            document.getElementById("btnServiceDone").hidden = !(mode === "edit" || replacement);
            document.getElementById("btnQcReject").hidden = !qc;
            document.getElementById("txtDiagnosisDate").readOnly = !(editable || qc);
            document.getElementById("ddlWarranty").disabled = !editable;
            document.getElementById("txtPartsRemarks").readOnly = !editable;

            if (approval) {
                HideSave();
                ShowApprove();
            } else {
                HideApprove();
                if (qc || editable || replacement) {
                    document.getElementById("btnFooterSave").style.display = "inline-block";
                    document.getElementById("btnFooterSave").innerHTML = qc
                        ? '<i class="bi bi-check2-circle" aria-hidden="true"></i> QC pass'
                        : '<i class="bi bi-save" aria-hidden="true"></i> Save';
                } else {
                    HideSave();
                }
            }
        },

        renderReference() {
            const problems = state.data.receivedProblems || [];
            const accessories = state.data.accessories || [];
            document.getElementById("referencePanel").hidden = problems.length === 0 && accessories.length === 0;
            document.getElementById("receivedProblems").innerHTML = problems.length
                ? problems.map(function (item) {
                    return `<li>${EnterpriseUI.encodeText(item.gdesc)}</li>`;
                }).join("")
                : '<li class="text-muted">No reported problems.</li>';
            document.getElementById("receivedAccessories").innerHTML = accessories.length
                ? accessories.map(function (item) {
                    return `<li>${EnterpriseUI.encodeText(item.gdesc)}</li>`;
                }).join("")
                : '<li class="text-muted">No accessories recorded.</li>';
        },

        renderProblems() {
            const editable = ["diagenrty", "edit"].includes(state.type.toLowerCase());
            document.querySelector("#diagnosisTable tbody").innerHTML = state.problems.length
                ? state.problems.map(function (item, index) {
                    return `
                        <tr>
                            <td>${EnterpriseUI.encodeText(item.gdesc)}</td>
                            <td>
                                <input type="text"
                                       class="form-control ui-size-xs diagnosis-remark"
                                       data-index="${index}"
                                       value="${EnterpriseUI.encodeText(item.remarks || "")}"
                                       ${editable ? "" : "readonly"} />
                            </td>
                            <td>
                                ${editable ? `
                                    <button type="button"
                                            class="btn btn-outline-danger ui-size-xs remove-problem"
                                            data-index="${index}"
                                            aria-label="Remove problem">
                                        <i class="bi bi-trash" aria-hidden="true"></i>
                                    </button>
                                ` : ""}
                            </td>
                        </tr>
                    `;
                }).join("")
                : '<tr><td colspan="3" class="text-center text-muted">No diagnosis problems added.</td></tr>';
        },

        renderParts() {
            const mode = state.type.toLowerCase();
            const editable = ["diagenrty", "edit"].includes(mode);
            const approval = mode === "approved";
            const qc = mode === "qc";
            document.querySelector("#partsTable tbody").innerHTML = state.parts.length
                ? state.parts.map(function (part, index) {
                    const amount = EnterpriseUI.parseNumber(part.qty) * EnterpriseUI.parseNumber(part.sirval);
                    part.amount = amount;
                    return `
                        <tr>
                            <td>${EnterpriseUI.encodeText(part.sirdesc)}</td>
                            <td>${EnterpriseUI.encodeText(part.sirunit || "")}</td>
                            <td class="text-end">${formatNumber(part.stkqty)}</td>
                            <td>
                                <input type="number"
                                       min="0"
                                       step="0.01"
                                       class="form-control ui-size-xs text-end part-qty"
                                       data-index="${index}"
                                       value="${formatInput(part.qty)}"
                                       ${editable ? "" : "readonly"} />
                            </td>
                            <td class="text-end">${formatNumber(part.sirval)}</td>
                            <td class="text-end part-amount">${formatNumber(amount)}</td>
                            <td class="text-center">
                                <input type="checkbox"
                                       class="form-check-input part-approved"
                                       data-index="${index}"
                                       ${isApproved(part.apstatus) ? "checked" : ""}
                                       ${approval ? "" : "disabled"} />
                            </td>
                            <td>
                                <input type="number"
                                       min="0"
                                       step="0.01"
                                       class="form-control ui-size-xs text-end part-issue"
                                       data-index="${index}"
                                       value="${formatInput(part.execqty)}"
                                       ${qc ? "" : "readonly"} />
                            </td>
                            <td>
                                ${editable ? `
                                    <button type="button"
                                            class="btn btn-outline-danger ui-size-xs remove-part"
                                            data-index="${index}"
                                            aria-label="Remove spare part">
                                        <i class="bi bi-trash" aria-hidden="true"></i>
                                    </button>
                                ` : ""}
                            </td>
                        </tr>
                    `;
                }).join("")
                : '<tr><td colspan="9" class="text-center text-muted">No spare parts added.</td></tr>';
            document.getElementById("partsTotal").textContent = formatNumber(state.parts.reduce(function (total, item) {
                return total + EnterpriseUI.parseNumber(item.amount);
            }, 0));
            ui.setMode();
        },

        renderHeader() {
            const header = state.data.header;
            const product = state.data.product;
            ui.setValue("txtServiceCenter", header.actdesc || state.centerId);
            ui.setValue("txtSalesCenter", product.actdesc);
            ui.setValue("txtDistributor", product.custname);
            ui.setValue("txtDistributorAddress", product.distadd);
            ui.setValue("txtModel", product.proddesc || header.rsirdesc);
            ui.setValue("txtImei", header.mimei || state.imei);
            ui.setValue("txtCustomer", header.custname);
            ui.setValue("txtMobile", header.custmob || header.custmob2);
            ui.setValue("txtEmail", header.custmail);
            ui.setValue("txtAddress", header.custadd);
            ui.setValue("txtComplaint", header.custop);
            ui.setValue("txtReceiveNo", header.recvno || state.receiveNo);
            ui.setValue("txtReceiveDate", header.recvdate);
            ui.setDate("txtDiagnosisDate", header.digndate || new Date());
            ui.setValue("txtDeliveryDate", header.deldate);
            ui.setValue("txtReplacementImei", header.rimei);
            ui.setValue("txtMissing", header.hsmissing);
            ui.setValue("txtPartsRemarks", state.parts[0]?.spcfdesc || "");
            ui.select("ddlWarranty", state.data.warrantyOptions, header.wrcode || header.rwrcode, "Select warranty status");
            ui.select("ddlProblem", state.data.problemOptions, "", "Select problem");
            ui.select("ddlSparePart", state.data.spareOptions, "", "Select spare part");
        }
    };

    function formatNumber(value) {
        return EnterpriseUI.parseNumber(value).toLocaleString(undefined, {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        });
    }

    function formatInput(value) {
        return EnterpriseUI.parseNumber(value).toFixed(2);
    }

    function isApproved(value) {
        return String(value).toLowerCase() === "true" || String(value).toLowerCase() === "ok";
    }

    function syncRows() {
        document.querySelectorAll(".diagnosis-remark").forEach(function (element) {
            state.problems[Number(element.dataset.index)].remarks = element.value.trim();
        });
        document.querySelectorAll(".part-qty").forEach(function (element) {
            const part = state.parts[Number(element.dataset.index)];
            part.qty = EnterpriseUI.parseNumber(element.value);
            part.amount = part.qty * EnterpriseUI.parseNumber(part.sirval);
        });
        document.querySelectorAll(".part-approved").forEach(function (element) {
            state.parts[Number(element.dataset.index)].apstatus = element.checked ? "True" : "False";
        });
        document.querySelectorAll(".part-issue").forEach(function (element) {
            state.parts[Number(element.dataset.index)].execqty = EnterpriseUI.parseNumber(element.value);
        });
    }

    function saveRequest() {
        syncRows();
        return {
            type: state.type,
            centerId: state.centerId,
            receiveNo: state.receiveNo,
            imei: ui.value("txtImei"),
            replacementImei: ui.value("txtReplacementImei"),
            diagnosisDate: ui.value("txtDiagnosisDate"),
            receivedDate: ui.value("txtReceiveDate"),
            productCode: state.data.header.procode || state.data.product.rescod,
            warrantyCode: ui.value("ddlWarranty"),
            remarks: ui.value("txtPartsRemarks"),
            problems: state.problems,
            spareParts: state.parts
        };
    }

    function workflowRequest() {
        syncRows();
        return {
            type: state.type,
            centerId: state.centerId,
            receiveNo: state.receiveNo,
            date: ui.value("txtDiagnosisDate"),
            diagnosisDate: state.data.header.digndate || ui.value("txtDiagnosisDate"),
            imei: ui.value("txtImei"),
            replacementImei: ui.value("txtReplacementImei"),
            spareParts: state.parts
        };
    }

    async function submit(action, request, message) {
        if (state.saving) {
            return;
        }
        state.saving = true;
        Helpers.setPending("#btnFooterSave", true);
        try {
            await service.post(action, request);
            ui.notify("Updated", message, "success");
            await loadData();
        } catch (error) {
            ui.notify("Not updated", error?.objError || "The operation could not be completed.", "warning");
        } finally {
            state.saving = false;
            Helpers.setPending("#btnFooterSave", false);
        }
    }

    function addProblem() {
        const select = document.getElementById("ddlProblem");
        if (!select.value) {
            return;
        }
        if (state.problems.some(function (item) { return item.gcod === select.value; })) {
            ui.notify("Duplicate", "This problem is already added.", "warning");
            return;
        }
        state.problems.push({
            grp: "A",
            gcod: select.value,
            gdesc: select.options[select.selectedIndex].text,
            remarks: ""
        });
        ui.renderProblems();
    }

    function addPart() {
        const code = ui.value("ddlSparePart");
        if (!code) {
            return;
        }
        if (state.parts.some(function (item) { return item.rsircode === code; })) {
            ui.notify("Duplicate", "This spare part is already added.", "warning");
            return;
        }
        const option = state.data.spareOptions.find(function (item) { return item.code === code; });
        state.parts.push({
            rsircode: code,
            sirdesc: option?.description || "",
            sirunit: option?.unit || "",
            qty: 1,
            sirval: state.data.companyCode === "8421" || ["0302", "0304"].some(function (prefix) {
                return ui.value("ddlWarranty").startsWith(prefix);
            }) ? EnterpriseUI.parseNumber(option?.value) : 0,
            amount: EnterpriseUI.parseNumber(option?.value),
            apstatus: "False",
            execqty: 0,
            spcfcod: option?.specificationCode || "",
            spcfdesc: option?.specification || "",
            stkqty: EnterpriseUI.parseNumber(option?.stockQuantity)
        });
        ui.renderParts();
    }

    async function refreshParts() {
        const date = ui.value("txtDiagnosisDate");
        if (!date) {
            return;
        }
        try {
            state.data.spareOptions = await service.getSpareOptions(date);
            ui.select("ddlSparePart", state.data.spareOptions, "", "Select spare part");
        } catch (error) {
            ui.notify("Error", error?.objError || "Spare-part options could not be refreshed.", "warning");
        }
    }

    function bindEvents() {
        $("#btnAddProblem").on("click", addProblem);
        $("#btnAddPart").on("click", addPart);
        $("#txtDiagnosisDate").on("changeDate", refreshParts);
        $(document).on("click", ".remove-problem", function () {
            state.problems.splice(Number(this.dataset.index), 1);
            ui.renderProblems();
        });
        $(document).on("click", ".remove-part", function () {
            state.parts.splice(Number(this.dataset.index), 1);
            ui.renderParts();
        });
        $(document).on("change", ".part-qty, .part-approved, .part-issue", function () {
            syncRows();
            ui.renderParts();
        });
        $("#btnFooterSave").on("click", function () {
            if (state.type.toLowerCase() === "qc") {
                submit("QcPass", workflowRequest(), "QC completed successfully.");
                return;
            }
            submit("Save", saveRequest(), "Diagnosis information was saved.");
        });
        $("#btnServiceDone").on("click", function () {
            submit("ServiceDone", workflowRequest(), "Service work was completed.");
        });
        $("#btnQcReject").on("click", function () {
            submit("QcReject", workflowRequest(), "Product was referred back.");
        });
        $("#btnFooterRefresh").on("click", loadData);
        $("#btnFooterCancel").on("click", function () {
            window.location.href = "/F_20_Service/RptServInterface?Type=All";
        });
    }

    async function loadData() {
        if (!state.centerId || !state.receiveNo) {
            ui.notify("Required", "Service center and receive number are required.", "warning");
            return;
        }
        try {
            state.data = await service.load();
            state.problems = state.data.diagnosisProblems || [];
            state.parts = state.data.spareParts || [];
            ui.renderHeader();
            ui.renderReference();
            ui.renderProblems();
            ui.renderParts();
        } catch (error) {
            ui.notify("Error", error?.objError || "Diagnosis information could not be loaded.", "warning");
        }
    }

    function init() {
        ShowFooterWithButtons();
        HideRecalculate();
        bindEvents();
        loadData();
    }

    return {
        init: init,
        approve() {
            submit("Approve", workflowRequest(), "Spare parts were approved.");
        }
    };
})();

window.onApproveClick = function () {
    DiagProductEntryPage.approve();
};

document.addEventListener("DOMContentLoaded", DiagProductEntryPage.init);
