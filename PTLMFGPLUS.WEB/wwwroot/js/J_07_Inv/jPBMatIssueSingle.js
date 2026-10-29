const pbmiPath = "/F_07_Inv/PBMatIssueSingle/";
const pbmiQuery = new URLSearchParams(window.location.search);

var PBMatIssueSinglePage = (function () {
    "use strict";
    const type = pbmiQuery.get("Type") || pbmiQuery.get("type") || "Entry";
    const dprNo = pbmiQuery.get("actcode") || "";
    const requestedDate = pbmiQuery.get("genno") || "";
    const state = { stores: [], previous: [], available: [], requisitions: [], reqItems: [], rows: [], header: null, issueId: "", opened: false };
    const months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    const api = {
        get(action, data) { return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${pbmiPath}${action}`), data)); },
        post(action, data) { return Helpers.withLoader(() => FetchHelpers.postJson(FetchHelpers.urlconfig(`${pbmiPath}${action}`), data)); }
    };
    const el = id => document.getElementById(id);
    const value = id => el(id)?.value || "";
    const num = value => Number.parseFloat(value) || 0;
    const esc = value => $("<div>").text(value ?? "").html();
    const dateText = date => `${String(date.getDate()).padStart(2, "0")}-${months[date.getMonth()]}-${date.getFullYear()}`;
    const parseDate = text => { const m = String(text || "").match(/^(\d{1,2})-([A-Za-z]{3})-(\d{4})$/); return m ? new Date(+m[3], months.findIndex(x => x.toLowerCase() === m[2].toLowerCase()), +m[1]) : new Date(text); };
    const format = (value, digits = 4) => num(value).toLocaleString("en-US", { minimumFractionDigits: digits, maximumFractionDigits: digits });

    function loadSelect(id, items, valueField, textField, placeholder, selected) {
        Helpers.loadSelect2WithData({ selector: id, data: items, valueField, textField, placeholder, selectedValue: selected });
    }
    function showError(error, fallback) { Notifications.Toastr.error(error?.objError || error?.message || fallback); }
    function materialKey(row) { return `${row.preqno}|${row.rsircode}|${row.spcfcod}`; }

    async function initialize() {
        ShowFooterWithButtons();
        HideRefresh();
        HideCancel();

        const lockDate = el("pbmiPage").dataset.lockDate === "true";
        const initial = lockDate || !requestedDate ? new Date() : parseDate(requestedDate);
        $("#issueDate").datepicker({ format: { toDisplay: dateText, toValue: parseDate }, autoclose: true, todayHighlight: true, orientation: "bottom" }).datepicker("setDate", initial);
        el("issueDate").disabled = lockDate;
        state.stores = await api.get("GetStores", { type });
        loadSelect("storeCode", state.stores, "actcode", "actdesc", "Select store", state.stores[0]?.actcode);
        bindEvents();
        if (value("storeCode")) await openIssue();
    }

    function bindEvents() {
        $("#btnOpen").on("click", openIssue); $("#btnNew").on("click", reset);
        $("#btnPrevious").on("click", loadPrevious);
        $("#previousIssue").on("change", () => { if (value("previousIssue")) openIssue(); });
        $("#requisitionNo").on("change", changeRequisition);
        $("#btnSelect").on("click", selectOne); $("#btnSelectAll").on("click", selectAll);
        $("#btnToggleItems").on("click", () => { $("#reqItemsPanel").toggleClass("d-none"); el("btnToggleItems").textContent = $("#reqItemsPanel").hasClass("d-none") ? "Item Expand" : "Item Collapse"; });
        $("#btnReload").on("click", reloadMaterials);
        $("#btnFooterSave").on("click", save);
        $("#issueTable").on("input", "input[data-field]", updateIssueInput);
        $("#reqItemsTable").on("input", "input[data-index]", updateReqInput);
    }
    async function loadPrevious() {
        try {
            state.previous = await api.get("GetPreviousIssues", { date: value("issueDate") }) || [];
            loadSelect("previousIssue", state.previous, "misuno", "misuno1", "Select previous issue", null);
            el("previousPanel").classList.remove("d-none");
        } catch (error) { showError(error, "Previous issues could not be loaded."); }
    }

    async function openIssue() {
        if (!value("storeCode")) return Notifications.Toastr.error("Select a store.");
        try {
            const selectedIssue = value("previousIssue");
            const result = await api.get("Open", { type, date: value("issueDate"), storeCode: value("storeCode"), issueNo: selectedIssue, dprNo });
            state.header = result.header || null; state.issueId = result.header?.misuno || selectedIssue || "";
            state.rows = result.issueRows || []; state.available = result.availableMaterials || []; state.requisitions = result.requisitions || []; state.reqItems = result.requisitionItems || [];
            el("issueNo").value = result.issueNo || result.header?.misuno1 || "";
            if (state.header) { el("referenceNo").value = state.header.misuref || ""; el("remarks").value = state.header.rmrks || ""; if (state.header.misudate) $("#issueDate").datepicker("setDate", parseDate(state.header.misudate)); if (state.header.actcode) $("#storeCode").val(state.header.actcode).trigger("change.select2"); }
            loadSelect("requisitionNo", state.requisitions, "preqno", "preqno1", "Select requisition", state.requisitions[0]?.preqno);
            updateMaterialSelect(); renderAll(); state.opened = true;
            el("selectionPanel").classList.toggle("d-none", Boolean(selectedIssue) || type.toLowerCase() === "approve");
            el("btnFooterSave").querySelector("span").textContent = type.toLowerCase() === "approve" ? "Approve" : "Save";
        } catch (error) { showError(error, "Material issue could not be opened."); }
    }

    async function changeRequisition() {
        const req = value("requisitionNo"); updateMaterialSelect();
        if (!req) { state.reqItems = []; return renderReqItems(); }
        try { state.reqItems = await api.get("GetRequisitionItems", { requisitionNo: req, storeCode: value("storeCode") }) || []; renderReqItems(); }
        catch (error) { showError(error, "Requisition items could not be loaded."); }
    }
    function updateMaterialSelect() { const rows = state.available.filter(x => x.preqno === value("requisitionNo")); loadSelect("materialCode", rows, "rsircode1", "rsirdesc1", "Select material", rows[0]?.rsircode1); }
    function addRows(rows) { const keys = new Set(state.rows.map(materialKey)); rows.forEach(row => { if (!keys.has(materialKey(row))) { state.rows.push({ ...row }); keys.add(materialKey(row)); } }); renderIssues(); }
    function selectOne() { const selected = value("materialCode"); const row = state.available.find(x => x.preqno === value("requisitionNo") && x.rsircode1 === selected); if (row) addRows([row]); }
    function selectAll() { addRows(state.available.filter(x => x.preqno === value("requisitionNo"))); }

    function renderAll() { renderIssues(); renderReqItems(); }
    function renderIssues() {
        let previousReq = "", previousBatch = "", previousProcess = "";
        el("issueTable").querySelector("tbody").innerHTML = state.rows.map((r, i) => {
            const req = r.preqno === previousReq ? "" : r.preqno1; const batch = r.bactcode === previousBatch ? "" : r.bactdesc; const process = r.procode === previousProcess ? "" : r.prodesc;
            previousReq = r.preqno; previousBatch = r.bactcode; previousProcess = r.procode;
            return `<tr>
            <td>${i + 1}</td>
            <td>${esc(req)}</td>
            <td class="desc-cell">${esc(batch)}</td>
            <td class="desc-cell">${esc(process)}</td>
            <td class="desc-cell">${esc(r.rsirdesc)}</td>
            <td class="desc-cell">${esc(r.spcfdesc)}</td>
            <td>${esc(r.rsirunit)}</td><td class="text-end">${format(r.stockqty)}</td>
            <td class="text-end">${format(r.reqqty)}</td><td class="text-end">${format(r.balqty)}</td>
            <td><input class="form-control form-control-sm number-input" type="number" min="0" step="any" data-index="${i}" data-field="isuqty" value="${num(r.isuqty)}"></td>
            <td><input class="form-control form-control-sm number-input" type="number" min="0" step="any" data-index="${i}" data-field="isurate" value="${num(r.isurate)}"></td>
            <td class="text-end">${format(num(r.stockqty) - num(r.isuqty))}</td>
            </tr>`;
        }).join("");
    }
    function renderReqItems() {
        el("reqItemsTable").querySelector("tbody").innerHTML = state.reqItems.map((r, i) => `<tr><td>${i + 1}</td>
        <td class="desc-cell">${esc(r.rsirdesc)}</td>
        <td>${esc(r.rsirunit)}</td><td class="text-end">${format(r.rsqty)}</td>
        <td class="text-end">${format(r.issuecomp)}</td>
        <td><input class="form-control form-control-sm number-input" type="number" min="0" step="any" data-index="${i}" value="${num(r.balisu)}"></td>
        </tr>`
        ).join("");
    }
    function updateIssueInput(event) {
        const input = event.target; state.rows[+input.dataset.index][input.dataset.field] = num(input.value); if (input.dataset.field === "isuqty") renderIssues();
    }
    function updateReqInput(event) {
        state.reqItems[+event.target.dataset.index].balisu = num(event.target.value);
    }

    async function reloadMaterials() {
        if (state.reqItems.some(x => num(x.issuecomp) + num(x.balisu) > num(x.rsqty)))
            return Notifications.Toastr.error("Issue balance cannot exceed requisition quantity.");
        try {
            state.rows = await api.post("ReloadMaterials",
                { storeCode: value("storeCode"), date: value("issueDate"), dprNo: value("requisitionNo") || dprNo, requisitionItems: state.reqItems }) || []; renderIssues(); Notifications.Toastr.success("Material list reloaded.");
        }
        catch (error) {
            showError(error, "Material list could not be reloaded.");
        }
    }

    function encodedIssueNo() {
        if (state.issueId) return state.issueId;
        const shown = value("issueNo"); const date = parseDate(value("issueDate"));
        return shown.length >= 11 ? `${shown.substring(0, 3)}${date.getFullYear()}${shown.substring(3, 5)}${shown.substring(6)}` : shown;
    }
    async function save() {
        if (!state.opened) return Notifications.Toastr.error("Open the issue first.");
        try {
            if (type.toLowerCase() === "approve")
                await api.post("Approve", { issueNo: encodedIssueNo(), storeCode: value("storeCode") });
            else
                await api.post("Save",
                    { type, date: value("issueDate"), issueNo: encodedIssueNo(), storeCode: value("storeCode"), referenceNo: value("referenceNo"), remarks: value("remarks"), issueRows: state.rows, requisitionItems: state.reqItems, originalHeader: state.header }
                );
            Notifications.Toastr.success(type.toLowerCase() === "approve" ? "Material issue approved." : "Material issue saved successfully.");
            setTimeout(() => {
                window.location.reload();
            }, 1500);
        } catch (error) {
            showError(error, "Material issue could not be saved.");
        }
    }
    function reset() {
        state.previous = []; state.available = []; state.requisitions = []; state.reqItems = []; state.rows = []; state.header = null; state.issueId = ""; state.opened = false; el("referenceNo").value = "";
        el("issueNo").value = ""; el("remarks").value = "";
        el("previousPanel").classList.add("d-none");
        el("selectionPanel").classList.add("d-none");
        renderAll();
    }
    return {
        initialize
    };
})();

$(function () {
    PBMatIssueSinglePage.initialize().catch(error => Notifications.Toastr.error(error?.objError || "Material issue page could not be initialized."));
});
