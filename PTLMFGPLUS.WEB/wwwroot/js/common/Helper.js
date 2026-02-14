var Helpers = (function () {
    async function withLoader(action) {
        showLoader();
        try {
            return await action();
        }
        finally {
            hideLoader();
        }
    }

    function populateDropdown(
        selectId,
        data,
        valueField,
        textField,
        defaultText = "-- Select --",
        selectedValue = null
    ) {
        const ddl = document.getElementById(selectId);
        ddl.innerHTML = "";

        // Default option
        if (defaultText !== null) {
            const defaultOption = document.createElement("option");
            defaultOption.value = "";
            defaultOption.text = defaultText;
            ddl.appendChild(defaultOption);
        }

        if (!data || data.length === 0) return;

        data.forEach(item => {
            const option = document.createElement("option");
            option.value = item[valueField];
            option.text = item[textField];

            // Set selected
            if (
                selectedValue !== null &&
                String(item[valueField]) === String(selectedValue)
            ) {
                option.selected = true;
            }

            ddl.appendChild(option);
        });
    }
    async function loadSelect2WithAjax({
        selector,
        url,
        method = "GET",
        data = {},
        valueField = "value",
        textField = "text",
        placeholder = "Select an option",
        selectedValue = null,
        dropdownParent = null
    }) {

        const $element = $(`#${selector}`);
        console.log($element);
        // Destroy if already initialized
        if ($element.hasClass("select2-hidden-accessible")) {
            $element.select2("destroy");
        }

        // Initialize Select2
        $element.select2({
            placeholder: placeholder,
            width: "100%",
            allowClear: true,
            dropdownParent: dropdownParent ? $(dropdownParent) : $('body')
        });

        try {

            const response = await $.ajax({
                url: url,
                type: method,
                data: data
            });

            // Clear old options
            $element.empty();

            // Add default empty option
            $element.append(new Option("", "", false, false));

            // Append new options
            response.forEach(item => {
                const option = new Option(
                    item[textField],
                    item[valueField],
                    false,
                    false
                );
                $element.append(option);
            });

            // Set selected value AFTER options loaded
            if (selectedValue !== null) {
                $element.val(String(selectedValue)).trigger("change");
            }

        } catch (error) {
            console.error("Select2 AJAX load error:", error);
        }
    }

    function loadSelect2WithData({ selector,
        data = [],
        valueField = "value",
        textField = "text",
        placeholder = "Select an option",
        selectedValue = null,
        dropdownParent = null })
    {
        const $element = $(`#${selector}`);
        try {

            // Destroy previous Select2 if exists
            if ($element.hasClass("select2-hidden-accessible")) {
                $element.select2("destroy");
            }

            // Clear old options
            $element.empty();

            // Add default empty option
            $element.append(new Option("", "", false, false));

            // Append new options
            if (data && data.length > 0) {
                data.forEach(item => {
                    const option = new Option(
                        item[textField],
                        item[valueField],
                        false,
                        false
                    );
                    $element.append(option);
                });
            }

            // Initialize Select2
            $element.select2({
                placeholder: placeholder,
                width: "100%",
                allowClear: true,
                dropdownParent: dropdownParent ? $(dropdownParent) : $('body')
            });

            // Set selected value if provided
            if (selectedValue !== null) {
                $element.val(String(selectedValue)).trigger("change");
            }

        } catch (error) {
            console.error("Select2 Data Load Error:", error);
        }
    }

    return {
        withLoader,
        populateDropdown,
        loadSelect2WithAjax,
        loadSelect2WithData
    };

    




})();