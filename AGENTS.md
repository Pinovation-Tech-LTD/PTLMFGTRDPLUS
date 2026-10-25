## Migration Rules

- Treat PTLMFGERP Web Forms as the reference for business rules,
  calculations, validations, permissions, and workflow.
- Treat the existing ASP.NET Core MVC project as the reference for
  architecture, naming, dependency injection, UI, and coding style.
- Before migrating a page, inspect its ASPX, code-behind, related
  JavaScript, stored procedure CallTypes, and report definitions.
- Identify all page actions, including load, search, save, update,
  delete, approval, print, and export.
- Preserve existing behavior unless a change is explicitly requested.
- Do not copy Web Forms lifecycle, ViewState, or control-event patterns
  into MVC.
- If source behavior is unclear or appears incorrect, explain the
  issue instead of silently changing the business rule.
- Complete the full path from View and JS through Controller,
  Service, Data, and stored procedure mapping.
- Linebreak after each tag first tag in .cshtml page. dont write whole table in 1 line. break between each tag
- Same goes for js, dont write in 1 line. break between brackets, loop statements, make it readible, not minify.
- Make sure all Number field should be right aligned.
- All date format should be dd-MMM-yyyy imposed in every apges. Use Bootstrap Datepicker.
- Use reuseable Footer button for Save Refresh Calculate/Recalculate Close.
- Follow js structure as follow with folder js/J\_{area_name}/jsfilename.js:
  const commonPath = "/controlpanel/resourceperm/";
  var CompanyPermPage = (function () {
  var state = {
  selectedModuleId=0,
  pages: [],
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
          HideRecalculate();
      }
      function bindEvents() {
          //$('#ddlModule').on('change', function (e) {
          //    pageDataModuleWise(e);
          //});
          $("#btnOk").on("click", function () {
              let moduleId = ui.getValue("ddlModule");
              pageDataModuleWise(moduleId);
          });
          const btnFooterTotal = document.querySelector("#btnFooterSave")
          btnFooterTotal.addEventListener("click", function () {

          });
      }
      async function loadInitialData() {
          const moduleItems = await service.loaddropdown();
          ui.renderDropdown(moduleItems);
          const data = await service.loadAllPages();
          state.pages = data;
          ui.renderTable(data);
      }

      async function pageDataModuleWise(e) {
          const moduleId = e;//.target.value;
          state.selectedModuleId = moduleId;
          const pages = await service.loadPagesModuleWise(moduleId);
          ui.renderTable(pages);
      }

      return {
          init: init
      };

  })();

document.addEventListener('DOMContentLoaded', CompanyPermPage.init);

## Stored Procedures and Data Mapping

- Reuse existing stored procedure names and CallTypes.
- Preserve parameter order, parameter meaning, null handling,
  and expected result-set order.
- Use parameterized database calls. Never concatenate user input
  into SQL.
- Map result sets to typed response models where practical.
- Use a response class containing multiple typed lists when an
  operation returns multiple result sets.
- Use DataTable/DataSet only when required by an existing integration,
  dynamic result shape, or reporting component.
- Handle DBNull and nullable database fields explicitly.
- Preserve decimal precision for quantities, rates, and amounts.
- Do not replace existing transaction boundaries without reviewing
  their effect on the complete operation.

## Authentication and Company Isolation

- Obtain COMCOD, user ID, and permissions from the existing trusted
  server-side authentication/company context.
- Do not trust posted COMCOD or user ID as proof of authorization.
- Apply company filtering to reads, writes, lookups, and exports.
- Validate that requested records belong to the authorized company.
- Enforce permissions on controller endpoints, including AJAX actions.
- Follow the existing anti-forgery convention for state-changing requests.
- Never expose connection strings, credentials, or exception details
  to the browser.

## Models and Validation

- Follow existing conventions for Entity, request, response,
  and ViewModel classes.
- Do not bind database entities directly when that would allow users
  to edit protected fields.
- Validate required fields and business rules on the server.
- Use client-side validation for usability, not as the only validation.
- Preserve existing date formats and business-date interpretation.
- Preserve existing rounding rules and calculation order.

## State Management

- Replace ViewState with explicit request models and client-side state.
- Do not store large report datasets in Session by default.
- Follow the existing cache strategy when caching is necessary.
- Include company and relevant user/filter information in cache keys.
- Do not keep user-specific mutable data in static fields.
- Keep filters and selected record IDs explicit and easy to trace.

## Shared Frontend Components

- Reuse shared Loader, AJAX/Fetch, URL configuration, notification,
  confirmation, and error-handling functions.
- Follow the existing state, service, and UI JavaScript structure.
- Keep network calls in the service layer and DOM operations
  in the UI layer.
- Use shared initialization functions for DataTables and Select2.
- DataTables default page length: 20 rows.
- DataTables page-length options: 10, 15, 20, 30, 50, 100, 300.
- Preserve the current page after a row update when practical.
- Avoid duplicate DataTable, Select2, and event-handler initialization.
- Initialize dynamically added selects through the shared Select2 helper.
- For Select2 inside a modal, set the appropriate dropdown parent.
- Follow one shared Bootstrap modal layout for headings, labels,
  spacing, validation messages, Save, and Cancel buttons.
