var FetchHelpers = (function () {

    /* =========================
       CONFIG
    ========================= */

    function urlconfig(path) {
        if (/^https?:\/\//i.test(path)) {
            return path;
        }

        return appPath.replace(/\/$/, "") + "/" + path.replace(/^\//, "");
    }

    function showLoaderSafe() {
        if (typeof showLoader === "function") showLoader();
    }

    function hideLoaderSafe() {
        if (typeof hideLoader === "function") hideLoader();
    }

    function createTraceId() {
        if (window.crypto && typeof window.crypto.randomUUID === "function") {
            return window.crypto.randomUUID();
        }

        return Date.now().toString(36) + Math.random().toString(36).slice(2);
    }

    function getMessage(payload, status) {
        if (typeof payload === "string" && payload.trim()) {
            return payload;
        }

        return payload?.message
            || payload?.objError
            || payload?.title
            || `Request failed with status ${status}.`;
    }

    function createError(response, payload, requestTraceId) {
        const error = new Error(getMessage(payload, response.status));
        error.name = "HttpError";
        error.status = response.status;
        error.errorCode = payload?.errorCode || "REQUEST_FAILED";
        error.traceId = payload?.traceId
            || response.headers.get("X-Correlation-ID")
            || requestTraceId;
        error.errors = payload?.errors || null;
        error.payload = payload;
        error.objError = error.message;
        return error;
    }

    async function request(method, url, body, contentType, showLoading = true) {

        if (showLoading) showLoaderSafe();

        const requestTraceId = createTraceId();
        const options = {
            method,
            headers: {
                "Accept": "application/json",
                "X-Requested-With": "XMLHttpRequest",
                "X-Correlation-ID": requestTraceId
            }
        };

        if (method !== "GET") {
            const verificationToken = document.querySelector(
                'input[name="__RequestVerificationToken"]'
            )?.value;

            if (verificationToken) {
                options.headers.RequestVerificationToken = verificationToken;
            }
        }

        if (contentType) {
            options.headers["Content-Type"] = contentType;
        }

        if (body && method !== "GET") {
            options.body = body;
        }

        try {
            const response = await fetch(urlconfig(url), options);

            const ct = response.headers.get("content-type");
            let result;

            if (ct && ct.includes("application/json")) {
                result = await response.json();
            } else {
                result = await response.text();
            }

            if (!response.ok) {
                throw createError(response, result, requestTraceId);
            }

            return result;
        }
        catch (error) {
            if (error?.name === "HttpError") {
                throw error;
            }

            const networkError = new Error("The server could not be reached. Check your connection and try again.");
            networkError.name = "NetworkError";
            networkError.errorCode = "NETWORK_ERROR";
            networkError.traceId = requestTraceId;
            networkError.cause = error;
            networkError.objError = networkError.message;
            throw networkError;
        }

        finally {
            if (showLoading) hideLoaderSafe();
        }
    }



    function getForm(url, data = null, showLoading = true) {
        const query = data
            ? "?" + new URLSearchParams(data).toString()
            : "";
        return request(
            "GET",
            url + query,
            null,
            "application/x-www-form-urlencoded",
            showLoading
        );
    }

    function getJson(url, data = null, showLoading = true) {
        const query = data
            ? "?" + new URLSearchParams(data).toString()
            : "";
        return request(
            "GET",
            url + query,
            null,
            "application/json",
            showLoading
        );
    }

    function postForm(url, data, showLoading = true) {
        return request(
            "POST",
            url,
            new URLSearchParams(data).toString(),
            "application/x-www-form-urlencoded",
            showLoading
        );
    }
    // Inside your FetchHelpers object/class

    //function postForm(url, data, showLoading = true) {
    //    const params = new URLSearchParams();

    //    Object.entries(data).forEach(([key, value]) => {
    //        if (Array.isArray(value)) {
    //            value.forEach((item, index) => {
    //                if (typeof item === 'object' && item !== null) {
    //                    Object.entries(item).forEach(([prop, propValue]) => {
    //                        params.append(`${key}[${index}].${prop}`, propValue ?? '');
    //                    });
    //                } else {
    //                    params.append(`${key}[${index}]`, item ?? '');
    //                }
    //            });
    //        } else {
    //            params.append(key, value ?? '');
    //        }
    //    });

    //    return request(
    //        "POST",
    //        url,
    //        params.toString(),
    //        "application/x-www-form-urlencoded",
    //        showLoading
    //    );
    //}
    function postJson(url, data, showLoading = true) {
        return request(
            "POST",
            url,
            JSON.stringify(data),
            "application/json; charset=utf-8",
            showLoading
        );
    }

    function deleteForm(url, data = null, showLoading = true) {
        return request(
            "DELETE",
            url,
            data ? new URLSearchParams(data).toString() : null,
            "application/x-www-form-urlencoded",
            showLoading
        );
    }

    return {
        getForm,
        getJson,
        postForm,
        postJson,
        deleteForm,
        request,
        urlconfig
    };

})();
