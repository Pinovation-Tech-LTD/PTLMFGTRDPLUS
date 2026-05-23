/*Area/Controller/*/
const commonPath = "/F_07_RM/MaterialsTransfer/";
var MaterialsTransferPage = (function () {
    var state = {
        selectedModuleId:0,
        pages: [],
    };

    var service = {
        async loaddropdown() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetModules`)));
        },       
        async loadLastTransId(date) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetTransNo`), { date: date }));
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
            const tbody = document.getElementById('permbody');
            tbody.innerHTML = '';
            console.log(items);
            items.forEach((item, index) => {
                tbody.innerHTML += `
            <tr>
                <td>${index + 1}</td>
                <td>${item.moduleName}</td>
                <td>${item.pageName}</td>
                <td>${item.area}/${item.controller}/${item.action}</td>
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
    }
    async function LastTransId() {
        const date = ui.getValue("txtdate");
        const formatdate = formatDate(date);
        const TransId = await service.loadLastTransId(formatdate);
        ui.renderCurTransNo(TransId);
    }
    async function loadInitialData() {
        LastTransId();
    }
    return {
        init: init
    };
})();

document.addEventListener('DOMContentLoaded', MaterialsTransferPage.init);