- Disable Save while a request is pending and restore it afterward.
- Close the modal only after a successful save.
- Show clear loading, empty, success, and error states.
- Make sure all Number field should be right aligned.
- All date format should be dd-MMM-yyyy imposed in every apges. Use Bootstrap Datepicker.

## Performance

- Avoid loading all records for large transaction lists.
- Use the existing server-side paging/filtering pattern where supported.
- Do not add a database call for every row.
- Avoid repeated requests for unchanged lookup data.
- Use asynchronous I/O consistently where supported by the project.
- Do not use Task.Run to wrap synchronous database calls.
- Keep browser payloads small, especially for large grids and reports.

## Reports and Exports

- Preserve report filters, columns, totals, grouping, and calculations.
- Preserve print and export functionality when migrating a page.
- Ensure exports apply the same permissions and filters as the screen.
- Inspect the existing Core reporting solution before choosing a
  replacement for a Web Forms reporting component.

## Change Scope and Dependencies

- Make focused changes needed for the requested feature.
- Do not refactor unrelated modules or reformat unrelated files.
- Reuse existing packages and helpers before adding dependencies.
- Do not edit generated files or commit secrets.
- Do not modify the Web Forms reference project unless requested.

## Validation and Completion

- Build the affected project after changes.
- Run relevant existing tests when available.
- Verify migrated actions against the Web Forms behavior.
- Check empty results, invalid input, failed saves, and permission denial.
- For financial or stock operations, verify totals and calculations
  using representative examples.
- Report what was migrated, what was verified, and any remaining gaps.
- If the build or runtime verification cannot be completed, state why.

# PTLMFGPLUS Enterprise UI/UX Standard : IMPORTANT AND MUST FOLLOW

**Status:** Implementation standard  
**Applies to:** PTLMFGPLUS ASP.NET Core MVC pages  
**Last reviewed:** 22-Oct-2026  
**Primary viewport:** 1366 x 768 desktop, with responsive support down to 360 px

## 1. Purpose

This document defines how PTLMFGPLUS pages must look, behave, and be implemented. It is a product-specific standard, not a visual theme. Its goal is to make every manufacturing, inventory, production, finance, reporting, and administration workflow feel like one coherent enterprise application.

The Web Forms system remains the source of truth for business behavior. The MVC project remains the source of truth for architecture. This UI standard governs how that behavior is presented in MVC without changing calculations, validations, permissions, workflow, stored procedures, or report results.

## 2. Current-state evaluation

### 2.1 What is already strong

- `_AdminLayout.cshtml` provides a modern application shell with navigation, page title, report output, profile, theme, content, and footer regions.
- `enterprise-components.css` already defines shared light/dark design tokens, control sizes, focus states, tables, Select2, Datepicker, and numeric alignment.
- `enterprise-ui.js` centralizes Datepicker, Select2, and DataTables initialization.
- Transaction, interface, dashboard, permission, and production pages already have reusable CSS foundations.
- Shared footer actions exist for Save, Refresh, Recalculate, Approve, Delete, Save and New, and Cancel.
- The codebase already uses Bootstrap, Bootstrap Icons, Select2, DataTables, Bootstrap Datepicker, SweetAlert2, toastr, shared fetch helpers, and loader helpers.

### 2.2 Problems that prevent a fully enterprise experience

| Area                    | Finding                                                                                                                                       | Impact                                                                    | Priority |
| ----------------------- | --------------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- | -------- |
| Visual consistency      | Pages mix cards, shadows, corner radii, label weights, icon colors, and control densities                                                     | Users must relearn the visual hierarchy on each page                      | P0       |
| Page CSS                | Several Razor pages contain inline styles or large `<style>` blocks                                                                           | Dark mode, responsive behavior, and maintenance diverge                   | P0       |
| Actions                 | Primary actions appear in page cards, table toolbars, modals, and the fixed footer without one rule                                           | Save/approve/delete workflows become unpredictable                        | P0       |
| Layouts                 | `_AdminLayout`, `_MainLayout`, and `_Layout` coexist; `_MainLayout` still contains starter-theme branding and duplicate plugin initialization | Future pages can accidentally inherit the wrong shell                     | P1       |
| Framework compatibility | The admin shell uses Bootstrap 5 while DataTables Bootstrap 4 adapters/styles are loaded                                                      | Subtle spacing, dropdown, and responsive inconsistencies can occur        | P1       |
| Tables                  | Some pages use DataTables, some native tables, and some fixed-width tables without a clear selection rule                                     | Filtering, empty states, totals, alignment, and mobile behavior differ    | P0       |
| Form density            | `form-control-sm`, custom classes, default controls, and page-specific heights are mixed                                                      | Rows do not align and scanning becomes slower                             | P0       |
| Dates                   | Shared Datepicker is correct, but some pages initialize dates again locally                                                                   | Double initialization and format drift are possible                       | P0       |
| Navigation              | Dynamic admin navigation exists, while a 500+ line legacy sidebar remains in the project                                                      | Two competing navigation models increase migration risk                   | P1       |
| Accessibility           | Some icon buttons need explicit names; validation, focus order, keyboard behavior, and live status messaging are not consistently documented  | Keyboard and assistive-technology users receive an uneven experience      | P0       |
| Feedback states         | Loading, empty, validation, no-permission, failure, and retry states are not expressed uniformly                                              | Users cannot reliably distinguish slow, empty, invalid, and failed states | P0       |
| Responsive behavior     | Some pages rely on fixed column widths or page-specific CSS                                                                                   | Laptop and mobile layouts can clip actions or data                        | P1       |

