/*Area/Controller/*/
const commonPath = "/F_13_ProdMon/ProductionInterface/";
const urlParams = new URLSearchParams(window.location.search);
var ProductionInterfacePage = (function () {
    var state =
    {
        interfacedata: [],
        selectedInterface: ""
    };
    var service = {
        async loadInterfaceData(frmdate, todate) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetInterfaceInfo`), { frmdate: frmdate, todate: todate }));
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
            const tbody = document.getElementById('selectedtabledataReq');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>                 
                <td class="fs-6">${item.pbmno1}</td> 
                <td class="fs-6">
    ${new Date(item.bgddat).toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric'
                })}
        </td>
                <td class="fs-6">${item.prodesc}</td> 
                <td class="fs-6">${item.itemcount}</td>  
                <td class="fs-6">${item.bgdwqty}</td> 
                <td class="fs-6">${item.bgdamt}</td>
                <td class="fs-6">
    ${new Date(item.sdate).toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric'
                })}
</td>
               <td class="fs-6">
    ${new Date(item.enddate).toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric'
                })}
</td>          
                <td class="fs-6">${item.pbmststus}</td>              
                <td class="fs-6">${item.actdesc}</td>              
            </tr>
        `;
            });
        },
        renderRequisitionApprovedTable(items) {
            const tbody = document.getElementById('selectedtabledataReqApp');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.pbmno1}</td>                
                <td class="fs-6">
    ${new Date(item.bgddat).toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric'
                })}
</td>
                <td class="fs-6">${item.prodesc}</td> 
                <td class="fs-6">${item.itemcount}</td> 
                <td class="fs-6">${item.bgdwqty}</td> 
                <td class="fs-6">${item.bgdamt}</td> 
                <td class="fs-6">${item.bgdbal}</td> 
                <td class="fs-6">${item.actdesc}</td>               
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-mtreqno="${item.pbmno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>

                        <button class="btn btn-primary btn-edit" data-mtreqno="${item.pbmno}"  title="Edit">
                            <i class="bi bi-pencil-square"></i>
                        </button>

                        <button class="btn btn-danger btn-delete" data-mtreqno="${item.pbmno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        },
        renderRequisitionStoreIssue(items) {
            const tbody = document.getElementById('selectedtabledataMaterialissue');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.pbmno1}</td>               
                <td class="fs-6">${item.preqno1}</td> 
                 <td class="fs-6">
    ${new Date(item.pbdate).toLocaleDateString('en-GB', {
        day: '2-digit',
        month: 'short',
        year: 'numeric'
    })}
</td>
                <td class="fs-6">${item.batchdesc}</td> 
                <td class="fs-6">${item.fgreqqty}</td> 
                <td class="fs-6">${item.trescount}</td> 
                <td class="fs-6">${item.rcount}</td> 
                <td class="fs-6">${item.isstatus}</td>               
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-mtreqno="${item.mtreqno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-mtreqno="${item.mtreqno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        },
        renderRequisitionIssueApprove(items) {
            const tbody = document.getElementById('selectedtabledataIssueAppr');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.misuno1}</td>                            
               <td class="fs-6">
    ${new Date(item.misudate).toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric'
                })}
</td>
                <td class="fs-6">${item.preqno1}</td> 
                <td class="fs-6">${item.actdesc}</td> 
                <td class="fs-6">${item.batchdesc}</td> 
                <td class="fs-6">${item.fgreqqty}</td> 
                <td class="fs-6">${item.fgisuqty}</td> 
                <td class="fs-6">${item.isuqty}</td>                
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-getpno="${item.getpno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-getpno="${item.getpno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        },
        renderRequisitionProEntry(items) {
            const tbody = document.getElementById('selectedtabledataproentry');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.pbmno1}</td>                            
               <td class="fs-6">
    ${new Date(item.pbdate).toLocaleDateString('en-GB', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric'
                })}
