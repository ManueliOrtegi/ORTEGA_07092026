using Microsoft.Extensions.Logging;

namespace FileProcessor.Api.Middleware;

internal sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Request started {Method} {Path}", context.Request.Method, context.Request.Path);
        var start = DateTime.UtcNow;
        await _next(context);
        var elapsed = (DateTime.UtcNow - start).TotalMilliseconds;
        _logger.LogInformation("Request completed {Method} {Path} in {Elapsed}ms", context.Request.Method, context.Request.Path, elapsed);
    }
}