### 2.3 UX maturity assessment

The project has a **good component foundation but inconsistent page adoption**. Treat the shared shell, tokens, and JavaScript helpers as the beginning of the design system. Do not start another visual redesign. The fastest path to enterprise quality is consolidation, documented page patterns, and systematic migration.

## 3. Enterprise ERP principles for PTLMFGPLUS

The design takes inspiration from proven ERP patterns without copying another product's branding:

1. **Role and task first (SAP Fiori).** Show the information and actions needed for the user's current business task. Do not expose every possible option at once.
2. **Predictable workspaces (Dynamics 365).** Use stable page patterns: filters, status/KPI summary, working list or form, and related actions.
3. **Calm consistency (Oracle Redwood).** Prefer clear hierarchy, restrained color, reusable components, and progressive modernization over decorative variation.
4. **Fast list-to-record work (Odoo).** Make search, filter, list, select, edit, save, and return-to-list transitions efficient.
5. **Accessible by construction (Microsoft Fluent).** Semantic markup, keyboard operation, visible focus, meaningful labels, and WCAG 2.1 AA contrast are default requirements.
6. **Manufacturing accuracy over decoration.** Quantities, rates, stock balances, statuses, and totals must be easy to compare. Animation and color must never compete with operational data.
7. **Preserve expert speed.** Dense ERP pages may be compact, but never cramped. Frequent users must be able to tab through a form, operate lookup controls, and complete primary workflows without a mouse.

Reference material:

- SAP Fiori Design Principles: https://experience.sap.com/fiori-design-web/design-principles/
- Microsoft Dynamics 365 navigation and workspace model: https://learn.microsoft.com/en-us/dynamics365/fin-ops-core/dev-itpro/user-interface/page-navigation
- Microsoft Fluent accessibility: https://fluent2.microsoft.design/accessibility
- Oracle Redwood adoption: https://docs.oracle.com/en/cloud/saas/readiness/redwood-adoption/
- Odoo record view types: https://www.odoo.com/documentation/19.0/developer/reference/user_interface/view_records.html

## 4. Product information architecture

### 4.1 Global shell

Every authenticated application page uses `_AdminLayout.cshtml` and contains these stable regions:

1. **Left navigation:** role-filtered modules and pages.
2. **Top bar:** sidebar control, one page title, report output controls when applicable, theme, and profile.
3. **Content:** the selected page pattern.
4. **Action footer:** page-level commit and workflow actions.
5. **System feedback layer:** loader, toast, confirmation, modal, and validation summary.

Do not repeat the page title inside the content unless the page has a distinct object title, such as `Purchase Requisition MTRF-000123`.

### 4.2 Navigation hierarchy

Use this hierarchy consistently:

```text
Module
└── Workspace or process group
    └── Page / task
        └── Record or transaction
```

- Navigation labels use business terms, not controller names.
- Group pages by the operator's workflow, not by database structure.
- Active module and page must be visually identifiable.
- Permission-hidden actions must not leave empty navigation groups.
- Opening a transaction from a list must preserve the list's filters and page when practical.

### 4.3 Page title convention

Use a short noun or task name:

- `Production Budget`
- `Material Transfer Request`
- `Company Permissions`
- `Production Monitoring`

Avoid titles such as `Production Budget Page`, `Entry Form`, or controller/action names.

## 5. Standard page patterns

Every page must select one primary pattern before implementation.

### Pattern A: Operational workspace

Use for dashboards, production interfaces, monitoring, approvals, and queues.

```text
[Compact filter bar                         ]
[KPI/status] [KPI/status] [KPI/status] [Alert]
[Primary work list / chart / queue           ]
[Exceptions or related work                  ]
```

Rules:

- KPIs must lead to a task or explain current operational status.
- Show no more than 4-6 first-level KPIs at 1366 px.
- Use color only for semantic status, not to decorate each card differently.
- Default to tables for exact operational data and charts for trends or composition.
- Filters affecting the whole workspace stay at the top; local filters stay with their table.

Use existing `interface-workspace`, `interface-toolbar`, `interface-kpi-grid`, `interface-kpi-card`, and `interface-data-card` classes.

### Pattern B: Transaction entry

Use for requests, transfers, issues, receipts, budgets, vouchers, and other documents.

```text
[Document identity | dates | parties | status]
[Line entry / lookup controls               ]
[Editable line table                        ]
[Totals and narration                       ]
---------------------------------------------
[Fixed page action footer]
```

Rules:

- Header fields establish document context before line entry.
- Put system-generated ID and status in read-only fields or badges.
- Keep line-entry controls directly above the line table.
- Use a native table for editable lines and DataTables for read-only history/search lists.
- Totals remain visible at the bottom of the table or in a summary block.
- Page-level Save/Approve/Delete/Cancel actions belong in the shared footer.
- Row actions belong inside the row; modal Save belongs inside the modal.