</td>
                <td class="fs-6">${item.prodesc}</td> 
                <td class="fs-6">${item.batchdesc}</td> 
                <td class="fs-6">${item.itemcount}</td> 
                <td class="fs-6">${item.acqty}</td> 
                <td class="fs-6">${item.balqty}</td> 
                <td class="fs-6">${item.proamt}</td>                
                <td class="fs-6">${item.proatatus}</td>                
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-getpno="${item.getpno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-getpno="${item.getpno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        },
        renderRequisitionQcEntry(items) {
            const tbody = document.getElementById('selectedtabledataqcentry');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.pbmno1}</td>                           
                <td class="fs-6">${item.prodid}</td> 
                <td class="fs-6">
                    ${new Date(item.prodate).toLocaleDateString('en-GB', {
                        day: '2-digit',
                        month: 'short',
                        year: 'numeric'
                    })}
                </td>
                <td class="fs-6">${item.prodesc}</td> 
                <td class="fs-6">${item.batchdesc}</td> 
                <td class="fs-6">${item.itemcount}</td> 
                <td class="fs-6">${item.balqty}</td> 
                <td class="fs-6">${item.proamt}</td>                
                <td class="fs-6">${item.pstatus}</td>                
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-getpno="${item.pbmno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-getpno="${item.pbmno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        },
        renderRequisitionFgReceive(items) {
            const tbody = document.getElementById('selectedtabledatafgreceive');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.prodid}</td>                           
                <td class="fs-6">
                    ${new Date(item.prodate).toLocaleDateString('en-GB', {
                        day: '2-digit',
                        month: 'short',
                        year: 'numeric'
                    })}
                </td>
                <td class="fs-6">${item.grrno}</td> 
                <td class="fs-6">${item.batchdesc}</td> 
                <td class="fs-6">${item.itemcount}</td> 
                <td class="fs-6">${item.proqty}</td> 
                <td class="fs-6">${item.proamt}</td>                
                <td class="fs-6">${item.rcvtype}</td>                
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-getpno="${item.pbmno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-getpno="${item.pbmno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        }
        ,
        renderRequisitionWhReceive(items) {
            const tbody = document.getElementById('selectedtabledatawhreceive');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.prodid}</td>                           
                <td class="fs-6">
                    ${new Date(item.prodate).toLocaleDateString('en-GB', {
                        day: '2-digit',
                        month: 'short',
                        year: 'numeric'
                    })}
                </td>
                <td class="fs-6">${item.grrno}</td> 
                <td class="fs-6">${item.batchdesc}</td> 
                <td class="fs-6">${item.itemcount}</td> 
                <td class="fs-6">${item.proqty}</td> 
                <td class="fs-6">${item.proamt}</td>                
                <td class="fs-6">${item.rcvtype}</td> 
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-getpno="${item.pbmno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-getpno="${item.pbmno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        }
        ,
        renderRequisitionmatTransfered(items) {
            const tbody = document.getElementById('selectedtabledatamattransfered');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.pbno}</td>                           
                <td class="fs-6">${item.preqno}</td> 
                <td class="fs-6">
                    ${new Date(item.pbdate).toLocaleDateString('en-GB', {
                        day: '2-digit',
                        month: 'short',
                        year: 'numeric'
                    })}
                </td>
                <td class="fs-6">${item.batchdesc}</td> 
                <td class="fs-6">${item.rsirdesc}</td> 
                <td class="fs-6">${item.itemcount}</td> 
                <td class="fs-6">${item.acqty}</td>                
                <td class="fs-6">${item.processbal}</td> 
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-getpno="${item.pbmno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-getpno="${item.pbmno}"  title="Delete">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                </td>
            </tr>
        `;
            });
        }
        ,
        renderRequisitionCompletedProd(items) {
            const tbody = document.getElementById('selectedtabledatacompletedprod');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.pbmno1}</td>                           
                <td class="fs-6">
                    ${new Date(item.bgddat).toLocaleDateString('en-GB', {
                        day: '2-digit',
                        month: 'short',
                        year: 'numeric'
                    })}
                </td>
                <td class="fs-6">${item.itemcount}</td> 
                <td class="fs-6">${item.bgdwqty}</td> 
                <td class="fs-6">${item.bgdamt}</td> 
                <td class="fs-6">${item.pbmststus}</td>                
                <td>
                    <div class="btn-group btn-group-sm">
                        <button class="btn btn-success btn-confirm" data-getpno="${item.pbmno}"  title="Confirm">
                            <i class="bi bi-check-lg"></i>
                        </button>
                        <button class="btn btn-danger btn-delete" data-getpno="${item.pbmno}"  title="Delete">
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
        loadInterfaceQtyData();

    }
    async function bindEvents() {
        $("#btnOk").on("click", async function () {
            loadInterfaceQtyData();
            
        });
        $("#btnFooterSave").on("click", async function () {
        });
        $("#requestsDiv").on("click", async function () {
            state.selectedInterface = "requisition";
            loadTableInterface();
        });
        $("#reqApprovalDiv").on("click", async function () {
            state.selectedInterface = "reqApproved";
            loadReqApprovedData();
        });
        $("#materialIssueDiv").on("click", async function () {
            state.selectedInterface = "materialissue";
            loadMateIssueData();
        });
        $("#issueAppDiv").on("click", async function () {
            state.selectedInterface = "issueapprove";
            loadIssueApprData();
        });
        $("#matTransferedDiv").on("click", async function () {
            state.selectedInterface = "mattransfered";
            loadmatTransferedData();
        });
        $("#ProEntryProdDiv").on("click", async function () {
            state.selectedInterface = "proentry";
            loadProEntryData();
        });
        $("#QcEntryDiv").on("click", async function () {
            state.selectedInterface = "qcproduction";
            loadQcEntryData();
        });
        $("#FgReceiveProdDiv").on("click", async function () {
            state.selectedInterface = "fgreceive";
            loadFgReceiveData();
        });
        $("#WhReceiveProdDiv").on("click", async function () {
            state.selectedInterface = "whreceive";
            loadWhReceiveData();
        });
        $("#CompletedProdDiv").on("click", async function () {
            state.selectedInterface = "completedprod";
            loadCompletedProdData();
        });


        $(document).on("click", ".btn-confirm", function () {
            let mtreqno = $(this).data("mtreqno");
            let getpno = $(this).data("getpno");
            let checkinterface = state.selectedInterface;
            if (checkinterface === 'reqApproved') {
                window.open(`/F_07_RM/PurMTReq/PurMTReqIndex?type=approved&mtrref=${mtreqno}`, '_blank');
            }
            else if (checkinterface === 'storeissue') {
                window.open(`/F_07_RM/PurMTReqGatePass/PurMTReqGatePassIndex?type=entry&mtrref=${mtreqno}`, '_blank');
            }
            else if (checkinterface === 'storerecive') {

                window.open(`/F_13_ProdMon/MaterialsTransfer/MaterialTransferIndex?type=entry&getpno=${getpno}`, '_blank');
            }

        });
        $("#requisitionEntry").on("click", async function () {
            window.open(`/F_13_ProdMon/ProdBudget/ProdBudgetIndex?type=Entry`, '_blank');
        });
    }
    async function loadInterfaceQtyData() {
        let frmdate = ui.getValue("txtfrmdate");
        let todate = ui.getValue("txttodate");
        const formatfrmdate = (frmdate);
        const formattodate = (todate);

        const result = await service.loadInterfaceData(formatfrmdate, formattodate);
        if (result === null) {
            Notifications.Toastr.error("No data Found");
        }
        state.interfacedata = result;
        loadQtyInterface();
    }
    async function loadMateIssueData() {
        if (!state.interfacedata.item1 || state.interfacedata.item1.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item1 || [];
        // let filterdata = data.filter(x =>
        //     Number(x.gatpbal) !== 0 &&
        //     x.approved &&
        //     x.approved.trim().toUpperCase() === "OK"
        // );
        ui.renderRequisitionStoreIssue(data);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "table";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadIssueApprData() {
        if (!state.interfacedata.item6 || state.interfacedata.item6.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item6 || [];
       
        ui.renderRequisitionIssueApprove(data);
        document.getElementById("tableContainerIssueAppr").style.display = "table";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadmatTransferedData() {
        if (!state.interfacedata.item5 || state.interfacedata.item5.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item5 || [];
       
        ui.renderRequisitionmatTransfered(data);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "table";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadProEntryData() {
        if (!state.interfacedata.item2 || state.interfacedata.item2.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item2 || [];
       
        ui.renderRequisitionProEntry(data);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "table";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadQcEntryData() {
        if (!state.interfacedata.item3 || state.interfacedata.item3.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item3 || [];
       
        ui.renderRequisitionQcEntry(data);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "table";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadFgReceiveData() {
        if (!state.interfacedata.item4 || state.interfacedata.item4.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item4 || [];
        let filterdata = data.filter(x => x.qcapprove === '');
        ui.renderRequisitionFgReceive(filterdata);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "table";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadWhReceiveData() {
        if (!state.interfacedata.item4 || state.interfacedata.item4.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item4 || [];
        let filterdata = data.filter(x => x.qcapprove !== '');
        ui.renderRequisitionWhReceive(filterdata);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "table";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadCompletedProdData() {
        if (!state.interfacedata.item0 || state.interfacedata.item0.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item0 || [];
        let filterdata = data.filter(x => x.pbmststus === "Complete");   
        if (!filterdata || filterdata.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        ui.renderRequisitionCompletedProd(filterdata);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "table";
    }
    async function loadReqApprovedData() {
        if (!state.interfacedata.item0 || state.interfacedata.item0.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        let data = state.interfacedata.item0 || [];
        let filterdata = data.filter(x => x.pbmststus === "Requsition");
        ui.renderRequisitionApprovedTable(filterdata);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "table";
        document.getElementById("tableContainerReq").style.display = "none";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";

    }
    async function loadQtyInterface() {
        ui.setValue("reqQty", Number(state.interfacedata.item7[0].reqqty));
        ui.setValue("reqApprQty", Number(state.interfacedata.item7[0].proreqqty));
        ui.setValue("materialIssueQty", Number(state.interfacedata.item7[0].issueqty));
        ui.setValue("issueApprQty", Number(state.interfacedata.item7[0].isuapqty));
        ui.setValue("mattTransQty", Number(state.interfacedata.item7[0].procsqty));
        ui.setValue("ProEntryProdIntQty",Number(state.interfacedata.item7[0].prodqty))
        ui.setValue("QcProdIntQty",Number(state.interfacedata.item7[0].qcqty))
        ui.setValue("FgReceiveProdIntQty",Number(state.interfacedata.item7[0].florcv))
        ui.setValue("WhReceiveProdIntQty", Number(state.interfacedata.item7[0].strecqty))
        ui.setValue("CompletedProdIntQty", Number(state.interfacedata.item7[0].compqty))
    }
    async function loadTableInterface() {
        if (!state.interfacedata.item0 || state.interfacedata.item0.length === 0) {
            Notifications.Toastr.error("No data Found");
            return;
        }
        ui.renderRequisitionTable(state.interfacedata.item0);
        document.getElementById("tableContainerIssueAppr").style.display = "none";
        document.getElementById("tableContainerMaterialissue").style.display = "none";
        document.getElementById("tableContainerReqApp").style.display = "none";
        document.getElementById("tableContainerReq").style.display = "table";
        document.getElementById("tableContainerProentry").style.display = "none";
        document.getElementById("tableContainerQcEntry").style.display = "none";
        document.getElementById("tableContainerFgReceive").style.display = "none";
        document.getElementById("tableContainerWhReceive").style.display = "none";
        document.getElementById("tableContainermatTransfered").style.display = "none";
        document.getElementById("tableContainerCompletedProd").style.display = "none";


    }
    async function loadInitialData() {

    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', ProductionInterfacePage.init);