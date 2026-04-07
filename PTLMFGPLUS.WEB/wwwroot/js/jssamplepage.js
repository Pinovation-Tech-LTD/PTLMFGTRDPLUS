/*Area/Controller/*/
const commonPath = "/controlpanel/resourceperm/";
var ActionPage = (function () {
    var state = {
        selectedModuleId=0,
        pages: [],
    };

    var service = {
        async loaddropdown() {
            return Helpers.withLoader(() => FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}GetModules`)));
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
            //let moduleId = ui.getValue("ddlModule");
            //pageDataModuleWise(moduleId);
        });

        $("#btnFooterSave").on("click", function () {
            //let moduleId = ui.getValue("ddlModule");
            //pageDataModuleWise(moduleId);
        });



    }
    async function loadInitialData() {
        //const moduleItems = await service.loaddropdown();
        //ui.renderDropdown(moduleItems);
        //const data = await service.loadAllPages();
        //state.pages = data;
        //ui.renderTable(data);
    }
    return {
        init: init
    };
});

document.addEventListener('DOMContentLoaded', ActionPage.init);