Use `enterprise-entry-page`, `entry-header-card`, `entry-form-grid`, `entry-field`, `entry-panel`, `entry-native-table-shell`, and `enterprise-native-table`.

### Pattern C: Master list and record

Use for suppliers, items, users, permissions, configuration, and reference data.

```text
[Search / filters]                  [New]
[Read-heavy DataTable                    ]

Selection -> dedicated page, side panel, or standard modal
```

Rules:

- A list defaults to 20 records and the approved page-length options.
- Search and filters must say what they search.
- Put create/export/bulk actions above the table.
- Put edit/view actions in the final column with text or an accessible label.
- Use a modal only for a short, bounded form. Use a full page for complex records or line collections.
- Preserve page, sort, and filters after a row update when practical.

### Pattern D: Report and inquiry

Use for read-only financial, stock, and operational inquiries.

```text
[Required filters] [Optional filters] [Run]
[Applied-filter summary] [Export/Print]
[Report result or DataTable             ]
[Totals / grouping                      ]
```

Rules:

- Do not run expensive reports automatically on first load unless the result is intentionally small.
- `Run report` or `Show` is the primary action; export and print are secondary.
- The applied filters must remain visible after results load.
- Empty report results must state that no records match the selected filters.
- Screen, print, and export use the same authorized company and filters.

### Pattern E: Guided process

Use only when the operation has a real sequence that prevents valid completion out of order.

```text
1. Context -> 2. Details -> 3. Review -> 4. Complete
```

Rules:

- Do not use a stepper merely to divide a long form.
- The current, completed, and unavailable steps must be explicit.
- Preserve entered state when moving backward.
- Show a final review before an irreversible or high-impact commit.

## 6. Visual design system

### 6.1 Color

Use only semantic tokens from `enterprise-components.css` for shared components. Never hard-code white backgrounds, black text, or arbitrary colored shadows in a page.

| Purpose              | Token / class                | Usage                                 |
| -------------------- | ---------------------------- | ------------------------------------- |
| Primary brand/action | `--enterprise-primary`       | Main action, focus, active navigation |
| Supporting accent    | `--enterprise-accent`        | Selected or supporting emphasis       |
| Warning/highlight    | `--enterprise-highlight`     | Warning or exceptional attention      |
| Surface              | `--enterprise-surface`       | Cards, forms, modals                  |
| Muted surface        | `--enterprise-surface-muted` | Read-only, grouped, subtle sections   |
| Main text            | `--enterprise-text`          | Labels and content                    |
| Muted text           | `--enterprise-text-muted`    | Hints and metadata                    |
| Border               | `--enterprise-border`        | Dividers and controls                 |

Semantic status must be consistent:

- **Success:** completed, posted, approved, in stock.
- **Warning:** attention required, low stock, expiring, pending near deadline.
- **Danger:** failed, rejected, overdue, invalid, destructive action.
- **Info:** neutral workflow state, draft, in review, informational.

Never use color as the only carrier of status. Pair it with text and, where useful, an icon.

### 6.2 Typography

- Use the shell's established font stack; do not import a page-specific font.
- Page title: supplied by the top bar.
- Section title: 16-18 px, semibold.
- Form labels: 12-13 px, semibold.
- Body/table: 12-14 px depending on density.
- Helper/metadata: minimum 11 px.
- Use sentence case: `Save budget`, not `SAVE BUDGET`.
- Avoid decorative bold text. Weight indicates hierarchy, not decoration.

### 6.3 Spacing, radius, and elevation

- Use a 4 px base rhythm: 4, 8, 12, 16, 24, 32.
- Default page gap: 16 px.
- Card padding: 12-16 px on compact pages; 20-24 px only on simple pages.
- Use `--enterprise-radius` for standard controls and cards.
- Use at most one subtle shadow level for raised surfaces.
- Do not use `shadow-xl`, glowing hover effects, large colored shadows, or card lift animation on operational forms.
- Borders and spacing should create most grouping; shadows are secondary.

### 6.4 Density

Use `ui-size-xm` as the normal ERP control size and `ui-size-xs` inside editable grids. A row must not mix heights.

- `ui-size-xs`: dense editable table controls and compact action footer.
- `ui-size-xm`: standard form fields and filter bars.
- `ui-size-md`: rare high-emphasis standalone input.
- `ui-size-lg`: login or exceptional landing-page use only.

Do not create new page-specific height classes.

## 7. Component rules

### 7.1 Forms

- Every control has a persistent visible label. Placeholder text is an example or hint, not a label.
- Required labels use one shared required marker and server-side validation.
- Keep related fields in a section with a short heading.
- Order fields by the operator's task sequence.
- Use read-only styling for values users cannot change; do not merely disable everything without explanation.
- Tab order follows visual order.
- On failure, preserve user input and focus the first invalid field.
- Show field validation next to the field and a summary for multi-error submissions.
- Disable only dependent regions while loading; do not freeze the whole page unnecessarily.

### 7.2 Dates

- Display and entry format: `dd-MMM-yyyy`, for example `22-Oct-2026`.
- Use Bootstrap Datepicker through `.datepicker` or `.ui-datepicker-input`.
- Do not use HTML `type="date"`, jQuery UI Datepicker, or page-specific date formats.
- Server-side parsing must follow the existing business-date convention and must not depend on browser locale.
- Date ranges validate `from <= to` on client and server.

