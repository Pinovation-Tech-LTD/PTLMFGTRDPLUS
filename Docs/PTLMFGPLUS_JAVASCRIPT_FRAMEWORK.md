# PTLMFGPLUS JavaScript Framework

**Status:** Required implementation standard  
**Applies to:** All ASP.NET Core MVC page scripts  
**Last reviewed:** 22-Oct-2026

## 1. Objective

Every PTLMFGPLUS page script must follow one predictable structure so developers can locate state, requests, rendering, validation, workflows, and initialization without relearning each page.

The standard structure is:

```text
Configuration
└── State
    ├── Service: network calls only
    ├── UI: DOM reads, writes, and rendering only
    ├── Validation: client usability rules
    ├── Workflows: coordinate state, service, and UI
    ├── Events: bind once
    └── Init: configure shared UI and load initial data
```

Business rules, authorization, company isolation, calculations, and final validation remain server responsibilities. JavaScript improves usability but is never the security boundary.

## 2. File and module naming

Page scripts belong in:

```text
wwwroot/js/J_{AreaName}/j{PageName}.js
```

Examples:

```text
wwwroot/js/J_07_RM/jPurMTReq.js
wwwroot/js/J_13_ProdMon/jProductionInterface.js
wwwroot/js/J_ControlPanel/user-permission.js
```

Use one public page module per page:

```javascript
var ProductionInterfacePage = (function () {
    // Private implementation.

    return {
        init: init
    };
})();

document.addEventListener(
    "DOMContentLoaded",
    ProductionInterfacePage.init
);
```

Do not expose state, service methods, DOM helpers, or internal workflows globally.

## 3. Required module order

Use this order in every page file:

1. Strict mode and route configuration.
2. Immutable configuration and selectors.
3. State.
4. Shared helper aliases.
5. Service.
6. UI.
7. Validation.
8. Workflow functions.
9. Event binding.
10. Initial loading.
11. Initialization.
12. Public return object and DOM-ready registration.

## 4. Canonical page template

```javascript
const commonPath = "/F_07_RM/MaterialTransfer/";

var MaterialTransferPage = (function () {
    "use strict";

    const selectors = {
        date: "#txtDate",
        project: "#ddlProject",
        table: "#tblMaterials",
        save: "#btnFooterSave"
    };

    const state = {
        documentId: null,
        lines: [],
        lookups: {},
        table: null,
        isLoading: false,
        isSaving: false
    };

    const service = {
        async loadLookups() {
            return Helpers.withLoader(function () {
                return FetchHelpers.getForm(
                    FetchHelpers.urlconfig(`${commonPath}GetLookups`)
                );
            });
        },

        async save(request) {
            return Helpers.withLoader(function () {
                return FetchHelpers.postJson(
                    FetchHelpers.urlconfig(`${commonPath}Save`),
                    request
                );
            });
        }
    };

    const ui = {
        getValue(selector) {
            return document.querySelector(selector)?.value?.trim() || "";
        },

        setValue(selector, value) {
            const element = document.querySelector(selector);

            if (element) {
                element.value = value ?? "";
            }
        },

        setSaving(isSaving) {
            state.isSaving = isSaving;

            const button = document.querySelector(selectors.save);

            if (button) {
                button.disabled = isSaving;
            }
        },

        renderLines(lines) {
            // DOM rendering only. Do not call the service here.
        }
    };

    const validation = {
        buildRequest() {
            return {
                documentId: state.documentId,
                date: ui.getValue(selectors.date),
                projectCode: ui.getValue(selectors.project),
                lines: state.lines
            };
        },

        validateRequest(request) {
            const errors = [];

            if (!request.date) {
                errors.push("Date is required.");
            }

            if (!request.projectCode) {
                errors.push("Project is required.");
            }

            if (!request.lines.length) {
                errors.push("Add at least one material line.");
            }

            return errors;
        }
    };

    async function saveDocument() {
        if (state.isSaving) {
            return;
        }

        const request = validation.buildRequest();
        const errors = validation.validateRequest(request);

        if (errors.length) {
            Notifications.Toastr.error(errors[0]);
            return;
        }

        ui.setSaving(true);

        try {
            const response = await service.save(request);

            state.documentId = response.documentId;
            Notifications.Toastr.success(response.message || "Saved successfully.");
        }
        catch (error) {
            Notifications.Toastr.error(
                error?.objError || "The document could not be saved."
            );
        }
        finally {
            ui.setSaving(false);
        }
    }

    function bindEvents() {
        document
            .querySelector(selectors.save)
            ?.addEventListener("click", saveDocument);
    }

    async function loadInitialData() {
        try {
            state.lookups = await service.loadLookups();
            // Render lookup data through ui methods.
        }
        catch (error) {
            Notifications.Toastr.error(
                error?.objError || "Initial data could not be loaded."
            );
        }
    }

    function configureFooter() {
        ShowFooterWithButtons();
        HideRecalculate();
        HideApprove();
    }

    function init() {
        bindEvents();
        configureFooter();
        EnterpriseUI.initialize(document);
        loadInitialData();
    }

    return {
        init: init
    };
})();

document.addEventListener(
    "DOMContentLoaded",
    MaterialTransferPage.init
);
```

