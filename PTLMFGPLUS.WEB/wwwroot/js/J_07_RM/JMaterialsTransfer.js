/*Area/Controller/*/
const commonPath = "/F_07_RM/MaterialsTransfer/";
const UrlParams = new URLSearchParams(window.location.search);
const genno = UrlParams.get("getpno");
var MaterialsTransferPage = (function () {
    var state = {       
        ProjectListState: [],
        tblMatTrnsState: [],
        tblGatePInfoState: [],
        tblGatePNoState: [],
        tblProjectResourceListState: [],
        tblSpcfState: [],
    };

    var service = {
        async loaddropdown() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetModules`)));
        },       
        async loadLastTransId(date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetLastMTRNumber`), { date: date }));
        },
        async loadProjectFromList() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProjectFromList`)));
        },
        async loadProjectResourceList(projectCode, curDate, findResDesc) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProjectResourceList`), { projectCode: projectCode, curDate: curDate, findResDesc: findResDesc }));
        },
        async loadMtReqGPassList(CurDate1, SearchText) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetGatePassNo`), { CurDate1: CurDate1, SearchText: SearchText}));
        },
        async loadPrevTransferInfo(mTRNNo, CurDate1) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPrevTransferInfo`), { mTRNNo: mTRNNo, CurDate1: CurDate1 }));
        },
        
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
        renderCurTransNo(items) {
            if (!items || items.length === 0) return;
            this.setValue("txtCurTransNo", items[0].maxtrnno1);
        },
        renderProjectFromList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    actcode: "",
                    actdesc1: "None"
                }];
            } else {
                data = items;
            }
            Helpers.loadSelect2WithData({
                selector: "txtProjectFromList",
                data: data,
                valueField: "actcode",
                textField: "actdesc1",
                selectedValue: data[0].actcode
            })
        },
        renderProjectToList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    actcode: "",
                    actdesc1: "None"
                }];
            } else {
                data = items;
            }
            Helpers.loadSelect2WithData({
                selector: "txtProjectToList",
                data: data,
                valueField: "actcode",
                textField: "actdesc1",
                selectedValue: data[0].actcode
            })
        },
        renderProjectResourceList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    rsircode: "",
                    resdesc: "None"
                }];
            } else {
                data = items;
            }
            Helpers.loadSelect2WithData({
                selector: "txtProjectResourceList",
                data: data,
                valueField: "rsircode",
                textField: "resdesc",
                selectedValue: data[0].rsircode
            })
        },
        renderProjectSpaceList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    spcfcod: "",
                    spcfdesc: "None"
                }];
            } else {
                data = items;
            }
            Helpers.loadSelect2WithData({
                selector: "txtProjectSpecification",
                data: data,
                valueField: "spcfcod",
                textField: "spcfdesc",
                selectedValue: data[0].spcfcod
            })
        },
        renderDropdown(items) {
            Helpers.loadSelect2WithData({
                selector: "ddlModule",
                data: items,
                valueField: "id",
                textField: "name",
                selectedValue: 0
            })
        },
        renderTable(items) {
            const tbody = document.getElementById('selectedtabledata');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td>${index + 1}</td>                
                <td>${item.mtrref}</td>
                <td>${item.getpref}</td>
                <td>${item.resdesc}</td>                
                <td>${item.sirunit}</td>
                <td>${item.mtrfqty}</td>
                <td>${item.qty}</td>
                <td>${item.rate}</td>
                <td>${item.amt}</td>
                
                <td></td>
            </tr>
        `;
            });
        }
    };
    function init() {
        bindEvents();
        loadInitialData();
        ShowFooterWithButtons();
        HideRefresh();
    }
    function bindEvents() {        
        $("#btnOk").on("click", function () {
           
        });

        $("#btnFooterSave").on("click", function () {
            
        });
        $("#txtProjectFromList").on("change", function () {
            const fromlistactcode = ui.getValue("txtProjectFromList");
            const filterProjectlist = state.ProjectListState.filter(x => x.actcode !== fromlistactcode);
            ui.renderProjectToList(filterProjectlist);
        });
        $("#txtProjectResourceList").on("change", function () {
             SpecificationList();
        });
    }
    async function LastTransId() {
        const date = ui.getValue("txtdate");
        const formatdate = formatDate(date);
        const TransId = await service.loadLastTransId(formatdate);
        ui.renderCurTransNo(TransId);
    }
    async function ProjectFromList() {
        const ProjectList = await service.loadProjectFromList();
        state.ProjectListState = ProjectList;
        ui.renderProjectFromList(state.ProjectListState);
        
    }
    async function ResourceList() {            
        const projectCode = ui.getValue("txtProjectFromList");
        const curDate1 = ui.getValue("txtdate");
        const curDate = formatDate(curDate1);
        const ResDesc = ui.getValue("txtSearchRes");
        const findResDesc = (ResDesc ?? "") + "%";
        var resList = await service.loadProjectResourceList(projectCode, curDate, findResDesc);
        state.tblProjectResourceListState = resList.tblPorjectResList;
        state.tblSpcfState = resList.tblSpcf
        ui.renderProjectResourceList(state.tblProjectResourceListState);        
    }
    async function SpecificationList() {
        const mResCode = ui.getValue("txtProjectResourceList");
        const filterSpcf = state.tblSpcfState.filter(x => x.mspcfcod === mResCode.substring(0, 9) || x.spcfcod === '000000000000');
        ui.renderProjectSpaceList(filterSpcf);
    }
    async function GatePassList() {
        const date = ui.getValue("txtdate");
        const curdate1 = formatDate(date);
        const SearchText = "%%";
        var passList = await service.loadMtReqGPassList(curdate1, SearchText);
        if (passList.length === 0) {
            return;
        }
        state.tblGatePInfoState = passList.tblGatePInfo;
        state.tblGatePNoState = passList.tblGatePNo;
    }
    async function GetMatTransfer() {
        const date = ui.getValue("txtdate");        
        const curdate1 = formatDate(date);
        const mTRNNo = "NEWTRNS";
        // if need it 
        //if (ui.getValue("#txtPrevISSList").length > 0) {
            
        //    //mTRNNo = this.ddlPrevISSList.SelectedValue.ToString();
        //}

        var PrevTransfer = await service.loadPrevTransferInfo(mTRNNo, curdate1);
        if (PrevTransfer.length === 0) {
            return;
        }
        state.tblMatTrnsState = PrevTransfer.tblMatTrns;

        if (genno.length > 0 && genno.substring(0, 3) === "GPN")  // add this when need this.ddlGatePass.Items.Count > 0 ||
        {
            var gatepno = "";
            if (genno.length > 0) {
                gatepno = genno;
            }
            else {
                // gatepno = this.ddlGatePass.SelectedValue.ToString(); need to add 
            }
            const dr = state.tblGatePNoState.filter(x => x.getpno === gatepno);
            if (dr.length > 0) {                
                $("#txtProjectFromList").prop("disabled", true);
                $("#txtProjectToList").prop("disabled", true);
                $("#txtProjectFromList")
                    .val(dr[0].tfpactcode)
                    .trigger("change");
                $("#txtProjectToList")
                    .val(dr[0].ttpactcode)
                    .trigger("change");
            }
        }
        if (mTRNNo === "NEWTRNS") {
            var ds1 = await service.loadLastTransId(curdate1);
            ui.renderCurTransNo(ds1.maxtrnno1);
            return;
        }
        

    }
    async function lbtnOk_Click() {

        const btn = document.getElementById("btnOk");
        const div = document.getElementById("btnOkClick");

        if (btn.textContent.trim() === "OK") {

            btn.innerHTML = `
            <i class="bi bi-check-lg"></i> New
        `;
            div.style.display = "flex";
            await GetMatTransfer();
            await ResourceList();           
        }
        else {

            btn.innerHTML = `
            <i class="bi bi-check-lg"></i> OK
        `;
        }
    }
    async function lnkselectgpAll_Click() {
        if (genno.length > 0) {
            gatepno = genno;
        }
        else {
            //gatepno = this.ddlResSpcfgp.SelectedValue.ToString().Substring(0, 14); -- if need uncommand it           
        }
        const dt = state.tblMatTrnsState;
        const dt1 = state.tblGatePInfoState;
        const dr1 = dt.filter(x => x.getpno === gatepno);
        const dtg = dt1.filter(x => x.getpno === gatepno);
        if (dr1.length === 0) {

            for (const dr2 of dtg)
            {
                dt.push({
                    mtreqno: dr2.mtreqno,
                    mtrref: dr2.mtrref,
                    getpno: dr2.getpno,
                    getpref: dr2.getpref,
                    rsircode: dr2.rsircode,
                    spcfcod: dr2.spcfcod,
                    resdesc: dr2.rsirdesc,
                    spcfdesc: dr2.spcfdesc,
                    sirunit: dr2.rsirunit,
                    mtrfqty: dr2.mtrfqty,
                    qty: dr2.getpqty,
                    rate: dr2.rate,
                    amt: dr2.getpamt,
                    reqno: ""
                });
            };
        }
        state.tblMatTrnsState = dt;
       await Data_Bind();
    }
    async function Data_Bind() {
        ui.renderTable(state.tblMatTrnsState);
    }
    async function loadInitialData() {
        await LastTransId();
        await ProjectFromList();
        await GatePassList();    
        $("#txtCurTransNo").prop("disabled", true);
        if (genno.length > 0) {
            if (genno.substring(0, 3) === "GPN") {
                await GatePassList();
                await lbtnOk_Click();
                await lnkselectgpAll_Click();
            }
            else {

            }
        };

       
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', MaterialsTransferPage.init);