### 7.3 Numbers, quantities, rates, and amounts

- Right-align numeric input and output.
- Align decimal precision within a column.
- Preserve domain precision and rounding order; presentation must not change calculations.
- Use thousands separators for read-only amounts where existing business behavior permits.
- Do not put unit text inside editable numeric values; show it in the label or unit column.
- Negative values must remain easy to distinguish and must not rely only on red color.

### 7.4 Select2 and lookups

- Use native select for very small stable option sets; use Select2 for searchable business lookups.
- Placeholder: `Select supplier`, not `Choose an option...` when the business object is known.
- Add a clear option only when empty is a valid business value.
- Initialize through `Helpers.loadSelect2WithData` or `EnterpriseUI.initializeSelect2`.
- Never initialize the same select twice.
- Modal Select2 controls use the shared modal-parent handling.
- Large lookups must use server-side search rather than downloading all records.

### 7.5 Tables and grids

Choose intentionally:

| Need                                                      | Component                                         |
| --------------------------------------------------------- | ------------------------------------------------- |
| Editable lines, formulas, hierarchy, client working state | Native table                                      |
| Read-heavy list, sort, filter, paging, export             | DataTables                                        |
| Small summary with no interactions                        | Native table                                      |
| Responsive object summary on narrow screen                | Cards only when rows cannot remain understandable |

All tables must provide:

- A clear header and stable column order.
- Right alignment for numeric columns.
- `dd-MMM-yyyy` for dates.
- A visible empty state.
- Loading feedback that cannot be confused with empty data.
- A table-level error and retry path when loading fails.
- Sticky headers only when the scroll container is deliberate.
- Totals in `tfoot` or a nearby summary, not hidden in JavaScript-only state.
- Horizontal scrolling for necessary wide grids; do not shrink text below the approved minimum.

DataTables defaults:

```javascript
pageLength: 20,
lengthMenu: [10, 15, 20, 30, 50, 100, 300],
responsive: true
```

### 7.6 Buttons and action hierarchy

Use one clear primary action per region.

| Level       | Examples                            | Treatment                                                          |
| ----------- | ----------------------------------- | ------------------------------------------------------------------ |
| Primary     | Save, Submit, Approve, Run report   | Filled primary/success according to established workflow semantics |
| Secondary   | Refresh, Recalculate, Export, Print | Outline or neutral                                                 |
| Tertiary    | View details, Clear filters         | Text/low-emphasis                                                  |
| Destructive | Delete, Reject, Cancel transaction  | Danger; confirm with consequences                                  |

- Button text starts with a verb.
- Icons support text; icon-only buttons require `aria-label` and tooltip/title.
- Disable the initiating action while its request is pending and show progress.
- Do not close a modal or clear a form until a save succeeds.
- Confirm only destructive, irreversible, or unusually high-impact actions. Do not confirm routine saves.

### 7.7 Shared action footer

The shared footer is the single location for page-level workflow actions. Configure it per page and bind handlers once.

Recommended order from left to right:

```text
Approve | Refresh | Delete | Recalculate | Save | Save and new | Cancel
```

Only display actions valid for the current record state and permission. Hidden buttons must not remain keyboard-focusable. Do not add a second page-level Save button in a card.

### 7.8 Modals

Use a modal for focused work that is short and safe to interrupt: confirmation, lookup, short edit, row details, or assignment.

- Standard sizes: default, `modal-lg`, or `modal-xl`; choose the smallest that fits.
- Use `modal-dialog-centered` for short dialogs and `modal-dialog-scrollable` for long content.
- Title describes the task or object.
- Footer contains Cancel then the primary action.
- Focus moves into the modal, stays trapped by Bootstrap, and returns to the trigger.
- Do not nest modals.
- A form requiring tabs, multiple line grids, or extensive validation deserves a full page.

### 7.9 Notifications and state feedback

Use the least disruptive feedback appropriate to the event:

- **Inline validation:** correctable field/form problem.
- **Inline status/message bar:** page-level warning, permission limitation, or recoverable load failure.
- **Toast:** successful background or completed routine action.
- **Confirmation dialog:** destructive or irreversible action.
- **Modal message:** only when the user must acknowledge before continuing.
- **Loader/skeleton:** active request. Include meaningful text for longer operations.

Messages state what happened and what the user can do:

- Good: `Material request MTRF-1024 was saved.`
- Good: `The request was not saved. Quantity exceeds available stock.`
- Poor: `Success.`
- Poor: `An error occurred.`

Never display exception details, SQL text, connection information, or stack traces.

## 8. Accessibility baseline

Every migrated or new page must meet these requirements:

- Semantic headings follow one logical hierarchy.
- Every form control has an associated `<label for>`.
- Icon-only actions have an accessible name.
- Keyboard users can reach and operate every action.
- Visible focus is not removed.
- Text and meaningful UI meet WCAG 2.1 AA contrast.
- Status changes important to task completion use an appropriate live region or focus change.
- Tables use real header cells and correct `scope` where ambiguity exists.
- Modals have an accessible title and close label.
- Required state, invalid state, and error text are programmatically connected.
- Color, icon, or position is never the only way information is communicated.
- Touch targets remain usable on small screens even when desktop density is compact.

