const commonPath = "/controlpanel/resourceperm/";
var CompanyPermPage = (function () {
    var state = {

    };

    var service = {
        async loaddropdown() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetModules`)));
        },

        async loadAllPages() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetAllPages`)));
        },

        async loadPagesModuleWise(id) {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetPagesModuleWise`), { moduleId: id }));
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
        renderDropdown(items) {
            console.log(items);
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
    }
    function bindEvents() {
        //$('#ddlModule').on('change', function (e) {
        //    pageDataModuleWise(e);
        //});
        $("#btnOk").on("click", function () {
            let moduleId = ui.getValue("ddlModule");
            pageDataModuleWise(moduleId);
        })
    }
    async function loadInitialData() {
        const moduleItems = await service.loaddropdown();
        ui.renderDropdown(moduleItems);
        const data = await service.loadAllPages();
        ui.renderTable(data);
    }

    async function pageDataModuleWise(e) {
        const moduleId = e;//.target.value;
        const pages = await service.loadPagesModuleWise(moduleId);
        ui.renderTable(pages);
    }

    return {
        init: init
    };
})();




document.addEventListener('DOMContentLoaded', CompanyPermPage.init);
