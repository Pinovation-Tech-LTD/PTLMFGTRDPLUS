const commonPath = "/ControlPanel/ComPermission/";

var CompanyPermPage = (function () {
    "use strict";

    var state = {
        selectedModuleId: "AA",
        pages: [],
        table: null
    };

    var service = {
        async loadPermissions(moduleId, showAll) {
            return Helpers.withLoader(() => FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}GetPermissions`),
                {
                    moduleId: moduleId,
                    showAll: showAll
                }
            ));
        },

        async savePermissions(payload, verificationToken) {
            const response = await fetch(FetchHelpers.urlconfig(`${commonPath}SavePermissions`), {
                method: "POST",
                headers: {
                    "Content-Type": "application/json; charset=utf-8",
                    "RequestVerificationToken": verificationToken
                },
                body: JSON.stringify(payload)
            });
            const result = await response.json();

            if (!response.ok) {
                throw result;
            }

            return result;
        }
    };

    var ui = {
        getValue(id) {
            const element = document.getElementById(id);
            return element ? element.value : "";
        },

        isChecked(id) {
            const element = document.getElementById(id);
            return element ? element.checked : false;
        },

        getVerificationToken() {
            const token = document.querySelector('input[name="__RequestVerificationToken"]');
            return token ? token.value : "";
        },

        setSavePending(isPending) {
            const button = document.getElementById("btnFooterSave");

            if (button) {
                button.disabled = isPending;
            }
        },

        encode(value) {
            return $("<div>").text(value ?? "").html();
        },

        getErrorMessage(error, fallback) {
            return error?.objError || error?.message || fallback;
        },

        syncVisibleRows() {
            document.querySelectorAll("#tblpageperCheck tbody tr[data-index]").forEach(function (row) {
                const item = state.pages[Number(row.dataset.index)];

                if (!item) {
                    return;
                }

                const aliasInput = row.querySelector(".permission-alias");
                const permissionInput = row.querySelector(".row-permission");

                item.alias = aliasInput?.value.trim() || item.dscrption;
                item.chkper = Boolean(permissionInput?.checked);
            });
        },

        updateSummary() {
            ui.syncVisibleRows();

            const availableCount = document.getElementById("availableCount");
            const selectedCount = document.getElementById("selectedCount");
            const checkAll = document.getElementById("chktblall");
            const allSelected = state.pages.length > 0 && state.pages.every(item => item.chkper);
            const someSelected = state.pages.some(item => item.chkper);

            availableCount.textContent = state.pages.length;
            selectedCount.textContent = state.pages.filter(item => item.chkper).length;
            checkAll.checked = allSelected;
            checkAll.indeterminate = someSelected && !allSelected;
        },

        renderTable(items) {
            EnterpriseUI.destroyDataTable("#tblpageperCheck");
            state.table = null;

            const tbody = document.querySelector("#tblpageperCheck tbody");

            tbody.innerHTML = items.map(function (item, index) {
                return `
                    <tr data-index="${index}">
                        <td class="text-end">
                            ${index + 1}.
                        </td>
                        <td>
                            <span class="module-name">
                                ${ui.encode(item.modulename)}
                            </span>
                            <small>
                                ${ui.encode(item.fmgrpdesc)}
                            </small>
                        </td>
                        <td>
                            ${ui.encode(item.dscrption)}
                        </td>
                        <td>
                            <input type="text"
                                   class="form-control ui-size-xm permission-alias"
                                   value="${ui.encode(item.alias || item.dscrption)}"
                                   maxlength="100"
                                   aria-label="Page alias" />
                        </td>
                        <td class="text-center">
                            <input type="checkbox"
                                   class="form-check-input row-permission"
                                   ${item.chkper ? "checked" : ""}
                                   aria-label="Permit page" />
                        </td>
                    </tr>
                `;
            }).join("");

            state.table = EnterpriseUI.createDataTable("#tblpageperCheck", {
                order: [],
                autoWidth: false,
                stateSave: false,
                columnDefs: [
                    {
                        orderable: false,
                        targets: [0, 3, 4]
                    },
                    {
                        searchable: false,
                        targets: [0, 4]
                    }
                ],
                language: {
                    emptyTable: "No pages are available for this module.",
                    search: "Search pages:"
                }
            });

            ui.updateSummary();
        }
    };

    function init() {
        state.selectedModuleId = ui.getValue("ddlmodule") || "AA";
        configureFooter();
        bindEvents();
        loadPermissions();
    }

    function configureFooter() {
        ShowFooterWithButtons();
        HideRefresh();
        HideRecalculate();
        HideCancel();
        HideApprove();
    }

    function bindEvents() {
        $("#ddlmodule").on("change", function () {
            state.selectedModuleId = ui.getValue("ddlmodule");
            loadPermissions();
        });

        document.getElementById("showAll").addEventListener("change", function () {
            loadPermissions();
        });

        document.getElementById("chktblall").addEventListener("change", function () {
            const isChecked = this.checked;

            state.pages.forEach(function (item) {
                item.chkper = isChecked;
            });

            ui.renderTable(state.pages);
        });

        $("#tblpageperCheck tbody").on(
            "change input",
            ".row-permission, .permission-alias",
            function () {
                ui.updateSummary();
            }
        );

        document.getElementById("btnFooterSave").addEventListener("click", function () {
            savePermissions();
        });
    }

    async function loadPermissions() {
        try {
            const pages = await service.loadPermissions(
                state.selectedModuleId,
                ui.isChecked("showAll")
            );
            state.pages = pages || [];
            ui.renderTable(state.pages);
        }
        catch (error) {
            state.pages = [];
            ui.renderTable(state.pages);
            Notifications.Toastr.error(
                "Load failed",
                ui.getErrorMessage(error, "Company permissions could not be loaded.")
            );
        }
    }

    async function savePermissions() {
        ui.syncVisibleRows();
        ui.setSavePending(true);

        const payload = {
            moduleId: state.selectedModuleId,
            permissions: state.pages.map(function (item) {
                return {
                    frmid: item.frmid,
                    qrytype: item.qrytype || "",
                    alias: item.alias || item.dscrption,
                    chkper: Boolean(item.chkper)
                };
            })
        };

        try {
            const result = await service.savePermissions(
                payload,
                ui.getVerificationToken()
            );

            Notifications.Toastr.success("Updated", result.message);
            await loadPermissions();
        }
        catch (error) {
            Notifications.Toastr.error(
                "Update failed",
                ui.getErrorMessage(error, "Company permissions could not be updated.")
            );
        }
        finally {
            ui.setSavePending(false);
        }
    }

    return {
        init: init
    };
})();

document.addEventListener("DOMContentLoaded", CompanyPermPage.init);