## 9. Responsive behavior

Design and verify at these practical widths:

| Width      | Expected behavior                                                           |
| ---------- | --------------------------------------------------------------------------- |
| 1440+      | Full sidebar, multi-column form, wide table                                 |
| 1366 x 768 | Primary acceptance viewport; no hidden workflow actions                     |
| 1024       | Reduced columns; sidebar may collapse; tables scroll intentionally          |
| 768        | Two-column or stacked filters; action footer wraps                          |
| 360-430    | Single-column form; modal fits viewport; essential actions remain available |

Desktop data density is important, but mobile does not need every desktop column. Preserve the task, not the exact desktop geometry. Allow horizontal table scrolling when hiding a column would remove critical meaning.

## 10. Standard Razor structure

Use a readable tag-per-line structure and page-specific style/script sections.

```cshtml
@{
    ViewData["Title"] = "Material Transfer Request";
}

@section Styles {
    <link rel="stylesheet"
          href="~/css/f_07_rm/material-transfer-request.css"
          asp-append-version="true" />
}

<div class="enterprise-entry-page">
    <section class="card entry-header-card" aria-labelledby="documentContextTitle">
        <div class="card-body">
            <h2 id="documentContextTitle" class="ui-section-title">
                Document context
            </h2>

            <div class="entry-form-grid">
                <div class="entry-field entry-span-2">
                    <label for="txtRequestDate" class="form-label ui-label-required">
                        Request date
                    </label>
                    <input id="txtRequestDate"
                           name="requestDate"
                           type="text"
                           class="form-control ui-size-xm datepicker"
                           autocomplete="off"
                           required />
                    <span class="ui-validation" data-valmsg-for="requestDate"></span>
                </div>
            </div>
        </div>
    </section>

    <section class="entry-panel" aria-labelledby="linesTitle">
        <h2 id="linesTitle" class="ui-section-title">
            Request lines
        </h2>

        <div class="entry-native-table-shell">
            <table class="table table-striped table-hover enterprise-native-table">
                <thead>
                    <tr>
                        <th scope="col">#</th>
                        <th scope="col">Resource</th>
                        <th scope="col" class="text-end">Quantity</th>
                    </tr>
                </thead>
                <tbody id="requestLinesBody">
                </tbody>
            </table>
        </div>
    </section>
</div>

@section Scripts {
    <script src="~/js/J_07_RM/jMaterialTransferRequest.js"
            asp-append-version="true">
    </script>
}
```

Do not place page CSS in `<style>` inside the Razor file. Do not load framework libraries again at page level.

## 11. Standard JavaScript structure

Follow the existing module convention. The example below defines responsibilities; preserve existing project naming when migrating an established page.

```javascript
const commonPath = "/f_07_rm/materialtransferrequest/";

var MaterialTransferRequestPage = (function () {
  var state = {
    requestId: null,
    lines: [],
    isSaving: false,
  };

  var service = {
    async loadLookups() {
      return Helpers.withLoader(function () {
        return FetchHelpers.getForm(
          FetchHelpers.urlconfig(`${commonPath}GetLookups`),
        );
      });
    },

    async save(request) {
      return Helpers.withLoader(function () {
        return FetchHelpers.postJson(
          FetchHelpers.urlconfig(`${commonPath}Save`),
          request,
        );
      });
    },
  };

  var ui = {
    setSaving(isSaving) {
      state.isSaving = isSaving;

      var saveButton = document.getElementById("btnFooterSave");

      if (saveButton) {
        saveButton.disabled = isSaving;
      }
    },

    renderLines(lines) {
      var body = document.getElementById("requestLinesBody");

      body.innerHTML = "";

      if (!lines.length) {
        body.innerHTML = `
                    <tr>
                        <td colspan="3" class="ui-empty-state">
                            No request lines added.
                        </td>
                    </tr>
                `;

        return;
      }

      lines.forEach(function (line, index) {
        body.insertAdjacentHTML(
          "beforeend",
          `
                    <tr>
                        <td>${index + 1}</td>
                        <td>${line.description}</td>
                        <td class="text-end">${line.quantity}</td>
                    </tr>
                `,
        );
      });
    },
  };

  function bindEvents() {
    var saveButton = document.getElementById("btnFooterSave");

    if (saveButton) {
      saveButton.addEventListener("click", saveRequest);
    }
  }

  async function saveRequest() {
    if (state.isSaving) {
      return;
    }

    ui.setSaving(true);

    try {
      var request = buildRequest();
      var response = await service.save(request);

      Notifications.Toastr.success(response.message);
    } catch (error) {
      Notifications.Toastr.error(
        error?.objError || "The request could not be saved.",
      );
    } finally {
      ui.setSaving(false);
    }
  }

  function buildRequest() {
    return {
      requestId: state.requestId,
      lines: state.lines,
    };
  }

  async function loadInitialData() {
    var lookupData = await service.loadLookups();

    // Render lookups through the UI layer.
  }

  function init() {
    bindEvents();
    ShowFooterWithButtons();
    HideRecalculate();
    loadInitialData();
  }

  return {
    init: init,
  };
})();

document.addEventListener("DOMContentLoaded", MaterialTransferRequestPage.init);
```