## 5. State rules

State contains mutable page data only:

```javascript
const state = {
    selectedId: null,
    records: [],
    table: null,
    isLoading: false,
    isSaving: false
};
```

Rules:

- Keep state private inside the page module.
- Store identifiers and data, not DOM elements that can become stale.
- Use explicit pending flags to prevent duplicate requests.
- Never store the trusted company code, authenticated user ID, or permissions as client authority.
- Do not use mutable global variables.
- Do not duplicate server datasets in several unrelated variables.

## 6. Service rules

The service object owns network calls only.

```javascript
const service = {
    async load(id) {
        return Helpers.withLoader(function () {
            return FetchHelpers.getForm(
                FetchHelpers.urlconfig(`${commonPath}Get`),
                { id: id }
            );
        });
    }
};
```

Rules:

- Do not read or update the DOM in service methods.
- Use `FetchHelpers` and the existing anti-forgery convention.
- Use `Helpers.withLoader` for user-visible operations.
- Use server-side paging/search for large datasets.
- Return the server response to the workflow function.
- Do not silently convert a failed request into an empty successful result.
- Never build SQL, connection strings, company context, or authorization claims in JavaScript.

## 7. UI rules

The UI object owns DOM operations only:

- Read and set control values.
- Render tables, lists, badges, totals, and empty states.
- Show and hide page regions.
- Configure Select2, Datepicker, DataTables, and modals.
- Enable or disable actions.
- Move focus after an important transition.

UI methods do not call controller endpoints and do not decide business rules.

Use `textContent` for plain server-provided values. When template strings are required, encode untrusted values through an existing shared encoder before inserting HTML.

## 8. Shared DataTables column factories

Use `EnterpriseUI.dataTableColumns` instead of redefining common columns on each page.

```javascript
const {
    textColumn,
    wrappedTextColumn,
    numericColumn,
    dateColumn,
    rowNumberColumn
} = EnterpriseUI.dataTableColumns;

const columns = [
    rowNumberColumn(),
    textColumn("pbmno1"),
    dateColumn("bgddat"),
    wrappedTextColumn("prodesc"),
    numericColumn("itemcount"),
    numericColumn("bgdwqty"),
    numericColumn("bgdamt"),
    dateColumn("sdate"),
    dateColumn("enddate"),
    textColumn("pbmststus"),
    wrappedTextColumn("actdesc")
];
```

Factories provide:

| Factory | Behavior |
|---|---|
| `textColumn(field)` | Plain data column with safe empty default |
| `wrappedTextColumn(field)` | Long description column with shared wrapping class |
| `numericColumn(field)` | Shared numeric class and right alignment |
| `dateColumn(field)` | `dd-MMM-yyyy` display while preserving raw sort value |
| `rowNumberColumn()` | Correct row number across paginated pages |

Every factory accepts an optional DataTables override object:

```javascript
numericColumn("amount", {
    name: "amount",
    searchable: false
});

textColumn("status", {
    render: renderStatusBadge
});
```

Page-specific action columns remain local because their permissions and workflows differ.

Use the shared table initializer:

```javascript
state.table = EnterpriseUI.createDataTable("#tblRecords", {
    enterpriseTitle: "Material requests",
    data: state.records,
    columns: columns,
    order: []
});
```

Do not reimplement search placement, scrolling, pagination, page length, theme styling, or title behavior.

## 9. Workflow rules

Workflow functions coordinate service, validation, state, and UI:

```text
User event
→ validate/build request
→ set pending state
→ service call
→ update state
→ render UI
→ notify user
→ clear pending state
```

Use `try/catch/finally` for every workflow that changes a pending state.

Do not put a complete workflow inside an event callback. Call a named function so it can be understood and tested independently.

## 10. Event rules

- Bind events once inside `bindEvents()`.
- Prefer native `addEventListener` for fixed elements.
- Use delegated jQuery events for dynamic table rows where the project already relies on jQuery.
- Namespace document-level jQuery events when a page can reinitialize them.
- Never bind events inside render loops.
- Do not use inline `onclick` in new page markup.
- Prevent duplicate submission through state and disabled buttons.

