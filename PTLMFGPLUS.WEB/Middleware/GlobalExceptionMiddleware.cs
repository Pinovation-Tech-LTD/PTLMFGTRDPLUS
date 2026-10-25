using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using PTLMFGPLUS.WEB.Models;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PTLMFGPLUS.WEB.Middleware
{
    public sealed class GlobalExceptionMiddleware
    {
        private const string TraceHeaderName = "X-Correlation-ID";
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = GetTraceId(context);
            context.Response.OnStarting(() =>
            {
                context.Response.Headers[TraceHeaderName] = traceId;
                return Task.CompletedTask;
            });

            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception,
                    "Unhandled request exception. TraceId: {TraceId}; Method: {Method}; Path: {Path}; User: {User}",
                    traceId,
                    context.Request.Method,
                    context.Request.Path,
                    context.User?.Identity?.Name ?? "Anonymous");

                if (context.Response.HasStarted)
                {
                    throw;
                }

                await WriteErrorResponseAsync(context, traceId);
            }
        }

        private static async Task WriteErrorResponseAsync(HttpContext context, string traceId)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Headers.CacheControl = "no-store, no-cache";

            if (ExpectsJson(context.Request))
            {
                context.Response.ContentType = "application/json; charset=utf-8";
                var response = new ApiErrorResponse
                {
                    Message = "The request could not be completed. Please contact support with the reference ID.",
                    TraceId = traceId
                };
                await JsonSerializer.SerializeAsync(context.Response.Body, response,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));
                return;
            }

            var errorUrl = $"{context.Request.PathBase}/Home/Error?statusCode=500&traceId={Uri.EscapeDataString(traceId)}";
            context.Response.Redirect(errorUrl);
        }

        private static bool ExpectsJson(HttpRequest request)
        {
            return request.Headers.Accept.ToString().Contains("application/json", StringComparison.OrdinalIgnoreCase)
                || request.Headers.XRequestedWith == "XMLHttpRequest"
                || request.Path.StartsWithSegments("/api");
        }

        private static string GetTraceId(HttpContext context)
        {
            var suppliedTraceId = context.Request.Headers[TraceHeaderName].ToString();
            return string.IsNullOrWhiteSpace(suppliedTraceId)
                ? context.TraceIdentifier
                : suppliedTraceId[..Math.Min(suppliedTraceId.Length, 100)];
        }
    }
}