Additional rules:

- `state` contains mutable client state.
- `service` contains network calls only.
- `ui` contains DOM reads/writes and rendering only.
- Business workflow functions coordinate state, service, validation, and UI.
- Bind each event once.
- Escape or safely render server-provided text; do not inject untrusted HTML.
- State-changing calls follow the existing anti-forgery convention.
- Do not trust posted company code, user ID, status, rate, amount, or permission.

## 12. Page implementation workflow

Apply this sequence to every page migration or redesign.

### Step 1: Discover behavior

- Inspect ASPX, code-behind, JavaScript, stored procedure CallTypes, report definitions, and permissions.
- List page actions: load, search, add line, calculate, save, update, delete, approve, print, export, and close.
- Document record states and which actions are valid in each state.
- Identify required fields, calculations, rounding, dates, and authorization rules.

### Step 2: Select one page pattern

Choose operational workspace, transaction entry, master list/record, report/inquiry, or guided process. Do not improvise a new structure until the standard patterns have been evaluated.

### Step 3: Define the page contract

Before writing markup, define:

- Page title and business purpose.
- Primary user role and primary task.
- Header/context fields.
- Main data region.
- Primary, secondary, destructive, row, and modal actions.
- Loading, empty, invalid, no-permission, failure, and success states.
- Desktop and narrow-screen behavior.

### Step 4: Build with shared components

- Use `_AdminLayout.cshtml`.
- Use existing design tokens and page-pattern classes.
- Add page CSS only for genuinely page-specific composition.
- Use shared Datepicker, Select2, DataTables, footer, loader, fetch, notification, and confirmation helpers.
- Keep tags and JavaScript readable and unminified.

### Step 5: Connect the complete path

Implement View and JavaScript through Controller, Service, Data, and stored procedure mapping. Server validation, company isolation, permissions, and anti-forgery are mandatory regardless of client behavior.

### Step 6: Verify behavior and UX

Verify:

- Default load and populated state.
- Empty state.
- Invalid input and server validation.
- Slow request and double-click prevention.
- Failed lookup/load/save with retry or recovery.
- Save/update/delete/approval state transitions.
- Permission denial and company isolation.
- Date display and parsing.
- Numeric alignment, precision, totals, and rounding.
- Print/export filter parity.
- Keyboard-only operation and visible focus.
- Light and dark theme.
- 1366 x 768, 1024 px, 768 px, and phone width.
- No duplicate Select2, Datepicker, DataTable, or event initialization.

### Step 7: Complete engineering validation

- Build the affected project.
- Run relevant tests.
- Compare actions and representative calculations with Web Forms.
- Record any unclear source behavior instead of silently changing it.
- Report what was migrated, what was verified, and remaining gaps.

## 13. Definition of done checklist

### Structure

- [ ] One approved page pattern is used.
- [ ] `_AdminLayout.cshtml` supplies the application shell.
- [ ] No duplicate page heading is present.
- [ ] Razor markup is readable with line breaks between tags.
- [ ] Page CSS is in a stylesheet, not an inline `<style>` block.

### Controls and content

- [ ] Controls in the same row have the same height.
- [ ] Every control has a visible associated label.
- [ ] Required fields and validation are clear.
- [ ] Dates use Bootstrap Datepicker and `dd-MMM-yyyy`.
- [ ] Numeric inputs and cells are right-aligned.
- [ ] Business terms and button labels are consistent.

### Actions and feedback

- [ ] One primary action exists per region.
- [ ] Page-level actions use the shared footer.
- [ ] Save is disabled while pending and restored afterward.
- [ ] Modal closes only after successful save.
- [ ] Loading, empty, success, validation, failure, and permission states are implemented.
- [ ] Destructive actions explain consequences and require confirmation.

### Tables and responsiveness

- [ ] Native table versus DataTables was selected intentionally.
- [ ] DataTables default to 20 rows and approved length options.
- [ ] Table dates, numbers, totals, and empty state follow the standard.
- [ ] The page works at 1366 x 768 without hiding required actions.
- [ ] Narrow-screen behavior is verified.

### Accessibility and security

- [ ] Keyboard flow and visible focus are verified.
- [ ] Icon-only actions have accessible names.
- [ ] Status is not communicated only by color.
- [ ] Server validation, authorization, company isolation, and anti-forgery are enforced.
- [ ] Browser messages do not expose implementation or exception details.

### Completion

- [ ] Affected project builds.
- [ ] Relevant tests pass.
- [ ] Web Forms behavior has been compared.
- [ ] Remaining gaps are documented.

## 14. Adoption roadmap

### Phase 1: Freeze the foundation

1. Declare `_AdminLayout.cshtml`, `enterprise-components.css`, `enterprise-ui.js`, and the shared footer as the supported foundation.
2. Stop adding new inline page styles and new global helper duplicates.
3. Decide whether to upgrade DataTables integration to Bootstrap 5 or keep a tested compatibility layer; do not continue with an undocumented mix.
4. Mark `_MainLayout.cshtml` and the legacy sidebar as deprecated or document their only valid use.

### Phase 2: Consolidate high-use workflows

