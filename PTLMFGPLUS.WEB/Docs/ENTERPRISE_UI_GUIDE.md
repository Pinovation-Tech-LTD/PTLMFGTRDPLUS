# PTL Manufacturing Plus Enterprise UI Guide

This guide is the required baseline for new ASP.NET Core MVC pages and migrated Web Forms pages. Reuse the shared classes and initialization helpers instead of creating page-specific copies.

## 1. Shared assets

`_AdminLayout.cshtml` loads the shared design files in this order:

1. `enterprise-components.css` — theme variables and common controls.
2. `interface-workspace.css` — workflow/interface dashboards.
3. `transaction-entry.css` — compact entry and transaction pages.
4. `admin-shell.css` — application shell, sidebar, top bar, and footer.

Do not import Bootstrap, Select2, Datepicker, DataTables, jQuery, or these styles again from an individual page.

Use these design tokens instead of hard-coded light colors:

```css
var(--enterprise-primary)
var(--enterprise-accent)
var(--enterprise-highlight)
var(--enterprise-surface)
var(--enterprise-surface-muted)
var(--enterprise-text)
var(--enterprise-text-muted)
var(--enterprise-border)
var(--enterprise-table-header)
var(--enterprise-table-footer)
var(--enterprise-table-hover)
```

## 2. Control sizes

Every input, select, and button in one row must use the same size class.

| Size | Class | Intended use |
|---|---|---|
| Extra small | `ui-size-xs` | Dense transaction grids and compact toolbars |
| Small / XM | `ui-size-xm` | Default application forms |
| Medium | `ui-size-md` | Important standalone controls |
| Large | `ui-size-lg` | Login or exceptional high-emphasis controls |

Example:

```html
<label for="txtDate" class="form-label">Date</label>
<input id="txtDate" class="form-control ui-size-xm" />
<button type="button" class="btn btn-primary ui-size-xm">Load</button>
```

Use `text-end` or `ui-number` on numeric values. All editable numeric inputs are globally right-aligned.

## 3. Compact entry page

Use the transaction layout for pages with header filters and editable native tables.

```html
<div class="enterprise-entry-page">
    <div class="card entry-header-card">
        <div class="card-body">
            <div class="entry-form-grid">
                <div class="entry-field entry-span-2">
                    <label for="txtDate" class="form-label">Date</label>
                    <input id="txtDate"
                           class="form-control ui-size-xm datepicker"
                           autocomplete="off" />
                </div>
                <div class="entry-field entry-span-3">
                    <label for="ddlItem" class="form-label">Item</label>
                    <select id="ddlItem"
                            class="form-select ui-size-xm"
                            data-placeholder="Select item">
                    </select>
                </div>
                <div class="entry-field-actions entry-span-1">
                    <button type="button" class="btn btn-primary ui-size-xm">Open</button>
                </div>
            </div>
        </div>
    </div>
</div>
```

The form grid has 12 desktop columns. Use `entry-span-1` through `entry-span-6`. It automatically changes to eight columns on smaller laptops, two on tablets, and one on phones.

Use `entry-panel`, `entry-section-card`, `entry-section-actions`, and `entry-split-grid` for the workspace below the header.

## 4. Select2

All selects except `.no-select2` are initialized automatically by `EnterpriseUI`.

```html
<select id="ddlCompany"
        class="form-select ui-size-xm"
        data-placeholder="Select company">
</select>
```

For dynamic data use the shared helper:

```javascript
Helpers.loadSelect2WithData({
    selector: "ddlCompany",
    data: companies,
    valueField: "comcod",
    textField: "companyName",
    placeholder: "Select company",
    selectedValue: selectedCompany
});
```

Do not manually set `dropdownParent` for normal modal controls. The global helper detects the nearest modal automatically. Pass `dropdownParent` only when a non-standard overlay requires an explicit container.

After dynamically adding raw select elements, call:

```javascript
EnterpriseUI.initializeSelect2(containerElement);
```

Never initialize the same Select2 twice.

## 5. Bootstrap Datepicker

Use the shared `datepicker` or `ui-datepicker-input` class. The required display format is `dd-MMM-yyyy`.

```html
<input id="txtFromDate"
       type="text"
       class="form-control ui-size-xm datepicker"
       autocomplete="off" />
```

For dynamically added fields call:

```javascript
EnterpriseUI.initializeDatepickers(containerElement);
```

Do not use browser `type="date"`, jQuery UI Datepicker, or a second date format.

