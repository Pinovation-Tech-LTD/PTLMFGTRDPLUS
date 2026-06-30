/*Area/Controller/*/
const commonPath = "/F_13_ProdMon/ProdBudget/";
const urlParams = new URLSearchParams(window.location.search);
const qparam = urlParams.get("type");
var ProdBudgetPage = (function () {
    var state =
    {

    };
    var service = {        
        async loadProdBudgetNoData(date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetProdBudgetNo`), { date:date }));
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
            }
            else {
                btn.innerHTML = '<i class="bi bi-check-lg"></i> Ok';  
                document.getElementById("previousBudget").style.display = "inline-block";
                document.querySelector(".rbtnList1").style.display = "none";
            }
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
    async function loadInitialData() {        
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', ProdBudgetPage.init);