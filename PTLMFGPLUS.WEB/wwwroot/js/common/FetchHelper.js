var FetchHelpers = (function () {

    /* =========================
       CONFIG
    ========================= */

    function urlconfig(path) {
        return appPath.replace(/\/$/, "") + "/" + path.replace(/^\//, "");
    }

    function showLoaderSafe() {
        if (typeof showLoader === "function") showLoader();
    }

    function hideLoaderSafe() {
        if (typeof hideLoader === "function") hideLoader();
    }

    async function request(method, url, body, contentType, showLoading = true) {

        if (showLoading) showLoaderSafe();

        const options = {
            method,
            headers: {}
        };

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
                // FIX: throw parsed JSON instead of raw text
                throw result;
            }

            return result;
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
        urlconfig
    };

})();
