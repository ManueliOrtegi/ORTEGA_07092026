using System.Net;
using System.Text.Json;
using FileProcessor.Api.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace FileProcessor.Api.Middleware;

internal sealed class ApiKeyMiddleware
{
    private readonly RequestDelegate next;
    private readonly string header = "X-API-Key";
    private readonly ApiKeyOptions options;
    private readonly ILogger<ApiKeyMiddleware> logger;

    public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyOptions> options, ILogger<ApiKeyMiddleware> logger)
    {
        this.next = next;
        this.options = options.Value;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.StartsWith("/test", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/openapi", StringComparison.OrdinalIgnoreCase))
        {
            await next(context);
            return;
        }

        if (string.IsNullOrWhiteSpace(options?.Key))
        {
            logger.LogWarning("No API key configured (ApiKey:Key). Requests will be allowed. Configure ApiKey:Key to enable enforcement.");
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(header, out var extracted))
        {
            logger.LogWarning("Missing API key header '{Header}'", header);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            var accept = context.Request.Headers["Accept"].ToString();
            if (accept != null && accept.Contains("text/html"))
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync($"<html><head><title>401 Unauthorized</title></head><body><h1>Invalid API Key</h1><p>The request is missing the required X-API-Key header.</p><p>To access the API, supply the header <code>X-API-Key</code> with a valid key, or fetch the OpenAPI spec at <a href=\"/openapi/spec.json\">/openapi/spec.json</a>.</p></body></html>");
            }
            else
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { success = false, message = "Invalid API Key." }));
            }
            return;
        }

        if (!string.Equals(extracted.ToString().Trim(), options.Key, StringComparison.Ordinal))
        {
            logger.LogInformation("Invalid API key provided. Received length: {ReceivedLength}", extracted.ToString().Length);
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(new { success = false, message = "Invalid API Key." }));
            return;
        }

        await next(context);
    }
}
