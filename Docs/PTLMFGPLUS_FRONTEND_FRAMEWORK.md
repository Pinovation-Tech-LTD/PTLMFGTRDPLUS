# PTLMFGPLUS Frontend Framework

Use the shared framework for every new or migrated MVC page. Start with
`Docs/PTLMFGPLUS_PAGE_SCRIPT_TEMPLATE.js`; do not recreate these capabilities locally.

## DataTables

Create tables only through:

```javascript
state.table = EnterpriseUI.createDataTable("#tblRecords", {
    data: state.records,
    columns: columns,
    order: []
});
```

The framework owns the 20-row default, approved page-length menu, language,
responsive behavior, scrolling, state, and standard toolbar/footer layout.
Use `EnterpriseUI.getDataTableDefaults()` only when a third-party integration
requires a settings object instead of `createDataTable`.

## Column builders

Use `EnterpriseUI.dataTableColumns`:

- `safeTextColumn(field)` for plain server text.
- `wrappedTextColumn(field)` for long safe text.
- `numericColumn(field, { numberFormat })` for right-aligned numbers.
- `dateColumn(field)` for `dd-MMM-yyyy` display and raw-value sorting.
- `rowNumberColumn()` for page-aware row numbers.
- `actionColumn(actions)` for accessible, encoded row actions.

Never concatenate server data into an HTML action string.

## Footer totals

```javascript
footerCallback: EnterpriseUI.dataTableFooter.create([
    {
        column: 4,
        numberFormat: {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }
    }
], {
    labelColumn: 0,
    label: "Total"
})
```

Totals use filtered rows by default. A definition may provide `value`,
`aggregate`, or `format` functions for an explicit business-specific rule.

## Responsive row lookup

```javascript
const row = EnterpriseUI.getResponsiveRowData(state.table, button);
```

This resolves both a normal DataTables row and a Responsive child row.

## Dates

Use `EnterpriseUI.dates`:

```javascript
const date = EnterpriseUI.dates.parse(value);
const display = EnterpriseUI.dates.format(value);
const serverValue = EnterpriseUI.dates.serialize(value, "display");
const range = EnterpriseUI.dates.validateRange(fromValue, toValue);
```

Initialize Bootstrap Datepicker filters from configuration:

```javascript
EnterpriseUI.dates.initializeFilters([
    { txtfrmdate: 30 },
    { txttodate: 0 },
    { txtdate: 7 },
    { txtMonthStart: "First" },
    { txtMonthEnd: "Last" }
]);
```

A positive numeric shorthand means that many days before today. `0` means
today. `"First"` and `"Last"` mean the first and last date of the current
month. Existing input is preserved unless `overwrite: true` is configured.

Advanced per-control configuration is also supported:

```javascript
EnterpriseUI.dates.initializeFilters([
    {
        txtReviewDate: {
            value: "First",
            monthOffset: -1,
            daysAfter: 2,
            overwrite: true,
            datepicker: {
                startDate: "01-Jan-2025",
                endDate: "31-Dec-2030",
                daysOfWeekDisabled: [0, 6]
            }
        }
    }
]);
```

Advanced rules accept `value`, `date`, `defaultValue`, `monthOffset`,
`daysBefore`, `daysAfter`, `offsetDays`, `day: "First"`, `day: "Last"`,
`overwrite`, and any Bootstrap Datepicker option under `datepicker`.

Parsing accepts the project display format, ISO dates, JavaScript `Date`, and
legacy JSON dates. Invalid calendar dates return `null`. The default serializer
returns `yyyy-MM-dd`; pass `"display"` when an existing endpoint requires
`dd-MMM-yyyy`.

## Requests, errors, and pending state

Use `Helpers.executeRequest` for page workflows:

```javascript
await Helpers.executeRequest({
    pending: "#btnFooterSave",
    fallbackMessage: "The record could not be saved.",
    action: function () {
        return service.save(request);
    },
    onSuccess: function (result) {
        Notifications.Toastr.success("Saved", result.message);
    }
});
```

It disables and restores initiating controls, uses the shared loader, routes
errors through `Notifications.handleError`, retains correlation IDs, and always
restores pending state. Use `onError` only to restore page data or show an inline
error state; do not show a second toast.

`FetchHelpers` automatically includes the existing anti-forgery token on
non-GET requests when the view contains `@Html.AntiForgeryToken()`.

## Loader

`AppLoader` is reference-counted. Nested or concurrent requests cannot hide the
loader while another operation is still active. Page code normally uses it
indirectly through `Helpers.executeRequest` or `Helpers.withLoader`.

When a service method is called inside `Helpers.executeRequest`, pass `false`
as the final `FetchHelpers` argument so only the workflow owns the loader:

```javascript
return FetchHelpers.postJson(url, request, false);
```
