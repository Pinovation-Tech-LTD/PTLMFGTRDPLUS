/*Area/Controller/*/
const commonPath = "/F_07_RM/RawMattInterface/";
const urlParams = new URLSearchParams(window.location.search);
var MatInterfacePage = (function () {
    var state =
    {
        interfacedata: [],
        selectedInterface: ""
    };
    var service = {        
        async loadInterfaceData(frmdate,todate) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetInterfaceInfo`), {frmdate, todate }));
        }
        
    };
    var ui = {
        setValue(id, value) {
            var el = document.getElementById(id);
            if (
                el.tagName === "INPUT" ||
                el.tagName === "TEXTAREA" ||
                el.tagName === "SELECT"
            ) {
                el.value = value;
            } else {
                el.innerText = value;
            }
        },
        getValue(id) {
            var el = document.getElementById(id);
            return el ? el.value : null;
        },        
        renderRequisitionTable(items) {
            const tbody = document.getElementById('selectedtabledata');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.mtreqno}</td>
                <td class="fs-6">${item.mtrdat}</td> 
                <td class="fs-6">${item.mtrref}</td> 
                <td class="fs-6">${item.tfpactdesc}</td> 
                <td class="fs-6">${item.ttpactdesc}</td> 
                <td class="fs-6">${item.mtrnar}</td>  
                <td class="fs-6">${item.tqty}</td> 
                <td class="fs-6">${item.tamt}</td>                
                <td class="fs-6">${item.postedusr}</td>              
            </tr>
        `;
            });
        }, 
        renderRequisitionApprovedTable(items) {
            const tbody = document.getElementById('selectedtabledata');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.mtreqno}</td>
                <td class="fs-6 getpno-col">${item.getpno}</td>
                <td class="fs-6">${item.mtrdat}</td> 
                <td class="fs-6">${item.mtrref}</td> 
                <td class="fs-6">${item.tfpactdesc}</td> 
                <td class="fs-6">${item.ttpactdesc}</td> 
                <td class="fs-6">${item.mtrnar}</td> 
                <td class="fs-6">${item.tqty}</td> 
                <td class="fs-6">${item.tamt}</td>                
                <td class="fs-6">${item.postedusr}</td>   
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-mtreqno="${item.mtreqno}" data-getpno="${item.getpno ?? ''}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>

                        <button class="btn btn-primary btn-edit" data-mtreqno="${item.mtreqno}"  title="Edit">
                            <i class="bi bi-pencil-square"></i>
                        </button>

                        <button class="btn btn-danger btn-delete" data-mtreqno="${item.mtreqno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        }        

    };
    function init() {       
        bindEvents();
        loadInitialData();     
       
    }    
    async function bindEvents() {
        $("#btnOk").on("click",async function () {
            let frmdate = ui.getValue("txtfrmdate");
            let todate = ui.getValue("txttodate");
            const formatfrmdate = (frmdate);
            const formattodate = (todate);

            const result = await service.loadInterfaceData(formatfrmdate, formattodate);
            if (result === null) {
                alert("Data Not Found");
            }
            state.interfacedata = result;
            loadQtyInterface();           
        });      
        $("#btnFooterSave").on("click", async function () {
        });
        $("#requisitionDiv").on("click", async function () {
            $(".getpno-col").hide();
            state.selectedInterface = "requisition";
            loadTableInterface();
           
        });
        $("#reqApprovalDiv").on("click", async function () {
            state.selectedInterface = "reqApproved";
            $(".getpno-col").hide();
            loadReqApprovedData();
            
        });
        $("#storeIssueDiv").on("click", async function () {
            state.selectedInterface = "storeissue";
            $(".getpno-col").hide();
            loadGpassData();
            
        });
        $("#storeReceiveDiv").on("click", async function () {
            state.selectedInterface = "storerecive";
            $(".getpno-col").show();
            loadStoreReceiveData();
            
        });

        $(document).on("click", ".btn-confirm", function () {
            let mtreqno = $(this).data("mtreqno");
            let getpno = $(this).data("getpno");
            let checkinterface = state.selectedInterface;
            if (checkinterface === 'reqApproved') {                
                window.open(`/F_07_RM/PurMTReq/PurMTReqIndex?type=approved&mtrref=${mtreqno}`,'_blank');
            }
            else if (checkinterface === 'storeissue') {                
               window.open(`/F_07_RM/PurMTReqGatePass/PurMTReqGatePassIndex?type=entry&mtrref=${mtreqno}`,'_blank');
            } 
            else if (checkinterface === 'storerecive')            {
                     
                window.open(`/F_07_RM/MaterialsTransfer/MaterialTransferIndex?type=entry&getpno=${getpno}`, '_blank');
            }

        });        
        $("#requisitionEntry").on("click", async function () {
             window.open(`/F_07_RM/PurMTReq/PurMTReqIndex?type=entry`,'_blank');
        });        
    }
    async function loadGpassData() {
        if (!state.interfacedata.item1 || state.interfacedata.item1.length === 0) {
            alert("No data found");
            return;
        }
        let data = state.interfacedata.item1 || [];
        let filterdata = data.filter(x =>
            Number(x.gatpbal) !== 0 &&
            x.approved &&
            x.approved.trim().toUpperCase() === "OK"
        );
        ui.renderRequisitionApprovedTable(filterdata);
        document.getElementById("tableContainer").style.display = "table";        
    }
    async function loadStoreReceiveData() {
        if (!state.interfacedata.item2 || state.interfacedata.item1.length === 0) {
            alert("No data found");
            return;
        }
        let data = state.interfacedata.item2 || [];
        let filterdata = data.filter(x =>
            Number(x.gatpqty) > 0 && Number(x.trnbal) > 0
        );
        ui.renderRequisitionApprovedTable(filterdata);
        document.getElementById("tableContainer").style.display = "table";        
    }
    async function loadReqApprovedData() {
        if (!state.interfacedata.item1 || state.interfacedata.item1.length === 0) {
            alert("No data found");
            return;
        }
        let data = state.interfacedata.item1 || [];
        let filterdata = data.filter(x => x.approved.trim().toUpperCase() !== "OK");
        ui.renderRequisitionApprovedTable(filterdata);
        document.getElementById("tableContainer").style.display = "table";
    }
    async function loadQtyInterface() {
        ui.setValue("reqQty", state.interfacedata.item3[0].reqqty);
        ui.setValue("reqApprQty", state.interfacedata.item3[0].reqaqty);
        ui.setValue("storeIssueQty", state.interfacedata.item3[0].gpqty);
        ui.setValue("storeRecvQty", state.interfacedata.item3[0].trnsqty);
        ui.setValue("mattTransQty", state.interfacedata.item3[0].trnsappqty);
    }
    async function loadTableInterface() {
        if (!state.interfacedata.item1 || state.interfacedata.item1.length === 0) {
            alert("No data found");
            return;
        }
        ui.renderRequisitionTable(state.interfacedata.item1);
        document.getElementById("tableContainer").style.display = "table";

    }
    async function loadInitialData() {      
        
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', MatInterfacePage.init);