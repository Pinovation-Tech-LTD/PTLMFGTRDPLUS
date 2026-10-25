using System.Collections.Generic;

namespace PTLMFGPLUS.WEB.Models
{
    public sealed class ApiErrorResponse
    {
        public bool Success { get; init; } = false;
        public string Message { get; init; } = string.Empty;
        public string ErrorCode { get; init; } = "UNEXPECTED_ERROR";
        public string TraceId { get; init; } = string.Empty;
        public IDictionary<string, string[]> Errors { get; init; }
    }
}