Example for a dynamic table:

```javascript
$(selectors.table).on(
    "click",
    ".row-action[data-action='edit']",
    function () {
        editRecord(this);
    }
);
```

## 11. Validation rules

- Client validation exists for usability; repeat all business validation on the server.
- Build an explicit request object. Do not post the entire DOM or database entity.
- Validate required fields, ranges, dates, and line presence before sending.
- Server calculations remain authoritative for quantities, rates, amounts, stock, and financial totals.
- Keep entered values after validation failure.
- Focus the first invalid control when practical.
- Use `dd-MMM-yyyy` display and the existing server business-date interpretation.

## 12. Async and feedback rules

- Use `async/await`; do not mix callbacks and promises in the same workflow.
- Await calls that affect the next UI state.
- Use clear fallback messages while preferring the server's safe business message.
- Disable only the initiating action or dependent region when practical.
- Routine success uses a toast.
- Correctable validation stays inline or near the control.
- Destructive operations use the shared confirmation pattern.
- Do not expose stack traces, exception text, SQL, or infrastructure details.

## 13. Select2, dates, and modals

- Populate lookups through `Helpers.loadSelect2WithData`.
- Initialize dynamic controls through `EnterpriseUI.initializeSelect2(container)`.
- Use `.datepicker` and the shared `dd-MMM-yyyy` configuration.
- Do not initialize Select2 or Datepicker twice.
- Modal Save is disabled while pending.
- Close a modal only after a successful save.
- Reset modal validation deliberately when opening a new record.

## 14. Shared footer actions

Configure the shared footer during initialization:

```javascript
function configureFooter() {
    ShowFooterWithButtons();
    HideRefresh();
    HideRecalculate();
    HideApprove();
}
```

Bind the visible actions in `bindEvents()`. Page-level Save, Approve, Delete, Recalculate, Refresh, and Cancel belong in the shared footer. Modal-specific actions remain in the modal.

## 15. Naming conventions

| Item | Convention | Example |
|---|---|---|
| Page module | PascalCase + `Page` | `ProductionInterfacePage` |
| State/property/function | camelCase | `selectedInterface` |
| DOM ID | Existing project convention | `txtFromDate`, `ddlProject` |
| Constant route | `commonPath` | `/F_13_ProdMon/ProductionInterface/` |
| Loader | `load...` | `loadInitialData` |
| Renderer | `render...` | `renderLines` |
| Event setup | `bindEvents` | One per module |
| Request builder | `build...Request` | `buildSaveRequest` |
| Validator | `validate...` | `validateSaveRequest` |
| Workflow | Business verb | `saveBudget`, `approveRequest` |

Use business language. Avoid names such as `doStuff`, `data1`, `temp`, `test`, or `clickFunction`.

## 16. Formatting rules

- Do not minify source files.
- Put braces and nested statements on readable lines.
- Avoid multiple unrelated statements on one line.
- Use early returns to reduce nesting.
- Keep template markup readable with one element per line.
- Prefer `const`; use `let` only when reassignment is required.
- Do not use `var` except where matching the established public module declaration.
- Keep functions focused. Split functions that perform requests, calculations, and DOM rendering together.

## 17. Prohibited patterns

Do not:

- Call AJAX directly from UI rendering functions.
- Trust posted company code, user ID, permission, totals, or record status.
- Store mutable user data in global variables.
- Initialize DataTables, Select2, or Datepicker repeatedly.
- Attach event handlers inside render loops.
- Concatenate untrusted values into HTML without encoding.
- Swallow errors and display an empty table as if loading succeeded.
- Use `setTimeout` to solve sequencing that should use `await`.
- Duplicate shared date, numeric, DataTable, notification, loader, or modal behavior.
- Change Web Forms business behavior silently during migration.

## 18. Page completion checklist

- [ ] Script is in the correct area folder.
- [ ] One private page module exposes only `init`.
- [ ] State, service, UI, validation, workflows, events, and initialization are separated.
- [ ] Network calls use `FetchHelpers` and shared loader behavior.
- [ ] Shared DataTables column factories are used.
- [ ] Dates display as `dd-MMM-yyyy`.
- [ ] Numeric values are right-aligned.
- [ ] Events are bound once.
- [ ] Pending actions prevent double submission.
- [ ] Empty, loading, validation, failure, and success states are implemented.
- [ ] Server authorization, company isolation, validation, and anti-forgery remain enforced.
- [ ] JavaScript syntax validation passes.
- [ ] The affected project builds.
- [ ] Relevant behavior is compared with Web Forms.
