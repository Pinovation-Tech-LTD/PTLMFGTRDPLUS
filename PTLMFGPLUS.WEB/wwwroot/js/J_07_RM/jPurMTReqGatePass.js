/*Area/Controller/*/
const commonPath = "/F_07_RM/PurMTReqGatePass/";
const urlParams = new URLSearchParams(window.location.search);
const mtreqno = urlParams.get("mtrref");
var MTReqGatePassPage = (function () {
    var state =
    {

    };
    var service = {

        async loadGPassNo(frmdate, todate) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPassNo`), { frmdate: frmdate, todate: todate }));
        },
        async loadGPassData(curdate, searchtext) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPassData`), { curdate: curdate, searchtext: searchtext }))
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
        });
    }
    async function GetGPassIdByDate() {
        const date = ui.getValue("txtdate");
        const formatteddate = formatDate(date);       
        const moduleItems = await service.loadGPassNo(formatteddate);
        ui.renderCurTransNo(moduleItems);        
    }
    async function GetGPassData() {
        const date = ui.getValue("txtdate");
        const curdate1 = formatDate(date);
        const GPassData = await service.loadGPassData(curdate1, "%");
        
    }
    async function loadInitialData() {
        GetGPassIdByDate();
        GetGPassData();
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', MTReqGatePassPage.init);