1. Migrate the most frequently used transaction pages to Pattern B.
2. Migrate production monitoring and interfaces to Pattern A.
3. Standardize permissions and master data on Pattern C.
4. Move existing Razor `<style>` blocks into scoped stylesheets and replace hard-coded colors with tokens.

### Phase 3: Accessibility and productivity

1. Run keyboard and contrast checks across the shell and shared components.
2. Add consistent validation summary, empty state, error/retry state, and live status patterns.
3. Add optional keyboard shortcuts only after actions and focus behavior are standardized.
4. Measure task completion time and error rate for high-volume workflows.

### Phase 4: Governance

1. Review this checklist in every page pull request.
2. Maintain a small internal component gallery showing approved controls and page patterns.
3. Reject new one-off colors, control heights, modal layouts, and table initializers unless they extend the design system deliberately.
4. Review the standard quarterly against user feedback and production evidence.

## 15. Recommended first cleanup targets

1. **Purchase Material Request:** remove embedded visual effects and style rules, adopt the transaction-entry classes, and keep page-level actions only in the footer.
2. **Production Budget:** normalize markup indentation, action hierarchy, editable table alignment, and page-specific stylesheet ownership.
3. **Production Interface:** route DataTables initialization through `EnterpriseUI` and standardize workspace filters and empty/error states.
4. **Legacy layouts/sidebar:** explicitly deprecate or isolate them so new pages cannot accidentally use old branding and duplicate plugin initialization.
5. **Shared dependencies:** resolve Bootstrap 5 shell versus DataTables Bootstrap 4 adapter compatibility as a controlled technical task.

These targets provide the greatest consistency gain without changing business logic.

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
    init: init,
  };
})();

document.addEventListener("DOMContentLoaded", ProductionInterfacePage.init);
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
    save: "#btnFooterSave",
  };

  const state = {
    documentId: null,
    lines: [],
    lookups: {},
    table: null,
    isLoading: false,
    isSaving: false,
  };

  const service = {
    async loadLookups() {
      return Helpers.withLoader(function () {
        return FetchHelpers.getForm(
          FetchHelpers.urlconfig(`${commonPath}GetLookups`),
        );
      });
    },

    async save(request) {
      return Helpers.withLoader(function () {
        return FetchHelpers.postJson(
          FetchHelpers.urlconfig(`${commonPath}Save`),
          request,
        );
      });
    },
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
    },
  };

  const validation = {
    buildRequest() {
      return {
        documentId: state.documentId,
        date: ui.getValue(selectors.date),
        projectCode: ui.getValue(selectors.project),
        lines: state.lines,
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
    },
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
    } catch (error) {
      Notifications.Toastr.error(
        error?.objError || "The document could not be saved.",
      );
    } finally {
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
    } catch (error) {
      Notifications.Toastr.error(
        error?.objError || "Initial data could not be loaded.",
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
    init: init,
  };
})();

document.addEventListener("DOMContentLoaded", MaterialTransferPage.init);
```

## 5. State rules

State contains mutable page data only:

```javascript
const state = {
  selectedId: null,
  records: [],
  table: null,
  isLoading: false,
  isSaving: false,
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
      return FetchHelpers.getForm(FetchHelpers.urlconfig(`${commonPath}Get`), {
        id: id,
      });
    });
  },
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
  rowNumberColumn,
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
  wrappedTextColumn("actdesc"),
];
```

Factories provide:

| Factory                    | Behavior                                              |
| -------------------------- | ----------------------------------------------------- |
| `textColumn(field)`        | Plain data column with safe empty default             |
| `wrappedTextColumn(field)` | Long description column with shared wrapping class    |
| `numericColumn(field)`     | Shared numeric class and right alignment              |
| `dateColumn(field)`        | `dd-MMM-yyyy` display while preserving raw sort value |
| `rowNumberColumn()`        | Correct row number across paginated pages             |

Every factory accepts an optional DataTables override object:

```javascript
numericColumn("amount", {
  name: "amount",
  searchable: false,
});

textColumn("status", {
  render: renderStatusBadge,
});
```

Page-specific action columns remain local because their permissions and workflows differ.

Use the shared table initializer:

```javascript
state.table = EnterpriseUI.createDataTable("#tblRecords", {
  enterpriseTitle: "Material requests",
  data: state.records,
  columns: columns,
  order: [],
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
$(selectors.table).on("click", ".row-action[data-action='edit']", function () {
  editRecord(this);
});
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

| Item                    | Convention                  | Example                              |
| ----------------------- | --------------------------- | ------------------------------------ |
| Page module             | PascalCase + `Page`         | `ProductionInterfacePage`            |
| State/property/function | camelCase                   | `selectedInterface`                  |
| DOM ID                  | Existing project convention | `txtFromDate`, `ddlProject`          |
| Constant route          | `commonPath`                | `/F_13_ProdMon/ProductionInterface/` |
| Loader                  | `load...`                   | `loadInitialData`                    |
| Renderer                | `render...`                 | `renderLines`                        |
| Event setup             | `bindEvents`                | One per module                       |
| Request builder         | `build...Request`           | `buildSaveRequest`                   |
| Validator               | `validate...`               | `validateSaveRequest`                |
| Workflow                | Business verb               | `saveBudget`, `approveRequest`       |

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
