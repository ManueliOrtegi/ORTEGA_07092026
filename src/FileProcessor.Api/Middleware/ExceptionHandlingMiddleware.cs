using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace FileProcessor.Api.Middleware;

internal sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ExceptionHandlingMiddleware> logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";
            var traceId = context.TraceIdentifier;
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { success = false, message = "Unexpected error occurred.", traceId }));
        }
    }
}
