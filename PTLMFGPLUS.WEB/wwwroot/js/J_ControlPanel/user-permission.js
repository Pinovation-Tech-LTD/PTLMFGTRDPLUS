const userPermissionPath = "/ControlPanel/UserPerm/";

var UserPermissionPage = (function () {
    "use strict";

    var state = {
        users: [],
        employees: [],
        roles: [],
        modules: [],
        permissions: [],
        selectedUser: null,
        copyFromUser: null,
        userTable: null,
        permissionTable: null
    };

    var service = {
        async loadInitialData(search) {
            return Helpers.withLoader(() => FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${userPermissionPath}GetInitialData`),
                { search: search || "" }
            ));
        },

        async loadUsers(search) {
            return Helpers.withLoader(() => FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${userPermissionPath}GetUsers`),
                { search: search || "" }
            ));
        },

        async loadPermissions(userId, moduleId, showAll) {
            return Helpers.withLoader(() => FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${userPermissionPath}GetPermissions`),
                {
                    userId: userId,
                    moduleId: moduleId,
                    showAll: showAll
                }
            ));
        },

        async post(action, payload, verificationToken) {
            const response = await fetch(FetchHelpers.urlconfig(`${userPermissionPath}${action}`), {
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
            return element ? element.value.trim() : "";
        },

        setValue(id, value) {
            const element = document.getElementById(id);

            if (element) {
                element.value = value ?? "";
            }
        },

        isChecked(id) {
            const element = document.getElementById(id);
            return element ? element.checked : false;
        },

        setChecked(id, checked) {
            const element = document.getElementById(id);

            if (element) {
                element.checked = Boolean(checked);
            }
        },

        setPending(id, pending) {
            const button = document.getElementById(id);

            if (button) {
                button.disabled = pending;
            }
        },

        getVerificationToken() {
            const token = document.querySelector('input[name="__RequestVerificationToken"]');
            return token ? token.value : "";
        },

        encode(value) {
            return $("<div>").text(value ?? "").html();
        },

        errorMessage(error, fallback) {
            return error?.objError || error?.message || fallback;
        },

        loadSelect(selector, items, valueField, textField, placeholder, selectedValue) {
            Helpers.loadSelect2WithData({
                selector: selector,
                data: items,
                valueField: valueField,
                textField: textField,
                placeholder: placeholder,
                selectedValue: selectedValue
            });
        },

        renderLookups() {
            ui.loadSelect("ddlEmployee", state.employees, "empid", "empname", "Select employee", "");
            ui.loadSelect("ddlUserRole", state.roles, "roleid", "roledesc", "Select role", "");
            ui.loadSelect("ddlPermissionModule", state.modules, "moduleid", "modulename", "Select module", "AA");
        },

        renderUsers() {
            EnterpriseUI.destroyDataTable("#tblUsers");
            state.userTable = null;

            const tbody = document.querySelector("#tblUsers tbody");

            tbody.innerHTML = state.users.map(function (user, index) {
                return `
                    <tr>
                        <td class="text-end">
                            ${index + 1}.
                        </td>
                        <td>
                            <button type="button" class="btn btn-link user-permission-link p-0" data-index="${index}">
                                ${ui.encode(user.usrid)}
                            </button>
                            <small>
                                ${ui.encode(user.usrsname)}
                            </small>
                        </td>
                        <td>
                            ${ui.encode(user.usrname)}
                        </td>
                        <td>
                            ${ui.encode(user.usrdesig)}
                        </td>
                        <td>
                            ${ui.encode(user.empname)}
                        </td>
                        <td>
                            ${ui.encode(user.roledesc)}
                        </td>
                        <td>
                            <span class="status-badge ${user.usractive ? "active" : "inactive"}">
                                ${user.usractive ? "Active" : "Inactive"}
                            </span>
                        </td>
                        <td>
                            <div class="row-actions">
                                <button type="button" class="btn btn-outline-primary btn-sm edit-user" data-index="${index}" title="Edit user">
                                    <i class="bi bi-pencil"></i>
                                </button>
                                <button type="button" class="btn btn-outline-secondary btn-sm copy-user" data-index="${index}" title="Copy privileges">
                                    <i class="bi bi-copy"></i>
                                </button>
                            </div>
                        </td>
                    </tr>
                `;
            }).join("");

            state.userTable = EnterpriseUI.createDataTable("#tblUsers", {
                order: [],
                stateSave: false,
                autoWidth: false,
                columnDefs: [
                    {
                        orderable: false,
                        searchable: false,
                        targets: [0, 7]
                    }
                ],
                language: {
                    emptyTable: "No users match the current search.",
                    search: "Filter users:"
                }
            });
            document.getElementById("userCount").textContent = state.users.length;
        },

        showUserModal(user) {
            const isEditing = Boolean(user);

            ui.setValue("txtUserId", user?.usrid || "");
            ui.setValue("txtShortName", user?.usrsname || "");
            ui.setValue("txtFullName", user?.usrname || "");
            ui.setValue("txtDesignation", user?.usrdesig || "");
            ui.setValue("txtPassword", "");
            ui.setValue("txtRemarks", user?.usrrmrk || "");
            ui.setChecked("chkUserActive", isEditing ? user.usractive : true);
            $("#ddlEmployee").val(user?.empid || "").trigger("change.select2");
            $("#ddlUserRole").val(user?.urole || "").trigger("change.select2");
            document.getElementById("txtUserId").readOnly = isEditing;
            document.getElementById("userEditModalLabel").textContent = isEditing ? "Edit user" : "New user";
            bootstrap.Modal.getOrCreateInstance(document.getElementById("userEditModal")).show();
        },

        showCopyModal(user) {
            const targets = state.users.filter(item => item.usrid !== user.usrid);

            state.copyFromUser = user;
            document.getElementById("copyFromUserName").textContent = `${user.usrname} (${user.usrid})`;
            ui.loadSelect("ddlCopyToUser", targets, "usrid", "usrname", "Select target user", "");
            bootstrap.Modal.getOrCreateInstance(document.getElementById("copyPermissionModal")).show();
        },

        showPermissionPanel(user) {
            state.selectedUser = user;
            document.getElementById("selectedUserName").textContent = user.usrname;
            document.getElementById("selectedUserId").textContent = user.usrid;
            document.getElementById("userListPanel").classList.add("d-none");
            document.getElementById("permissionPanel").classList.remove("d-none");
            ShowFooterWithButtons();
        },

        showUserList() {
            state.selectedUser = null;
            state.permissions = [];
            document.getElementById("permissionPanel").classList.add("d-none");
            document.getElementById("userListPanel").classList.remove("d-none");
            HideFooterWithButtons();
        },

        syncVisiblePermissions() {
            document.querySelectorAll("#tblUserPermissions tbody tr[data-index]").forEach(function (row) {
                const permission = state.permissions[Number(row.dataset.index)];

                if (!permission) {
                    return;
                }

                ["chkper", "entry", "deleteck", "printable"].forEach(function (field) {
                    const checkbox = row.querySelector(`[data-field="${field}"]`);
                    permission[field] = Boolean(checkbox?.checked);
                });
            });
        },

        updatePermissionSummary() {
            ui.syncVisiblePermissions();
            document.getElementById("pageCount").textContent = state.permissions.length;
            document.getElementById("permittedCount").textContent = state.permissions.filter(item => item.chkper).length;

            document.querySelectorAll(".bulk-permission").forEach(function (checkbox) {
                const field = checkbox.dataset.field;
                const allSelected = state.permissions.length > 0 && state.permissions.every(item => item[field]);
                const someSelected = state.permissions.some(item => item[field]);

                checkbox.checked = allSelected;
                checkbox.indeterminate = someSelected && !allSelected;
            });

            const allActions = document.getElementById("chkAllActions");
            const everyAction = state.permissions.length > 0 && state.permissions.every(function (item) {
                return item.chkper && item.entry && item.deleteck && item.printable;
            });
            const someAction = state.permissions.some(function (item) {
                return item.chkper || item.entry || item.deleteck || item.printable;
            });

            allActions.checked = everyAction;
            allActions.indeterminate = someAction && !everyAction;
        },

        renderPermissions() {
            EnterpriseUI.destroyDataTable("#tblUserPermissions");
            state.permissionTable = null;

            const tbody = document.querySelector("#tblUserPermissions tbody");

            tbody.innerHTML = state.permissions.map(function (item, index) {
                return `
                    <tr data-index="${index}">
                        <td class="text-end">
                            ${index + 1}.
                        </td>
                        <td>
                            <strong>
                                ${ui.encode(item.modulename)}
                            </strong>
                            <small>
                                ${ui.encode(item.frmdesc)}
                            </small>
                        </td>
                        <td>
                            ${ui.encode(item.dscrption)}
                        </td>
                        ${ui.permissionCheckbox("chkper", item.chkper)}
                        ${ui.permissionCheckbox("entry", item.entry)}
                        ${ui.permissionCheckbox("deleteck", item.deleteck)}
                        ${ui.permissionCheckbox("printable", item.printable)}
                        <td class="text-center">
                            <input type="checkbox"
                                   class="form-check-input row-all-permissions"
                                   ${item.chkper && item.entry && item.deleteck && item.printable ? "checked" : ""}
                                   aria-label="Select all row permissions" />
                        </td>
                    </tr>
                `;
            }).join("");

            state.permissionTable = EnterpriseUI.createDataTable("#tblUserPermissions", {
                order: [],
                stateSave: false,
                autoWidth: false,
                columnDefs: [
                    {
                        orderable: false,
                        searchable: false,
                        targets: [0, 3, 4, 5, 6, 7]
                    }
                ],
                language: {
                    emptyTable: "No permissions are available for this selection.",
                    search: "Search pages:"
                }
            });
            ui.updatePermissionSummary();
        },

        permissionCheckbox(field, checked) {
            return `
                <td class="text-center">
                    <input type="checkbox"
                           class="form-check-input permission-checkbox"
                           data-field="${field}"
                           ${checked ? "checked" : ""}
                           aria-label="${field} permission" />
                </td>
            `;
        }
    };

    function init() {
        configureFooter();
        bindEvents();
        loadInitialData();
    }

    function configureFooter() {
        HideFooterWithButtons();
        HideRefresh();
        HideRecalculate();
        HideCancel();
        HideApprove();
    }

    function bindElementEvent(id, eventName, handler) {
        const element = document.getElementById(id);

        if (element) {
            element.addEventListener(eventName, handler);
        }
    }

    function bindEvents() {
        bindElementEvent("btnSearchUser", "click", loadUsers);
        bindElementEvent("txtUserSearch", "keydown", function (event) {
            if (event.key === "Enter") {
                event.preventDefault();
                loadUsers();
            }
        });
        bindElementEvent("btnAddUser", "click", function () {
            ui.showUserModal(null);
        });
        bindElementEvent("btnBackToUsers", "click", ui.showUserList);
        bindElementEvent("btnSaveUser", "click", saveUser);
        bindElementEvent("btnFooterSave", "click", savePermissions);
        bindElementEvent("btnCopyPermissions", "click", copyPermissions);
        $("#ddlPermissionModule").on("change", loadPermissions);
        bindElementEvent("chkShowAllPages", "change", loadPermissions);

        $("#tblUsers tbody").on("click", ".user-permission-link", function () {
            openPermissions(Number(this.dataset.index));
        });
        $("#tblUsers tbody").on("click", ".edit-user", function () {
            ui.showUserModal(state.users[Number(this.dataset.index)]);
        });
        $("#tblUsers tbody").on("click", ".copy-user", function () {
            ui.showCopyModal(state.users[Number(this.dataset.index)]);
        });
        $("#tblUserPermissions tbody").on("change", ".permission-checkbox", ui.updatePermissionSummary);
        $("#tblUserPermissions tbody").on("change", ".row-all-permissions", function () {
            const row = this.closest("tr");
            const permission = state.permissions[Number(row.dataset.index)];
            const checked = this.checked;

            permission.chkper = checked;
            permission.entry = checked;
            permission.deleteck = checked;
            permission.printable = checked;
            ui.renderPermissions();
        });
        document.querySelectorAll(".bulk-permission").forEach(function (checkbox) {
            checkbox.addEventListener("change", function () {
                const field = this.dataset.field;
                const checked = this.checked;

                state.permissions.forEach(function (item) {
                    item[field] = checked;
                });
                ui.renderPermissions();
            });
        });
        bindElementEvent("chkAllActions", "change", function () {
            const checked = this.checked;

            state.permissions.forEach(function (item) {
                item.chkper = checked;
                item.entry = checked;
                item.deleteck = checked;
                item.printable = checked;
            });
            ui.renderPermissions();
        });
    }

    async function loadInitialData() {
        try {
            const result = await service.loadInitialData("");

            state.users = result.users || [];
            state.employees = result.employees || [];
            state.roles = result.roles || [];
            state.modules = result.modules || [];
            ui.renderLookups();
            ui.renderUsers();
        }
        catch (error) {
            Notifications.Toastr.error("Load failed", ui.errorMessage(error, "User permission data could not be loaded."));
        }
    }

    async function loadUsers() {
        try {
            state.users = await service.loadUsers(ui.getValue("txtUserSearch")) || [];
            ui.renderUsers();
        }
        catch (error) {
            Notifications.Toastr.error("Search failed", ui.errorMessage(error, "Users could not be loaded."));
        }
    }

    async function openPermissions(index) {
        const user = state.users[index];

        if (!user) {
            return;
        }

        ui.showPermissionPanel(user);
        $("#ddlPermissionModule").val("AA").trigger("change.select2");
        ui.setChecked("chkShowAllPages", false);
        await loadPermissions();
    }

    async function loadPermissions() {
        if (!state.selectedUser) {
            return;
        }

        try {
            state.permissions = await service.loadPermissions(
                state.selectedUser.usrid,
                ui.getValue("ddlPermissionModule") || "AA",
                ui.isChecked("chkShowAllPages")
            ) || [];
            ui.renderPermissions();
        }
        catch (error) {
            state.permissions = [];
            ui.renderPermissions();
            Notifications.Toastr.error("Load failed", ui.errorMessage(error, "User permissions could not be loaded."));
        }
    }

    async function saveUser() {
        const payload = {
            usrid: ui.getValue("txtUserId"),
            usrsname: ui.getValue("txtShortName"),
            usrname: ui.getValue("txtFullName"),
            usrdesig: ui.getValue("txtDesignation"),
            password: ui.getValue("txtPassword"),
            usrrmrk: ui.getValue("txtRemarks"),
            active: ui.isChecked("chkUserActive"),
            empid: ui.getValue("ddlEmployee"),
            roleid: ui.getValue("ddlUserRole")
        };

        if (!payload.usrid || !payload.usrsname || !payload.usrname) {
            Notifications.Toastr.warning("Required fields", "User ID, short name and full name are required.");
            return;
        }

        ui.setPending("btnSaveUser", true);
        try {
            const result = await service.post("SaveUser", payload, ui.getVerificationToken());

            bootstrap.Modal.getInstance(document.getElementById("userEditModal"))?.hide();
            Notifications.Toastr.success("Updated", result.message);
            await loadUsers();
        }
        catch (error) {
            Notifications.Toastr.error("Update failed", ui.errorMessage(error, "User information could not be updated."));
        }
        finally {
            ui.setPending("btnSaveUser", false);
        }
    }

    async function savePermissions() {
        ui.syncVisiblePermissions();
        ui.setPending("btnFooterSave", true);

        const payload = {
            userId: state.selectedUser.usrid,
            moduleId: ui.getValue("ddlPermissionModule") || "AA",
            permissions: state.permissions
        };

        try {
            const result = await service.post("SavePermissions", payload, ui.getVerificationToken());

            Notifications.Toastr.success("Updated", result.message);
            await loadPermissions();
        }
        catch (error) {
            Notifications.Toastr.error("Update failed", ui.errorMessage(error, "User permissions could not be updated."));
        }
        finally {
            ui.setPending("btnFooterSave", false);
        }
    }

    async function copyPermissions() {
        const targetUserId = ui.getValue("ddlCopyToUser");

        if (!state.copyFromUser || !targetUserId) {
            Notifications.Toastr.warning("Target required", "Select a target user.");
            return;
        }

        ui.setPending("btnCopyPermissions", true);
        try {
            const result = await service.post("CopyPermissions", {
                fromUserId: state.copyFromUser.usrid,
                toUserId: targetUserId
            }, ui.getVerificationToken());

            bootstrap.Modal.getInstance(document.getElementById("copyPermissionModal"))?.hide();
            Notifications.Toastr.success("Copied", result.message);
        }
        catch (error) {
            Notifications.Toastr.error("Copy failed", ui.errorMessage(error, "User privileges could not be copied."));
        }
        finally {
            ui.setPending("btnCopyPermissions", false);
        }
    }

    return {
        init: init
    };
})();

document.addEventListener("DOMContentLoaded", UserPermissionPage.init);
