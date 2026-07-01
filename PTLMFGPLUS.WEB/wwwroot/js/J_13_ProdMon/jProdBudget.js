/*Area/Controller/*/
const commonPath = "/F_13_ProdMon/ProdBudget/";
const urlParams = new URLSearchParams(window.location.search);
const qparam = urlParams.get("type");
var ProdBudgetPage = (function () {
    var state =
    {
        ProductList: [],
        tblbbudget: [],
    };
    var service = {        
        async loadProdBudgetNoData(date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProdBudgetNo`), { date:date }));
        },
        async loadPreviousBudgetListData(type) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPreviousBudgetList`), { type:type }));
        },
        async loadProductListData(type,date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProductList`), { type:type,date:date }));
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
        }

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
                $("input[name='rbtnlist']").prop("checked", false);
                document.querySelector(".rbtnList1").style.display = "none";
                
            }
        });
        $("#lblPrePBNo").on("click", function () {
            PreviousBudgetList();
        });
        $(document).on("click", "input[name='rbtnlist']", async function () {           
            if ($(this).val() === "0")
            {
                await ProductSelection();
                console.log("Product Selection");
            }            
            else if ($(this).val() === "1")
            {
                console.log("Material Input");
            }            
            else if ($(this).val() === "2")
            {
                console.log("Reports");
            }            
        });
        $("#btnSelectAll").on("click", function () {
            btnSelectAllData();
        });
        $("#btnFooterSave").on("click", async function () {
           
        });

    }
    async function ProdBudgetNo()
    {
        let date = ui.getValue("txtbgddate");
        const BudgetNo = await service.loadProdBudgetNoData(date);        
        document.getElementById("lblBpn").value = BudgetNo[0].bpno;
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
        }

    }
    async function ProductSelection()
    {
        let date = ui.getValue("txtbgddate");
        const type = qparam;
        const productlist = await service.loadProductListData(type, date);
        state.ProductList = productlist;
        document.getElementById("ProductSelection").style.display = "flex";
        ui.renderProductList(productlist);
    }
    async function PreviousBudgetList()
    {
        const type = qparam;
        const PrevBudgetList = await service.loadPreviousBudgetListData(type);        
        ui.renderPreviousBudgetList(PrevBudgetList);
    }
    async function loadInitialData() {        
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', ProdBudgetPage.init);