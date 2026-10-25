VIEW INFORMATION

@{
    ViewData["Title"] = "Production Interface";
    var cards = new[]
    {
        //Item1=Card Div ID, Item2=Name of Card, Item3=Number Id of Card, Item4==completed ? "processed" : "pending"
        ("requestsDiv", "Requests", "reqQty", "requests"),
        ("reqApprovalDiv", "Requisitions", "reqApprQty", "requisitions"),
        ("materialIssueDiv", "Material Issues", "materialIssueQty", "issues"),
        ("issueAppDiv", "Issue Approved", "issueApprQty", "approvals"),
        ("matTransferedDiv", "In Process", "mattTransQty", "process"),
        ("ProEntryProdDiv", "Production Entry", "ProEntryProdIntQty", "production"),
        ("QcEntryDiv", "Quality Control", "QcProdIntQty", "qc"),
        ("FgReceiveProdDiv", "Floor Receive", "FgReceiveProdIntQty", "floorReceive"),
        ("WhReceiveProdDiv", "WH Receive", "WhReceiveProdIntQty", "warehouseReceive"),
        ("CompletedProdDiv", "Completed", "CompletedProdIntQty", "completed"),
        ("ReworkDiv", "Rework", "ReworkQty", "rework")
    };
}

<div class="interface-workspace">
    <div class="interface-toolbar">
        <div class="interface-filter-group" aria-label="Production date filter">
            <div class="interface-filter-field">
                <label for="txtfrmdate">From</label>
                <input id="txtfrmdate" type="text" class="form-control ui-size-xs datepicker datepicker-current" autocomplete="off" />
            </div>
            <div class="interface-filter-field">
                <label for="txttodate">To</label>
                <input id="txttodate" type="text" class="form-control ui-size-xs datepicker datepicker-current" autocomplete="off" />
            </div>
            <button id="btnOk" type="button" class="btn btn-primary ui-size-xs">
                <i class="bi bi-arrow-repeat" aria-hidden="true"></i>
                Load
            </button>
        </div>
        <div class="interface-toolbar-actions">
            <span class="interface-help-text">Select a workflow to view pending transactions</span>
            <button id="requisitionEntry" type="button" class="btn btn-success ui-size-xs">
                <i class="bi bi-plus-circle" aria-hidden="true"></i>
                New budget
            </button>
        </div>
    </div>

    <div class="interface-kpi-grid" id="interfacevalue" aria-label="Production workflow summary">
        @for (int index = 0; index < cards.Length; index++)
        {
            var card = cards[index];
            <button type="button"
                    class="interface-kpi-card @(index == 0 ? "active" : string.Empty)"
                    id="@card.Item1"
                    data-interface="@card.Item4"
                    aria-pressed="@(index == 0 ? "true" : "false")">
                <span class="interface-kpi-title">@card.Item2</span>
                <span class="interface-kpi-meta">
                    <span class="interface-kpi-dot" aria-hidden="true"></span>
                    @(card.Item4 == "completed" ? "Processed" : "Pending")
                </span>
                <strong class="interface-kpi-value" id="@card.Item3">0</strong>
            </button>
        }
    </div>

 <section class="interface-data-card" aria-label="Production workflow records">
        <div class="interface-table-region">
            <div id="tableContainerReq" class="table-responsive" style="display: none;">
                <table id="productionRequestTable" class="table table-hover align-middle mb-0 interface-table">
                    <thead>
                        <tr>
                            <th>#</th>
                            <th>Budget Name</th>
                            <th>Date</th>
                            <th>Product Name</th>
                            <th>Item Count</th>
                            <th>Budgeted Qty</th>
                            <th>Budgeted Amount</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                            <th>Current Status</th>
                            <th>WIP Name</th>
                        </tr>
                    </thead>
                </table>
            </div>
            </div>
    </section>
</div>



//IIFE : Immediately Invoked Function Expression: It calls itself. Private scope. 
//Variables and functions inside the module stay private unless explicitly returned.

var ProductionInterfacePage = (function () {
    // Private code

    return {
        init
    };
})();

//ProductionInterfacePage.loadData(); cant be called as not returned by IIFE
//ProductionInterfacePage.init(); can be called as returned by IIFE

$(ProductionInterfacePage.init);


var ProductionInterfacePage = (function () {
    // Number of Tables for Interfaces
   const tableContainers = [
        "tableContainerReq",
        "tableContainerReqApp",
        ...
    ];
     //FROM ENTERPRISE UI DATATABLE REUSEABLE FUNCTION
    const {
        textColumn,
        wrappedTextColumn,
        numericColumn,
        dateColumn,
        rowNumberColumn
    } = EnterpriseUI.dataTableColumns;
    // Stores values that change while the user uses the page. 
    const state = {
        interfaceData: null, // Latest API response
        selectedInterface: "requests" // Currently selected workflow
    };
   // API CALLS TO CONTROLLER WITH LOADER 
     const service = {
        loadInterfaceData(frmdate, todate) {
            return Helpers.withLoader(() => FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetInterfaceInfo`),
                { frmdate, todate, type }
            ));
        }
    };


       function setCounter(id, value) {
        document.getElementById(id).textContent = Number(value || 0);
    }

    function renderCounters() {
        const summary = state.interfaceData?.item7?.[0] || {};
        setCounter("reqQty", summary.reqqty);
        setCounter("reqApprQty", summary.proreqqty);
        setCounter("materialIssueQty", summary.issueqty);
        setCounter("issueApprQty", summary.isuapqty);
        setCounter("mattTransQty", summary.procsqty);
        setCounter("ProEntryProdIntQty", summary.prodqty);
        setCounter("QcProdIntQty", summary.qcqty);
        setCounter("FgReceiveProdIntQty", summary.florcv);
        setCounter("WhReceiveProdIntQty", summary.strecqty);
        setCounter("CompletedProdIntQty", summary.compqty);
        setCounter("ReworkQty", summary.rework);
    }




    return {
        init
    };
})();


