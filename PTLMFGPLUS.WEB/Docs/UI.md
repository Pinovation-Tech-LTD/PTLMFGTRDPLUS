<input class="form-control ui-size-xm" />

<select class="form-select ui-size-xm">
</select>

<button class="btn btn-primary ui-size-xm">
    Save
</button>

EnterpriseUI.dates.initializeFilters([
    { txtfrmdate: 30 },
    { txttodate: 0 },
    { txtdate: 7 },
    { txtMonthStart: "First" },
    { txtMonthEnd: "Last" }
]);

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

const ui = Object.assign({}, EnterpriseUI.dom, {})