## 6. Native editable tables

Use a native table when rows contain editable inputs, hierarchical data, calculated totals, or client-side working state.

```html
<div class="entry-native-table-shell">
    <table class="table table-striped table-hover table-bordered enterprise-native-table">
        <thead>
            <tr>
                <th>#</th>
                <th>Description</th>
                <th>Quantity</th>
            </tr>
        </thead>
        <tbody></tbody>
        <tfoot>
            <tr>
                <th colspan="2" class="text-end">Total</th>
                <th class="text-end">0</th>
            </tr>
        </tfoot>
    </table>
</div>
```

Use `description` or `wrap-cell` for long text. Use `text-end` for quantities, rates, and amounts. Keep table inputs `ui-size-xs`.

## 7. jQuery DataTables

Use DataTables for read-heavy lists that need paging, sorting, searching, or export. Default page length is 20, with 10, 15, 20, 30, 50, 100, and 300 options.

```javascript
const table = EnterpriseUI.createDataTable("#tblUsers", {
    order: [],
    autoWidth: false,
    stateSave: false,
    columnDefs: [
        {
            orderable: false,
            searchable: false,
            targets: [0, 5]
        }
    ],
    language: {
        emptyTable: "No records found.",
        search: "Search:"
    }
});
```

Before replacing table markup call:

```javascript
EnterpriseUI.destroyDataTable("#tblUsers");
```

Do not initialize a DataTable repeatedly. Preserve the current page after row updates when practical.

## 8. Interface/workflow pages

Use these global classes:

- `interface-workspace`
- `interface-toolbar`
- `interface-filter-group`
- `interface-kpi-grid`
- `interface-kpi-card`
- `interface-data-card`
- `interface-table`
- `interface-action-button`

The KPI grid supplies seven repeating colors automatically through `nth-child(7n + n)`. Do not add individual inline colors.

## 9. Modals

Use the standard Bootstrap structure. Global styles handle light/dark mode, scrolling, Select2, backdrop, header, and footer.

```html
<div class="modal fade" id="editModal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
        <div class="modal-content">
            <div class="modal-header">
                <h2 class="modal-title">Edit record</h2>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body"></div>
            <div class="modal-footer">
                <button type="button" class="btn btn-outline-secondary ui-size-xm" data-bs-dismiss="modal">Cancel</button>
                <button type="button" class="btn btn-primary ui-size-xm">Save</button>
            </div>
        </div>
    </div>
</div>
```

Disable Save while a request is pending. Close the modal only after a successful response.

## 10. Shared footer actions

Use the master footer for primary page-level Save, Delete, Approve, Refresh, or Cancel actions.

```javascript
ShowFooterWithButtons();
HideRefresh();
HideRecalculate();
HideCancel();

document.getElementById("btnFooterSave").addEventListener("click", savePage);
```

Keep modal-specific Save buttons inside their modal.

## 11. Theme and responsive rules

- Never hard-code white backgrounds or black text.
- Use enterprise variables for surfaces, text, borders, hover, and focus.
- Design desktop pages for a 1366×768 viewport with the sidebar open.
- Prefer compact toolbars and avoid duplicate page headings; the Admin layout already displays the page title.
- Wrap wide tables in `table-responsive` or `entry-native-table-shell`.
- Do not hide required actions on mobile. Stack or horizontally scroll them.
- Verify both `data-theme="light"` and `data-theme="dark"`.

## 12. JavaScript structure

Follow the project module format:

1. `state` stores page state.
2. `service` or `api` performs network calls.
3. `ui` performs DOM work.
4. Named workflow functions apply business behavior.
5. `bindEvents()` binds each handler once.
6. `init()` initializes shared controls and loads initial data.

Use shared `Helpers.withLoader`, `FetchHelpers`, `Notifications`, and `EnterpriseUI`. Keep statements, conditions, loops, and markup templates readable; do not minify page JavaScript.

## 13. Completion checklist

- Controls in each row use the same size.
- Labels align and use shared typography.
- Dates display as `dd-MMM-yyyy`.
- Numeric fields and cells are right-aligned.
- Select2 works inside normal and scrolling modals.
- Native tables and DataTables share header, footer, hover, and dark-mode colors.
- Empty, loading, success, validation, and failure states are clear.
- State-changing requests use the project anti-forgery convention.
- Server endpoints enforce company and page/action permissions.
- The page works at 1366×768 and on mobile.
- The affected project builds successfully.
