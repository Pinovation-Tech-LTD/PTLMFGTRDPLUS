/*Area/Controller/*/
const commonPath = "/F_07_RM/PurMTReq/";
const urlParams = new URLSearchParams(window.location.search);
const qparam = urlParams.get("type");
const mtreqnoApproved = urlParams.get("mtrref");
var PurMtReqPage = (function () {
    var state =
    {
        item1: [],
        item2: [],
        selectedItems: [],
        FromToList: []
    };
    var service = {
        async loaddropdown() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetModules`)));
        },
        async loadLastMTRNumber(date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetLastMTRNumber`), { date: date }));
        },
        async loadPreviousOrder(date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPreviousOrderList`), { date: date }));
        },
        async loadApprovedData(mtrno, date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetMatTransferInfo`), { mtrno: mtrno, date: date }));
        },
        async loadProjectFromList() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProjectFromList`), {}));
        },
        async loadProjectResourceList(projectcode, curdate, findResDesc, stockCheck) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProjectResourceList`), { projectcode: projectcode, curdate: curdate, findResDesc: findResDesc, stockCheck: stockCheck }));
        },
        async footerSaveButton(previousOrderDataid,mtrref, mtreqdat, seletedFrom, selectedTo, mtrnar, selecteditem) {

            return Helpers.withLoader(() => FetchHelpers.postForm(FetchHelpers.urlconfig(`${commonPath}SaveButtonClick`),
                { previousOrderDataid: previousOrderDataid, mtrref: mtrref, mtreqdat: mtreqdat, seletedFrom: seletedFrom, selectedTo: selectedTo, mtrnar: mtrnar, selectedItem: JSON.stringify(selecteditem) }));
        },
        async approveButton(mtreqno) {
            return Helpers.withLoader(() => FetchHelpers.postForm(FetchHelpers.urlconfig(`${commonPath}ApprovedButtonClick`), { mtreqno: mtreqno }));
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

        renderPreviousMatOrder(items) {
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
                selector: "txtPreviousOrder",
                data: data,
                valueField: "mtreqno",
                textField: "mtreqno1",
                selectedValue: data[0].actcode
            })
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
        renderProjectSpecificationList(items) {
            let data = [];

            if (!items || items.length === 0) {
                data = [{
                    spcfcod: "000000000000",
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
        renderTable(items) {
            const tbody = document.getElementById('selectedtabledata');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.resdesc}</td>
                <td class="fs-6">${item.sirunit}</td> 
                <td class="fs-6">${item.balqty}</td> 
               <td>
                <input type="number"
               class="form-control form-control-sm qty-input w-100"
               data-index="${index}"
               value="${item.qty || 0}"
               style="max-width: 100px;"
               min="0"
               step="0.01">
            </td>

            <td class="rate">${parseFloat(item.rate).toFixed(2)}</td>

            <td>
                <span class="amt">${parseFloat(item.amt || 0).toFixed(2)} </span>
            </td>                
                
            </tr>
        `;
            });
        },
        renderCurTransNo(items) {
            if (!items || items.length === 0) return;
            this.setValue("txtCurTransNo", items[0].maxtrnno1);
        },

    };
    function init() {
        if (qparam == 'approved') {
            HideSave();
            
        }
        bindEvents();
        loadInitialData();
        ShowFooterWithButtons();
        HideRefresh();
        HideRecalculate();
    }
    function okClick() {
        ToggleDiv();        
        $("#txtProjectToList").prop("disabled", true);
        $("#txtProjectFromList").prop("disabled", true);
        $("#previousOrder").hide();
        $(this).text("New");
        //$("#previousOrder").closest(".col-md-4").hide();   //full div hide previous order
        
    }
    function bindEvents() {
        $("#btnOk").on("click", function () {
            let btnText = $(this).text().trim();
            const previousOrderData = ui.getValue("txtPreviousOrder");

            if (previousOrderData !== "") {
                ApprovedDataLoad(previousOrderData);
                
            }

            if (btnText === "OK") {
                GetResource();
                okClick();
            }
            else if (btnText === "New") {
                location.reload();
            }
        });
        $("#lblPreviousOrder").on("click", function () {
            GetPreviousOrder();
        })
        $("#btnFooterSave").on("click", async function () {

            const mtrref = ui.getValue("txtRefNo");
            if (mtrref === null || mtrref === '') {
                Notifications.Sweet.warning("Warning!!", "MTRF not found!!");
                return;
            }
            Notifications.Sweet.confirmSave("Are you sure you want to save this record?", async function(){                
                const mtrnar = ui.getValue("txtReqNarr");
                const mtreqdat = ui.getValue("txtdate");
                const seletedFrom = ui.getValue("txtProjectFromList");
                const selectedTo = ui.getValue("txtProjectToList");
                const selecteditem = state.selectedItems;
                const previousOrderDataid = ui.getValue("txtPreviousOrder");
                let result = await service.footerSaveButton(previousOrderDataid,mtrref, mtreqdat, seletedFrom, selectedTo, mtrnar, selecteditem);
                if (result === null) {                   
                    Notifications.Sweet.error("Error!!","Update Failed!!!");
                    return;
                }
                 Notifications.Sweet.success("", "", function(response){
                     window.location.href = FetchHelpers.urlconfig(`/F_07_RM/RawMattInterface/InterfaceIndex`);
                 });
            })


        });

        $("#btnFooterApprove").on("click", async function () {
            const mtrref = ui.getValue("txtRefNo");
            if (!mtrref) { alert("MRF No is required!"); return; }           
            let result = await service.approveButton(mtreqnoApproved);
            if (result === null) {
                alert("Approval failed");
                return;
            }
            const isConfirm = confirm("Are you sure you want to save this record?");

            if (!isConfirm) {
                return; 
            }
            alert("Approved successfully!");
            window.location.href = `/F_07_RM/RawMattInterface/InterfaceIndex`;
        });
        $("#txtProjectResourceList").on("change", function () {
            const selectedResource = ui.getValue("txtProjectResourceList");
            const filteredSpecs = state.item2.filter(x =>
                x.mspcfcod === selectedResource
            );
            ui.renderProjectSpecificationList(filteredSpecs);
        });
        $("#txtProjectFromList").on("change", function () {
            const seletedFrom = ui.getValue("txtProjectFromList");

            let toListItems = state.FromToList;
            if (seletedFrom && seletedFrom !== "") {
                toListItems = state.FromToList.filter(x => x.actcode !== seletedFrom);
            }
            ui.renderProjectToList(toListItems);
        });
        $("#txtdate").on("change", function () {
            GetTransIdByDate();
        });

        $("#btnSelect").on("click", function () {

            AddToList();

            ui.renderTable(state.selectedItems);

        })
        $(document).on("input", ".qty-input", function () {

            let index = $(this).data("index");
            let qty = parseFloat($(this).val()) || 0;
            let item = state.selectedItems[index];
            item.qty = qty;
            item.amt = qty * item.rate;
            $(this).closest("tr").find(".amt").text(item.amt.toFixed(2));

        });

    }
    async function AddToList() {
        const selectedResource = ui.getValue("txtProjectResourceList");
        const selectedSpecification = ui.getValue("txtProjectSpecification");
        const filtered = state.item1.filter(x =>
            selectedResource.includes(x.rsircode) &&
            selectedSpecification.includes(x.spcfcod)
        );
        filtered.forEach((item) => {

            const exists = state.selectedItems.some(x =>
                x.rsircode === item.rsircode &&
                x.spcfcod === item.spcfcod
            );
            if (!exists) {
                state.selectedItems.push({
                    rsircode: item.rsircode,
                    spcfcod: item.spcfcod,
                    resdesc: item.resdesc,
                    sirunit: item.sirunit,
                    balqty: item.balqty,
                    rate: parseFloat(item.rate) || 0,
                    qty: 0,
                    amt: 0
                });
            }

        });
    }
    async function GetPreviousOrder() {
        const date = ui.getValue("txtdate");
        const formatteddate = (date);
        const PreviousOrderList = await service.loadPreviousOrder(formatteddate);
        ui.renderPreviousMatOrder(PreviousOrderList);
    }
    async function GetResource() {
        let projectId = ui.getValue("txtProjectFromList");
        let date = ui.getValue("txtdate");
        let stockCheck = document.getElementById("chkStock").checked ? "Y" : "N";
        const projectResourcelist = await service.loadProjectResourceList(projectId, date, "%", stockCheck);
        state.item1 = projectResourcelist.item1;
        state.item2 = projectResourcelist.item2;
        ui.renderProjectResourceList(state.item1);
    }
    async function ToggleDiv() {
        var div = document.getElementById("btnOkClick");
        if (div) {
            div.style.display = (div.style.display === "none") ? "flex" : "none";
        }
    }
    async function GetTransIdByDate() {
        const date = ui.getValue("txtdate");
        const formatteddate = (date);
        if (qparam === 'entry') {            
            const moduleItems = await service.loadLastMTRNumber(formatteddate);
            ui.renderCurTransNo(moduleItems);
        }      

    }
    async function ApprovedDataLoad(mtreqnoid) {
        const date = ui.getValue("txtdate");
        const formatteddate = (date);
        const approvedData = await service.loadApprovedData(mtreqnoid, formatteddate);
        if (!approvedData) return;
        ui.setValue("txtRefNo", approvedData.item2[0].mtrref);
        let formattedDate = new Date(approvedData.item2[0].mtrdat)
            .toISOString()
            .split("T")[0];

        ui.setValue("txtdate", formattedDate);
        ui.setValue("txtCurTransNo", approvedData.item2[0].trnno1);
        ui.setValue("txtReqNarr", approvedData.item2[0].mtrnar);
        state.FromToList = await service.loadProjectFromList();
        ui.renderProjectFromList(state.FromToList);
        ui.renderProjectToList(state.FromToList);
        $("#txtProjectFromList")
            .val(approvedData.item2[0].tfpactcode)
            .trigger("change");

        $("#txtProjectToList")
            .val(approvedData.item2[0].ttpactcode)
            .trigger("change");
        $("#txtProjectToList").prop("disabled", true);
        $("#txtProjectFromList").prop("disabled", true);
        $("#btnOk").prop("disabled", true);
        $("#txtCurTransNo").prop("disabled", true);
        $("#txtRefNo").prop("disabled", true);
        $("#txtdate").prop("disabled", true);
        $("#previousOrder").hide();
        GetResource();
        state.selectedItems = approvedData.item1 || [];
        ui.renderTable(state.selectedItems, true);
    }
    async function loadInitialData() {
        if (qparam === 'entry') {
            GetTransIdByDate();
            const projectFromListItems = await service.loadProjectFromList();
            state.FromToList = projectFromListItems;
            ui.renderProjectFromList(state.FromToList);
                 
            $("#btnFooterApprove").hide();

        } else if (qparam === 'approved') {
            ApprovedDataLoad(mtreqnoApproved);
            ToggleDiv();           
        }
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', PurMtReqPage.init);