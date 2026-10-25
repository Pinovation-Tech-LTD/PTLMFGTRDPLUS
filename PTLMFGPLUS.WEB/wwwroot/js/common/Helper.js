var Helpers = (function () {
    function getSelect2DropdownParent($element, dropdownParent) {
        if (window.EnterpriseUI?.getSelect2DropdownParent) {
            return window.EnterpriseUI.getSelect2DropdownParent(
                $element,
                dropdownParent
            );
        }

        if (dropdownParent) {
            return $(dropdownParent);
        }

        const $modal = $element.closest(".modal");
        return $modal.length ? $modal : $(document.body);
    }

    async function withLoader(action) {
        if (window.AppLoader) {
            return window.AppLoader.run(action);
        }

        showLoader();

        try {
            return await action();
        }
        finally {
            hideLoader();
        }
    }

    function resolveElements(target) {
        if (!target) {
            return [];
        }

        if (typeof target === "string") {
            return Array.from(document.querySelectorAll(target));
        }

        if (target instanceof Element) {
            return [target];
        }

        return Array.from(target).filter(function (item) {
            return item instanceof Element;
        });
    }

    function setPending(target, pending) {
        resolveElements(target).forEach(function (element) {
            element.disabled = Boolean(pending);
            element.setAttribute("aria-busy", String(Boolean(pending)));
        });
    }

    async function executeRequest(options) {
        const settings = options || {};

        if (typeof settings.action !== "function") {
            throw new TypeError("Helpers.executeRequest requires an action function.");
        }

        setPending(settings.pending, true);

        try {
            const result = settings.useLoader === false
                ? await settings.action()
                : await withLoader(settings.action);

            if (typeof settings.onSuccess === "function") {
                await settings.onSuccess(result);
            }

            return result;
        }
        catch (error) {
            const details = window.Notifications?.handleError
                ? window.Notifications.handleError(
                    error,
                    settings.fallbackMessage || "The operation could not be completed.",
                    { silent: settings.silent === true }
                )
                : { message: error?.message || settings.fallbackMessage };

            if (typeof settings.onError === "function") {
                await settings.onError(error, details);
            }

            if (settings.rethrow === true) {
                throw error;
            }

            return null;
        }
        finally {
            setPending(settings.pending, false);

            if (typeof settings.onFinally === "function") {
                await settings.onFinally();
            }
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
        // Destroy if already initialized
        if ($element.hasClass("select2-hidden-accessible")) {
            $element.select2("destroy");
        }

        // Initialize Select2
        $element.select2({
            placeholder: placeholder,
            width: "100%",
            allowClear: true,
            dropdownParent: getSelect2DropdownParent(
                $element,
                dropdownParent
            )
        });

        if (window.EnterpriseUI?.applySelect2Size) {
            window.EnterpriseUI.applySelect2Size($element);
        }

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
                dropdownParent: getSelect2DropdownParent(
                    $element,
                    dropdownParent
                )
            });

            if (window.EnterpriseUI?.applySelect2Size) {
                window.EnterpriseUI.applySelect2Size($element);
            }

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
        executeRequest,
        setPending,
        populateDropdown,
        loadSelect2WithAjax,
        loadSelect2WithData
    };

    




})();
