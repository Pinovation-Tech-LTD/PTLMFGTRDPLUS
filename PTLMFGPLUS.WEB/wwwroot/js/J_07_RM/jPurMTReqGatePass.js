/*Area/Controller/*/
const commonPath = "/F_07_RM/PurMTReqGatePass/";
const urlParams = new URLSearchParams(window.location.search);
const qparam = urlParams.get("type");
const mtreqno = urlParams.get("mtrref");
var MTReqGatePassPage = (function () {
    var state =
    {
        GPassDataState: [],
        storeIssueData: []

    };
    var service = {

        async loadGPassNo(todate) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPassNo`),
                { todate: todate }));
        },
        async loadGPassData(curdate1, searchtext) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPassData`),
                { curdate1: curdate1, searchtext: searchtext }))
        },
        async lbtnUpdatePurApproved(qparam, mGetpNo,mmGetpDat, getpref, mtrNar, issueData) {
            return Helpers.withLoader(() => FetchHelpers.postForm(FetchHelpers.urlconfig(`${commonPath}SaveButtonClick`),
                { qparam: qparam, mGetpNo: mGetpNo, mmGetpDat: mmGetpDat, getpref: getpref, mtrNar: mtrNar, issueData: JSON.stringify(issueData) }))
        }
    };
    var ui = {
        setValue(id, value) {
            var el = document.getElementById(id);
            if (el) el.value = value;
        },
        getValue(id) {
            var el = document.getElementById(id);
            return el ? el.value : null;
        },
        renderTable(items) {
            const tbody = document.getElementById('selectedtabledata');
            tbody.innerHTML = '';

            items.forEach((item, index) => {
                tbody.innerHTML += `
        <tr>
            <td class="fs-6">${index + 1}</td>
            <td class="fs-6">${item.tfpactdesc}</td>
            <td class="fs-6">${item.ttpactdesc}</td> 
            <td class="fs-6">${item.rsirdesc}</td> 
            <td class="fs-6">${item.rsirunit}</td> 
            <td class="fs-6">${item.mtreqno1}</td> 
            <td class="fs-6">${item.mtrref}</td> 
            <td class="fs-6">${item.mtrfqty}</td> 
            <td class="fs-6">${item.balqty}</td> 
            <td class="fs-6">${item.stockqty}</td> 
            <td>
                <input type="text"
                    class="form-control form-control-sm qty-input w-100"
                    data-index="${index}"
                    value="${item.approveqty || item.mtrfqty}"
                    style="max-width: 100px;">
            </td>
        </tr>
        `;
            });
            document.querySelectorAll('.qty-input').forEach(input => {
                input.addEventListener('input', (e) => {
                    const index = e.target.dataset.index;
                    const value = e.target.value;

                    state.storeIssueData[index].getpqty = Number(value);

                });
            });
        },
        renderResList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    mtreqno: "",
                    mtreqno1: "None"
                }];
            } else {
                data = items;
            }
            Helpers.loadSelect2WithData({
                selector: "txtRequisitionList",
                data: data,
                valueField: "valuefiled",
                textField: "textfield",
                selectedValue: mtreqno
            })
        },
        renderResourceList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    mtreqno: "",
                    mtreqno1: "None"
                }];
            } else {
                data = items;
            }
            Helpers.loadSelect2WithData({
                selector: "txtResourcelist",
                data: data,
                valueField: "rsircode",
                textField: "rsirdesc",
                selectedValue: data[0].rsircode
            })
        },
        renderSpecificationList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    mtreqno: "",
                    mtreqno1: "None"
                }];
            } else {
                data = items;
            }
            Helpers.loadSelect2WithData({
                selector: "txtSpecification",
                data: data,
                valueField: "valuefiled",
                textField: "textfield",
                selectedValue: data[0].valuefiled
            })
        },
        renderCurTransNo(items) {
            if (!items || items.length === 0) return;
            this.setValue("txtCurGetPasNo", items[0].maxno1);
        },

    };
    function init() {
        bindEvents();
        loadInitialData();
        ShowFooterWithButtons();
        HideRefresh();
        HideRecalculate();
    }
    function bindEvents() {
        $("#btnOk").on("click", function () {

        });
        $("#btnFooterSave").on("click", async function () {
            const result = UpdatePurApproveClick();
        });
        $("#txtRequisitionList").on("change", async function () {
            const reqno = ui.getValue("txtRequisitionList");
            const filterResource = state.GPassDataState.item2.filter(x => reqno.includes(x.mtreqno));
            ui.renderResourceList(filterResource);
        });
        $("#txtResourcelist").on("change", async function () {
            const reqno = ui.getValue("txtRequisitionList");
            const rsircode = ui.getValue("txtResourcelist");
            const filterResource = state.GPassDataState.item1.filter(x => x.mtreqno === reqno && x.rsircode === rsircode);
            ui.renderSpecificationList(filterResource);
        });


    }
    async function GetGpassIdNew() {
        const date = ui.getValue("txtdate");
        const todate = formatDate(date);
        const moduleItems = await service.loadGPassNo(todate);
        return moduleItems;
    }
    async function GetGPassIdByDate() {
        const NewId = await GetGpassIdNew();
        ui.renderCurTransNo(NewId);
    }
    async function GetGPassData() {
        const date = ui.getValue("txtdate");
        const curdate1 = formatDate(date);
        const GPassData = await service.loadGPassData(curdate1, "%");
        state.GPassDataState = GPassData;
        ui.renderResList(state.GPassDataState.item3);
    }
    async function ShowTableData() {
        const TableData = state.GPassDataState.item1;

        let mReqNoData = ui.getValue("txtSpecification");
        const mreqNo = mReqNoData.substring(0, 14);
        const filterMReqno = TableData.filter(x => x.mtreqno == mreqNo);
        state.storeIssueData = filterMReqno;
        for (let i = 0; i < state.storeIssueData.length; i++)
        {
            state.storeIssueData[i].getpqty = state.storeIssueData[i].balqty;
            state.storeIssueData[i].rate = state.storeIssueData[i].mtrfrat;
            state.storeIssueData[i].getpamt = state.storeIssueData[i].mtrfamt;
        }
        ui.renderTable(state.storeIssueData);
    }
    
    async function UpdatePurApproveClick() {
        const date = ui.getValue("txtdate");
        const mmGetpDat = formatDate(date);
        const result = await GetGpassIdNew();
        const mGetpNo = result[0].maxno;
        const getpref = ui.getValue("txtGatemPassNo");
        const mtrNar = ui.getValue("txtReqNarr");
        const issueData = state.storeIssueData;
        const response = await service.lbtnUpdatePurApproved(qparam, mGetpNo, mmGetpDat, getpref, mtrNar, issueData);
        handleResponse(response);
    }
    function handleResponse(res) {
        if (!res) {
            alert("No response from server");
            return;
        }

        if (res.success) {
            alert(res.message || "Saved successfully");
        } else {
            alert(res.message || "Something went wrong");
        }
    }
    async function loadInitialData() {
        await GetGPassIdByDate();
        await GetGPassData();
        await ShowTableData();
        $("#txtCurGetPasNo").prop("disabled", true);
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', MTReqGatePassPage.init);