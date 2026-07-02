/*Area/Controller/*/
const commonPath = "/F_13_ProdMon/ProdBudget/";
const urlParams = new URLSearchParams(window.location.search);
const qparam = urlParams.get("type");
var ProdBudgetPage = (function () {
    var state =
    {
        ProductList: [],
        tblbbudget: [],
        tblbbudgetlog:[],
    };
    var service = {
        async loadProdBudgetNoData(date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProdBudgetNo`), { date: date }));
        },
        async loadPreviousBudgetListData(type) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPreviousBudgetList`), { type: type }));
        },
        async loadProductListData(type, date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProductList`), { type: type, date: date }));
        },
        async loadShowProductListData(type,batchno, date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetShowProductList`), { type: type,batchno:batchno, date: date }));
        },
        async Final_Update_Button(type, date, pbnno, sDate, tDate, BatchNameText, selectedItem, dtuser) {

            return Helpers.withLoader(() => FetchHelpers.postForm(FetchHelpers.urlconfig(`${commonPath}FinalUpdateButtonClick`),
                {
                    type: type, date: date, pbnno: pbnno, sDate: sDate, tDate: tDate, BatchNameText: BatchNameText, selectedItem: JSON.stringify(selectedItem),
                    dtuser: JSON.stringify(dtuser)
                }));
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
        renderPreviousBudgetList(items) {
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
                selector: "ddlPreProlist",
                data: data,
                valueField: "pbmno",
                textField: "pbmno1",
                selectedValue: data[0].pbmno
            })
        },
        renderProductList(items) {
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
                selector: "ddlProductList",
                data: data,
                valueField: "prodcode",
                textField: "proddesc",
                selectedValue: data[0].prodcode
            })
        },
        renderSelectedBudget(items) {
            const tbody = document.getElementById('selectedtabledataprodbudget');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td class="fs-6">${index + 1}</td>
                <td class="fs-6">${item.proddesc1}</td>
                <td class="fs-6">${item.proddesc}</td> 
                <td class="fs-6">${item.produnit}</td> 
                <td class="fs-6">${item.targetqty}</td> 
                <td class="fs-6">${item.stqty}</td> 
                <td class="fs-6">${item.nproqty}</td>                        
               <td>
                    <input type="number"
                   class="form-control form-control-sm qty-input w-100"
                   data-index="${index}"
                   value="${item.bgdwqty || 0}"
                   style="max-width: 100px;"
                   >
               </td>
            </tr>
        `;
            });
        }

    };
    function init() {
        bindEvents();
        loadInitialData();
        HideFooterWithButtons();
        HideRefresh();
        HideRecalculate();
        HideSave();
        HideCancel();
    }
    function bindEvents() {
        $("#btnOk").on("click", function () {
            const btn = document.getElementById("btnOk");
            const buttonText = btn.textContent.trim();
            if (buttonText === "Ok") {
                ProdBudgetNo();
                document.getElementById("previousBudget").style.display = "none";
                document.querySelector(".rbtnList1").style.display = "flex";
                btn.innerHTML = '<i class="bi bi-check-lg"></i> New';
                if (document.getElementById('ddlPreProlist').options.length > 1) {
                    console.log("ddl list ");
                }
                else {

                }
                return;
            }
            else {
                btn.innerHTML = '<i class="bi bi-check-lg"></i> Ok';
                document.getElementById("previousBudget").style.display = "inline-block";
                document.getElementById("ProductSelection").style.display = "none";
                document.getElementById("tableProdBudget").style.display = "none";
                $("input[name='rbtnlist']").prop("checked", false);
                document.querySelector(".rbtnList1").style.display = "none";
                state.tblbbudget = [];

            }
        });
        $("#lblPrePBNo").on("click", function () {
            PreviousBudgetList();
        });
        $(document).on("click", "input[name='rbtnlist']", async function () {
            if ($(this).val() === "0") {
                await ProductSelection();
                await ShowProductList();                
            }
            else if ($(this).val() === "1") {
                console.log("Material Input");
            }
            else if ($(this).val() === "2") {
                console.log("Reports");
            }
        });
        $("#btnSelectAll").on("click", function () {
            btnSelectAllData();
        });
        $("#btnSelect").on("click", function () {
            btnSelectData();
        });

        $(document).on("change", ".qty-input", function () {
            let index = $(this).data("index");
            let qty = parseFloat($(this).val()) || 0;
            let item = state.tblbbudget[index];
            item.bgdwqty = qty;           
        });
        $("#btnTotal").on("click", function () {
            calculateBgdwqtyTotal();
        });
        $("#btnFinalUpdate").on("click", async function () {
            FinalUpdateClick();
        });
        $("#btnFooterSave").on("click", async function () {

        });

    }
    async function ProdBudgetNo() {
        let date = ui.getValue("txtbgddate");
        const BudgetNo = await service.loadProdBudgetNoData(date);

        if (BudgetNo && BudgetNo.length > 0) {
            document.getElementById("txtBpn").value = BudgetNo[0].bpno;
        } else {
            document.getElementById("txtBpn").value = "";
        }
    }
    async function btnSelectAllData() {
        const ProdCode = ui.getValue("ddlProductList");
        let tbl1 = state.tblbbudget || [];
        let tbl2 = state.ProductList || [];

        let filtertblbbudget = tbl1.filter(row => row.prodcode === ProdCode);

        if (filtertblbbudget.length === 0) {
            for (let i = 0; i < tbl2.length; i++) {
                let src = tbl2[i];

                let newRow = {
                    scode: src.sirtdes,
                    prodcode1: src.prodcode1,
                    proddesc1: src.proddesc1,
                    prodcode: src.prodcode,
                    proddesc: src.proddesc,
                    produnit: src.produnit,
                    targetqty: parseFloat(src.targetqty) || 0,
                    stqty: parseFloat(src.stqty) || 0,
                    nproqty: parseFloat(src.nproqty) || 0,
                    bgdwqty: 0.00
                };

                tbl1.push(newRow);
            }

            state.tblbbudget = tbl1;
            await Data_Bind_Budget();
        }

    }
    async function btnSelectData() {
        const ProdCode = ui.getValue("ddlProductList");
        let tbl1 = state.tblbbudget || [];
        let dtp = state.ProductList || [];

        let filtertblproductlists = dtp.filter(row => row.prodcode === ProdCode);
        if (filtertblproductlists.length === 0) {
            Notifications.Sweet.warning("No Information found for Selected Product");
            return;
        }

        let filtertblproduct = filtertblproductlists[0];

        document.getElementById("txtBatchName").value = filtertblproduct.resdesc1 + "-XXXX";

        let filtertblbbudget = tbl1.filter(row => row.prodcode === ProdCode);

        if (filtertblbbudget.length === 0) {
            
                let newRow = {
                    scode: filtertblproduct.sirtdes,
                    prodcode1: filtertblproduct.prodcode1,
                    proddesc1: filtertblproduct.proddesc1,
                    prodcode: ProdCode,
                    proddesc: ui.getValue("ddlProductList"),
                    produnit: filtertblproduct.produnit,
                    targetqty: parseFloat(filtertblproduct.targetqty) || 0,
                    stqty: parseFloat(filtertblproduct.stqty),
                    nproqty: parseFloat(filtertblproduct.nproqty),
                    bgdwqty: parseFloat(filtertblproduct.stdqty) || 0
                };

                tbl1.push(newRow);           

            state.tblbbudget = tbl1;
            await Data_Bind_Budget();
        }

    }
    async function Data_Bind_Budget() {
        if (state.tblbbudget.length === 0) {
            return;
        }
        document.getElementById("tableProdBudget").style.display = "table";
        ui.renderSelectedBudget(state.tblbbudget);
    }
    async function FinalUpdateClick() {
        let sDate = ui.getValue("txtfrmdate");
        let tDate = ui.getValue("txttodate");
        let date = ui.getValue("txtbgddate");
        let tbl2 = state.tblbbudget || [];
         let bpnText = ui.getValue("txtBpn");
        let pbnno = bpnText.substring(0, 9) + bpnText.substring(10);

        let BatchNameText = ui.getValue("txtBatchName");
        if (BatchNameText.includes("XXXX"))
        {
            Notifications.Sweet.error("You Missing Batch Number");
            return;
        }
        let type = qparam;
        if (type === "EntryPreSemi") {
            if (ui.getValue("txtBatchName") === null)
            {
                Notifications.Sweet.error("You Missing Batch Number");
            }
        }
        const selectedItem = state.tblbbudget;
        const dtuser = state.tblbbudgetlog;
        let result = await service.Final_Update_Button(type, date, pbnno, sDate, tDate, BatchNameText, selectedItem,dtuser);


    }

    async function ProductSelection() {
        let date = ui.getValue("txtbgddate");
        const type = qparam;
        const productlist = await service.loadProductListData(type, date);
        state.ProductList = productlist;
        document.getElementById("ProductSelection").style.display = "flex";
        ui.renderProductList(productlist);
    }
    async function PreviousBudgetList() {
        const type = qparam;
        const PrevBudgetList = await service.loadPreviousBudgetListData(type);
        ui.renderPreviousBudgetList(PrevBudgetList);
    }
    async function ShowProductList() {
        const type = qparam;
        let bpnText = ui.getValue("txtBpn");
        let batchno = bpnText.substring(0, 9) + bpnText.substring(10);
        let date = ui.getValue("txtbgddate");

        const ShowProdList = await service.loadShowProductListData(type, batchno, date);
        state.tblbbudget = ShowProdList.item1;
        state.tblbbudgetlog = ShowProdList.item2;
        if (state.tblbbudget.length === 0) {
            document.getElementById("btnSelectAll").style.display = "flex";
        }
        else {
            document.getElementById("btnSelectAll").style.display = "none";
        }
        Data_Bind_Budget();
        
    }
    function calculateBgdwqtyTotal() {
        let items = state.tblbbudget || [];
        let total = items.reduce((sum, item) => {
            return sum + (parseFloat(item.bgdwqty) || 0);
        }, 0);

        document.getElementById("bgdwqtyTotal").innerText = total;
    }
    async function loadInitialData() {
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', ProdBudgetPage.init);