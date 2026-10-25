namespace PTLMFGPLUS.WEB.Models
{
    public sealed class ErrorViewModel
    {
        public int StatusCode { get; init; }
        public string Message { get; init; } = string.Empty;
        public string TraceId { get; init; } = string.Empty;
    }
}
