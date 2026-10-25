(function (window, $) {
    "use strict";

    // Bootstrap Datepicker's `M` token renders the required dd-MMM-yyyy display (for example, 21-Oct-2026).
    const dateDisplayFormat = "dd-M-yyyy";
    const tablePageLengths = [10, 15, 20, 30, 50, 100, 300];
    const sizeClasses = ["ui-size-xs", "ui-size-sm", "ui-size-xm", "ui-size-md", "ui-size-lg"];
    const dataTableScrollHeight = "clamp(220px, calc(100vh - 360px), 600px)";
    const dataTableDom = [
        "<'enterprise-dt-toolbar'<'enterprise-dt-title'><'enterprise-dt-search'f>>",
        "<'enterprise-dt-table't>",
        "<'enterprise-dt-footer'<'enterprise-dt-info'i><'enterprise-dt-pagination'p><'enterprise-dt-length'l>>"
    ].join("");

    const dataTableLanguage = {
        emptyTable: "No records found.",
        info: "_START_-_END_ of _TOTAL_ items",
        infoEmpty: "0-0 of 0 items",
        infoFiltered: "(filtered from _MAX_ items)",
        lengthMenu: "Items per page _MENU_",
        loadingRecords: "Loading records...",
        processing: "Loading records...",
        search: "",
        searchPlaceholder: "Search",
        zeroRecords: "No matching records found.",
        paginate: {
            first: "First",
            previous: "Previous",
            next: "Next",
            last: "Last"
        }
    };

    function getDataTableDefaults(overrides) {
        const defaults = {
            dom: dataTableDom,
            pageLength: 20,
            lengthMenu: tablePageLengths,
            language: dataTableLanguage,
            pagingType: "full_numbers",
            responsive: true,
            scrollCollapse: false,
            scrollX: true,
            scrollY: dataTableScrollHeight,
            stateSave: true
        };

        return $.extend(true, {}, defaults, overrides || {});
    }

    function mergeColumnOptions(defaults, options) {
        const merged = $.extend({}, defaults, options || {});

        if (defaults.className && options?.className) {
            merged.className = `${defaults.className} ${options.className}`;
        }

        return merged;
    }

    function encodeText(value) {
        const element = document.createElement("div");

        element.textContent = value ?? "";
        return element.innerHTML;
    }

    function encodeAttribute(value) {
        return encodeText(value)
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#39;");
    }

    function getElement(id) {
        return typeof id === "string"
            ? document.getElementById(id)
            : id;
    }

    function getValue(id) {
        const value = getElement(id)?.value;

        return typeof value === "string"
            ? value.trim()
            : value ?? "";
    }

    function setValue(id, value) {
        const element = getElement(id);

        if (element) {
            element.value = value ?? "";
        }
    }

    function getText(id) {
        return getElement(id)?.textContent?.trim() || "";
    }

    function setText(id, value) {
        const element = getElement(id);

        if (element) {
            element.textContent = value ?? "";
        }
    }

    function setVisible(id, visible, displayValue) {
        const element = getElement(id);

        if (!element) {
            return;
        }

        if (!visible) {
            if (element.style.display !== "none") {
                element.dataset.enterpriseDisplay = element.style.display;
            }

            element.style.display = "none";
            return;
        }

        element.style.display = displayValue
            ?? element.dataset.enterpriseDisplay
            ?? "";
    }

    function setDisabled(ids, disabled) {
        const targets = Array.isArray(ids) ? ids : [ids];

        targets.forEach(function (id) {
            const element = getElement(id);

            if (element) {
                element.disabled = Boolean(disabled);
            }
        });
    }

    function loadSelect(id, items, valueField, textField, placeholder, selectedValue) {
        if (!window.Helpers?.loadSelect2WithData) {
            throw new Error("Helpers.loadSelect2WithData is required to load Select2 data.");
        }

        window.Helpers.loadSelect2WithData({
            selector: id,
            data: items || [],
            valueField: valueField,
            textField: textField,
            placeholder: placeholder,
            selectedValue: selectedValue ?? null
        });
    }

    function getNestedValue(source, path) {
        if (!path) {
            return source;
        }

        return String(path)
            .split(".")
            .reduce(function (current, key) {
                return current == null ? undefined : current[key];
            }, source);
    }

    function textColumn(data, options) {
        return mergeColumnOptions({
            data: data,
            defaultContent: ""
        }, options);
    }

    function safeTextColumn(data, options) {
        return mergeColumnOptions({
            data: data,
            defaultContent: "",
            render: function (value, renderType) {
                if (renderType === "display") {
                    return encodeText(value);
                }

                return value ?? "";
            }
        }, options);
    }

    function wrappedTextColumn(data, options) {
        return mergeColumnOptions({
            data: data,
            defaultContent: "",
            className: "interface-cell-wrap",
            render: function (value, renderType) {
                if (renderType === "display") {
                    return encodeText(value);
                }

                return value ?? "";
            }
        }, options);
    }

    function parseNumber(value) {
        if (typeof value === "number") {
            return Number.isFinite(value) ? value : 0;
        }

        const normalized = String(value ?? "")
            .replace(/<[^>]*>/g, "")
            .replace(/,/g, "")
            .trim();
        const number = Number(normalized);

        return Number.isFinite(number) ? number : 0;
    }

    function formatNumber(value, options) {
        const settings = $.extend({
            locale: "en-US",
            minimumFractionDigits: 0,
            maximumFractionDigits: 6,
            emptyValue: ""
        }, options || {});

        if (value === null || value === undefined || value === "") {
            return settings.emptyValue;
        }

        return parseNumber(value).toLocaleString(settings.locale, {
            minimumFractionDigits: settings.minimumFractionDigits,
            maximumFractionDigits: settings.maximumFractionDigits
        });
    }

    function numericColumn(data, options) {
        const settings = options || {};
        const numberFormat = settings.numberFormat;
        const columnOptions = $.extend({}, settings);

        delete columnOptions.numberFormat;

        return mergeColumnOptions({
            data: data,
            defaultContent: "",
            className: "interface-number-cell text-end",
            render: function (value, renderType) {
                if (renderType === "sort" || renderType === "type") {
                    return parseNumber(value);
                }

                return formatNumber(value, numberFormat);
            }
        }, columnOptions);
    }

    function parseDate(value) {
        if (value instanceof Date) {
            return Number.isNaN(value.getTime())
                ? null
                : new Date(value.getFullYear(), value.getMonth(), value.getDate());
        }

        if (value === null || value === undefined || value === "") {
            return null;
        }

        const text = String(value).trim();
        const displayMatch = text.match(/^(\d{1,2})-([A-Za-z]{3})-(\d{4})$/);
        const isoMatch = text.match(/^(\d{4})-(\d{2})-(\d{2})(?:T.*)?$/);
        const jsonDateMatch = text.match(/^\/Date\((-?\d+)(?:[+-]\d+)?\)\/$/);
        let date;

        if (displayMatch) {
            const month = [
                "jan", "feb", "mar", "apr", "may", "jun",
                "jul", "aug", "sep", "oct", "nov", "dec"
            ].indexOf(displayMatch[2].toLowerCase());

            if (month < 0) {
                return null;
            }

            date = new Date(Number(displayMatch[3]), month, Number(displayMatch[1]));
        }
        else if (isoMatch) {
            date = new Date(Number(isoMatch[1]), Number(isoMatch[2]) - 1, Number(isoMatch[3]));
        }
        else if (jsonDateMatch) {
            const parsed = new Date(Number(jsonDateMatch[1]));
            date = new Date(parsed.getFullYear(), parsed.getMonth(), parsed.getDate());
        }
        else {
            const parsed = new Date(text);

            if (Number.isNaN(parsed.getTime())) {
                return null;
            }

            date = new Date(parsed.getFullYear(), parsed.getMonth(), parsed.getDate());
        }

        if (Number.isNaN(date.getTime())) {
            return null;
        }

        if (displayMatch
            && (date.getDate() !== Number(displayMatch[1])
                || date.getMonth() !== [
                    "jan", "feb", "mar", "apr", "may", "jun",
                    "jul", "aug", "sep", "oct", "nov", "dec"
                ].indexOf(displayMatch[2].toLowerCase())
                || date.getFullYear() !== Number(displayMatch[3]))) {
            return null;
        }

        if (isoMatch
            && (date.getDate() !== Number(isoMatch[3])
                || date.getMonth() !== Number(isoMatch[2]) - 1
                || date.getFullYear() !== Number(isoMatch[1]))) {
            return null;
        }

        return date;
    }

    function formatDate(value) {
        const date = parseDate(value);

        if (!date) {
            return "";
        }

        const day = String(date.getDate()).padStart(2, "0");
        const month = date.toLocaleString("en-US", { month: "short" });

        return `${day}-${month}-${date.getFullYear()}`;
    }

    function serializeDate(value, format) {
        const date = parseDate(value);

        if (!date) {
            return null;
        }

        if (format === "display" || format === "dd-MMM-yyyy") {
            return formatDate(date);
        }

        const month = String(date.getMonth() + 1).padStart(2, "0");
        const day = String(date.getDate()).padStart(2, "0");

        return `${date.getFullYear()}-${month}-${day}`;
    }

    function validateDateRange(fromValue, toValue, options) {
        const settings = $.extend({
            required: true,
            requiredMessage: "From date and To date are required.",
            rangeMessage: "From date cannot be later than To date."
        }, options || {});
        const from = parseDate(fromValue);
        const to = parseDate(toValue);

        if (settings.required && (!from || !to)) {
            return {
                valid: false,
                message: settings.requiredMessage,
                from: from,
                to: to
            };
        }

        if ((fromValue && !from) || (toValue && !to)) {
            return {
                valid: false,
                message: settings.invalidMessage || "Enter a valid date in dd-MMM-yyyy format.",
                from: from,
                to: to
            };
        }

        if (from && to && from > to) {
            return {
                valid: false,
                message: settings.rangeMessage,
                from: from,
                to: to
            };
        }

        return {
            valid: true,
            message: "",
            from: from,
            to: to
        };
    }

    function addDays(date, days) {
        const result = new Date(date.getFullYear(), date.getMonth(), date.getDate());

        result.setDate(result.getDate() + Number(days || 0));
        return result;
    }

    function addMonths(date, months) {
        const result = new Date(date.getFullYear(), date.getMonth(), 1);
        const originalDay = date.getDate();

        result.setMonth(result.getMonth() + Number(months || 0));
        result.setDate(Math.min(
            originalDay,
            new Date(result.getFullYear(), result.getMonth() + 1, 0).getDate()
        ));

        return result;
    }

    function resolveFilterDate(rule, currentDate) {
        const today = parseDate(currentDate) || parseDate(new Date());

        if (typeof rule === "number") {
            return addDays(today, -rule);
        }

        if (rule instanceof Date) {
            return parseDate(rule);
        }

        if (typeof rule === "string") {
            const normalized = rule.trim().toLowerCase();

            if (["today", "current", "now"].includes(normalized)) {
                return today;
            }

            if (["first", "firstday", "startofmonth"].includes(normalized)) {
                return new Date(today.getFullYear(), today.getMonth(), 1);
            }

            if (["last", "lastday", "endofmonth"].includes(normalized)) {
                return new Date(today.getFullYear(), today.getMonth() + 1, 0);
            }

            return parseDate(rule);
        }

        if (!rule || typeof rule !== "object") {
            return today;
        }

        let result = resolveFilterDate(
            rule.value ?? rule.date ?? rule.defaultValue ?? 0,
            today
        );

        if (!result) {
            return null;
        }

        if (rule.monthOffset) {
            result = addMonths(result, rule.monthOffset);
        }

        if (rule.daysBefore) {
            result = addDays(result, -Math.abs(rule.daysBefore));
        }

        if (rule.daysAfter) {
            result = addDays(result, Math.abs(rule.daysAfter));
        }

        if (rule.offsetDays) {
            result = addDays(result, rule.offsetDays);
        }

        const position = String(rule.day || rule.position || "").toLowerCase();

        if (["first", "firstday", "startofmonth"].includes(position)) {
            result = new Date(result.getFullYear(), result.getMonth(), 1);
        }

        if (["last", "lastday", "endofmonth"].includes(position)) {
            result = new Date(result.getFullYear(), result.getMonth() + 1, 0);
        }

        return result;
    }

    function initializeDateFilters(configurations, options) {
        const settings = $.extend(true, {
            currentDate: new Date(),
            overwrite: false,
            datepicker: {
                format: {
                    toDisplay: formatDate,
                    toValue: parseDate
                },
                autoclose: true,
                todayHighlight: true,
                todayBtn: "linked",
                orientation: "bottom auto"
            }
        }, options || {});
        const results = {};

        const items = Array.isArray(configurations)
            ? configurations
            : [configurations];

        items.filter(Boolean).forEach(function (configuration) {
            Object.entries(configuration || {}).forEach(function ([id, rule]) {
                const normalizedId = id.replace(/^#/, "");
                const element = document.getElementById(normalizedId);

                if (!element) {
                    results[normalizedId] = null;
                    return;
                }

                const ruleOptions = rule && typeof rule === "object" && !(rule instanceof Date)
                    ? rule
                    : {};
                const shouldOverwrite = ruleOptions.overwrite ?? settings.overwrite;
                const resolvedDate = resolveFilterDate(rule, settings.currentDate);
                const datepickerOptions = $.extend(
                    true,
                    {},
                    settings.datepicker,
                    ruleOptions.datepicker || {}
                );

                if ($.fn.datepicker) {
                    const $element = $(element);

                    if ($element.data("datepicker")) {
                        $element.datepicker("destroy");
                    }

                    $element.datepicker(datepickerOptions);

                    if (resolvedDate && (shouldOverwrite || !element.value)) {
                        $element.datepicker("setDate", resolvedDate);
                    }
                }
                else if (resolvedDate && (shouldOverwrite || !element.value)) {
                    element.value = formatDate(resolvedDate);
                }

                results[normalizedId] = resolvedDate;
            });
        });

        return results;
    }

    function formatDataTableDate(value) {
        return formatDate(value) || (value ? String(value) : "");
    }

    function dateColumn(data, options) {
        return mergeColumnOptions({
            data: data,
            defaultContent: "",
            render: function (value, renderType) {
                if (renderType === "sort" || renderType === "type") {
                    return value || "";
                }

                return formatDataTableDate(value);
            }
        }, options);
    }

    function rowNumberColumn(options) {
        return mergeColumnOptions({
            data: null,
            orderable: false,
            searchable: false,
            className: "text-end",
            render: function (_value, _renderType, _row, meta) {
                return meta.settings._iDisplayStart + meta.row + 1;
            }
        }, options);
    }

    function actionColumn(actions, options) {
        const definitions = Array.isArray(actions) ? actions : [];

        return mergeColumnOptions({
            data: null,
            orderable: false,
            searchable: false,
            className: "text-center enterprise-action-cell",
            render: function (_value, renderType, row, meta) {
                if (renderType !== "display") {
                    return "";
                }

                return definitions
                    .filter(function (action) {
                        return typeof action.visible !== "function"
                            || action.visible(row, meta);
                    })
                    .map(function (action) {
                        const label = typeof action.label === "function"
                            ? action.label(row, meta)
                            : action.label;
                        const title = typeof action.title === "function"
                            ? action.title(row, meta)
                            : action.title || label;
                        const cssClass = action.className || "btn btn-outline-primary btn-sm";
                        const icon = action.icon
                            ? `<i class="${encodeAttribute(action.icon)}" aria-hidden="true"></i>`
                            : "";
                        const text = action.iconOnly === true ? "" : encodeText(label || "Action");
                        const dataAttributes = Object.keys(action.data || {})
                            .map(function (key) {
                                const configuredValue = action.data[key];
                                const value = typeof configuredValue === "function"
                                    ? configuredValue(row, meta)
                                    : getNestedValue(row, configuredValue);

                                return `data-${encodeAttribute(key)}="${encodeAttribute(value)}"`;
                            })
                            .join(" ");

                        return `
                            <button type="button"
                                    class="${encodeAttribute(cssClass)}"
                                    data-action="${encodeAttribute(action.name || "action")}"
                                    aria-label="${encodeAttribute(label || title || "Action")}"
                                    title="${encodeAttribute(title || "Action")}"
                                    ${dataAttributes}>
                                ${icon}
                                ${text}
                            </button>
                        `;
                    })
                    .join("");
            }
        }, options);
    }

    function aggregateFooter(tableApi, definitions, options) {
        const settings = $.extend({
            scope: { search: "applied" },
            emptyValue: ""
        }, options || {});

        (definitions || []).forEach(function (definition) {
            const column = tableApi.column(definition.column, settings.scope);
            const footer = column.footer();

            if (!footer) {
                return;
            }

            const values = column
                .data()
                .toArray()
                .map(function (value) {
                    if (definition.value) {
                        return definition.value(value);
                    }

                    return parseNumber(value);
                });
            const aggregate = definition.aggregate
                ? definition.aggregate(values)
                : values.reduce(function (total, value) {
                    return total + parseNumber(value);
                }, 0);
            const formatted = definition.format
                ? definition.format(aggregate, values)
                : formatNumber(aggregate, definition.numberFormat);

            footer.textContent = formatted ?? settings.emptyValue;
            footer.classList.add("text-end");
        });

        if (settings.labelColumn !== undefined) {
            const labelFooter = tableApi.column(settings.labelColumn).footer();

            if (labelFooter) {
                labelFooter.textContent = settings.label || "Total";
            }
        }
    }

    function footerAggregator(definitions, options) {
        return function () {
            aggregateFooter(this.api(), definitions, options);
        };
    }

    function getResponsiveRowData(tableOrSelector, trigger) {
        const table = typeof tableOrSelector === "string"
            ? $(tableOrSelector).DataTable()
            : tableOrSelector;
        const rowElement = $(trigger).closest("tr");
        const parentRow = rowElement.hasClass("child")
            ? rowElement.prev()
            : rowElement;

        return table?.row(parentRow)?.data() ?? null;
    }

    function getControlSizeClass($control) {
        if ($control.hasClass("ui-size-xs") || $control.hasClass("form-select-xs")) {
            return "ui-size-xs";
        }

        if ($control.hasClass("ui-size-lg") || $control.hasClass("form-select-lg")) {
            return "ui-size-lg";
        }

        if ($control.hasClass("ui-size-md") || $control.hasClass("form-select-md")) {
            return "ui-size-md";
        }

        if ($control.hasClass("ui-size-xm") || $control.hasClass("form-select-xm")) {
            return "ui-size-xm";
        }

        return "ui-size-sm";
    }

    function applySelect2Size($select) {
        const $container = $select.next(".select2-container");
        $container.removeClass(sizeClasses.join(" "));
        $container.addClass(getControlSizeClass($select));
    }

    function getSelect2DropdownParent($select, dropdownParent) {
        if (dropdownParent) {
            return $(dropdownParent);
        }

        const $modal = $select.closest(".modal");
        return $modal.length ? $modal : $(document.body);
    }

    function initializeDatepickers(scope) {
        if (!$.fn.datepicker) {
            return;
        }

        $(scope)
            .find(".ui-datepicker-input, .datepicker")
            .each(function () {
                const $input = $(this);

                if ($input.data("datepicker")) {
                    return;
                }

                $input.datepicker({
                    format: {
                        toDisplay: formatDate,
                        toValue: parseDate
                    },
                    autoclose: true,
                    todayHighlight: true,
                    todayBtn: "linked",
                    orientation: "bottom auto"
                });
            });
    }

    function initializeSelect2(scope) {
        if (!$.fn.select2) {
            return;
        }

        $(scope)
            .find("select:not(.no-select2)")
            .each(function () {
                const $select = $(this);

                if ($select.hasClass("select2-hidden-accessible")) {
                    applySelect2Size($select);
                    return;
                }

                const options = {
                    width: "100%",
                    placeholder: $select.data("placeholder") || "Select an option",
                    allowClear: false,
                    dropdownParent: getSelect2DropdownParent($select)
                };

                $select.select2(options);
                applySelect2Size($select);
            });
    }

    function initializeDataTables(scope) {
        if (!$.fn.DataTable) {
            return;
        }

        $(scope)
            .find("table.ui-datatable, table[data-datatable='true']")
            .each(function () {
                if ($.fn.DataTable.isDataTable(this)) {
                    return;
                }

                const table = $(this).DataTable(getDataTableDefaults());

                enhanceDataTable(table);
            });
    }

    function enhanceDataTable(table) {
        if (!table || !table.table) {
            return;
        }

        const tableElement = table.table().node();
        const wrapper = table.table().container();

        if (!tableElement || !wrapper) {
            return;
        }

        wrapper.classList.add("enterprise-datatable");
        wrapper.closest(".table-responsive")?.classList.add("enterprise-datatable-host");

        wrapper.querySelectorAll("table.dataTable").forEach(function (dataTable) {
            dataTable.classList.add("enterprise-data-table");
        });

        const settings = table.settings()[0];
        const configuredTitle = settings?.oInit?.enterpriseTitle
            ?? tableElement.dataset.tableTitle
            ?? "";

        setDataTableTitle(table, configuredTitle);

        const filter = wrapper.querySelector(".dataTables_filter");
        const input = filter?.querySelector("input[type='search']");

        if (filter && input && !filter.querySelector(".enterprise-dt-search-control")) {
            const searchControl = document.createElement("div");

            searchControl.className = "enterprise-dt-search-control";

            input.setAttribute("aria-label", "Search table");
            input.setAttribute("autocomplete", "off");
            input.placeholder = input.placeholder || "Search";

            input.parentNode.insertBefore(searchControl, input);
            searchControl.appendChild(input);
        }

        const lengthSelect = wrapper.querySelector(".dataTables_length select");

        if (lengthSelect) {
            lengthSelect.setAttribute("aria-label", "Items per page");
        }
    }

    function setDataTableTitle(tableOrSelector, title) {
        if (!$.fn.DataTable) {
            return;
        }

        const table = typeof tableOrSelector === "string"
            ? $(tableOrSelector).DataTable()
            : tableOrSelector;
        const wrapper = table?.table?.().container();
        const titleElement = wrapper?.querySelector(".enterprise-dt-title");
        const normalizedTitle = String(title || "").trim();

        if (!titleElement) {
            return;
        }

        titleElement.textContent = normalizedTitle;
        titleElement.classList.toggle("is-empty", !normalizedTitle);
        titleElement.setAttribute("aria-hidden", normalizedTitle ? "false" : "true");
    }

    function createDataTable(selector, options) {
        if (!$.fn.DataTable) {
            return null;
        }

        const $table = $(selector);

        if ($.fn.DataTable.isDataTable($table)) {
            $table.DataTable().destroy();
        }

        const table = $table.DataTable(getDataTableDefaults(options));

        enhanceDataTable(table);

        return table;
    }

    function destroyDataTable(selector) {
        if (!$.fn.DataTable) {
            return;
        }

        const table = $(selector).get(0);

        if (table && $.fn.DataTable.isDataTable(table)) {
            const host = table.closest(".table-responsive");

            $(table).DataTable().destroy();
            host?.classList.remove("enterprise-datatable-host");
        }
    }

    function initialize(scope) {
        const root = scope || document;

        initializeDatepickers(root);
        initializeSelect2(root);
        initializeDataTables(root);
    }

    window.EnterpriseUI = {
        initialize: initialize,
        initializeDatepickers: initializeDatepickers,
        initializeSelect2: initializeSelect2,
        getSelect2DropdownParent: getSelect2DropdownParent,
        applySelect2Size: applySelect2Size,
        initializeDataTables: initializeDataTables,
        createDataTable: createDataTable,
        getDataTableDefaults: getDataTableDefaults,
        destroyDataTable: destroyDataTable,
        enhanceDataTable: enhanceDataTable,
        setDataTableTitle: setDataTableTitle,
        dataTableColumns: {
            textColumn: textColumn,
            safeTextColumn: safeTextColumn,
            wrappedTextColumn: wrappedTextColumn,
            numericColumn: numericColumn,
            dateColumn: dateColumn,
            rowNumberColumn: rowNumberColumn,
            actionColumn: actionColumn
        },
        dataTableFooter: {
            aggregate: aggregateFooter,
            create: footerAggregator
        },
        dom: {
            getValue: getValue,
            setValue: setValue,
            getText: getText,
            setText: setText,
            setVisible: setVisible,
            setDisabled: setDisabled,
            loadSelect: loadSelect
        },
        getResponsiveRowData: getResponsiveRowData,
        encodeText: encodeText,
        parseNumber: parseNumber,
        formatNumber: formatNumber,
        dates: {
            parse: parseDate,
            format: formatDate,
            serialize: serializeDate,
            validateRange: validateDateRange,
            resolveFilterDate: resolveFilterDate,
            initializeFilters: initializeDateFilters,
            datepickerFormat: {
                toDisplay: formatDate,
                toValue: parseDate
            }
        },
        formatDataTableDate: formatDataTableDate,
        dateDisplayFormat: dateDisplayFormat,
        tablePageLengths: tablePageLengths
    };

    if ($.fn.dataTable) {
        $.extend(true, $.fn.dataTable.defaults, getDataTableDefaults());
    }

    $(document)
        .off("init.dt.enterpriseUI")
        .on("init.dt.enterpriseUI", function (event, settings) {
            enhanceDataTable(new $.fn.dataTable.Api(settings));
        });

    $(function () {
        initialize(document);

        $(document).on("select2:open", function () {
            document.querySelector(".select2-container--open .select2-search__field")?.focus();
        });

        $(document).on("shown.bs.modal", ".modal", function () {
            initializeSelect2(this);

            $(this)
                .find(".modal-body")
                .off("scroll.enterpriseSelect2")
                .on("scroll.enterpriseSelect2", function () {
                    $(this)
                        .find("select.select2-hidden-accessible")
                        .select2("close");
                });
        });

        if (window.toastr) {
            window.toastr.options = {
                closeButton: true,
                newestOnTop: true,
                progressBar: true,
                positionClass: "toast-top-center",
                timeOut: 5000,
                extendedTimeOut: 1000
            };
        }
    });
})(window, window.jQuery);
