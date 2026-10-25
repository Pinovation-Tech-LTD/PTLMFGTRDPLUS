# PTLMFGPLUS Error Handling Standard

## Error flow

1. Page JavaScript calls the backend through `FetchHelpers`.
2. `FetchHelpers` adds `X-Correlation-ID` and normalizes HTTP/network failures into an `Error` with `status`, `errorCode`, `traceId`, `errors`, and `payload`.
3. Expected page failures are caught and passed to `Notifications.handleError(error, fallbackMessage)`.
4. Unexpected browser errors and unhandled promise rejections are caught globally and displayed through Toastr.
5. Unhandled server exceptions are caught by `GlobalExceptionMiddleware`, logged with correlation ID, route, method, user, and stack trace.
6. AJAX callers receive safe JSON. Full-page callers receive the shared error page. Exception details are never returned to the browser.

## Backend rules

- Do not catch `Exception` merely to return `false`, `null`, or an empty collection. This converts a system failure into apparently valid business data and prevents centralized logging.
- Let unexpected exceptions propagate to `GlobalExceptionMiddleware`.
- Catch an exception only when the method can recover, add meaningful context and rethrow, or convert a known business condition into a deliberate validation response.
- Never return exception messages, SQL, stack traces, connection details, or credentials to the browser.
- Return `400` for invalid input, `401` for unauthenticated requests, `403` for permission denial, `404` for missing records, and `409` for a business-state conflict.
- Include field validation errors in an `errors` dictionary when applicable.
- Search server logs using the reference ID shown to the user.

## Frontend rules

- Use `FetchHelpers.getForm`, `getJson`, `postForm`, `postJson`, or `deleteForm`; do not duplicate raw `fetch` response parsing in page scripts.
- Wrap awaited workflow entry points in `try/catch` and call:

```javascript
try {
    await service.save(request);
}
catch (error) {
    Notifications.handleError(error, "The record could not be saved.");
}
```

- Use a specific fallback message that tells the user which operation failed.
- Do not display a server stack trace or raw response body.
- Do not show both a local notification and allow the same rejected promise to become unhandled, because that produces duplicate messages.

## Legacy migration warning

Several existing services call `GetError()` inside `catch (Exception)` and then return empty data. Migrate those methods page-by-page after confirming the original Web Forms behavior. Until migrated, these swallowed failures cannot be handled by global middleware and may look like empty results.
