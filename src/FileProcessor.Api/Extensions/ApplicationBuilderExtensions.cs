using FileProcessor.Api.Middleware;
using Microsoft.AspNetCore.Builder;

namespace FileProcessor.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseApiMiddlewares(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<ApiKeyMiddleware>();
        return app;
    }
}
