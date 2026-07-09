using FileProcessor.Api.Extensions;
using FileProcessor.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Force HTTP on port 3000 for local development
builder.WebHost.UseUrls("http://localhost:3000");

builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();
// Serve OpenAPI (JSON) publicly so tools can fetch the spec without the API key middleware blocking it
app.UseOpenApi(settings => { settings.Path = "/openapi/spec.json"; });

// Apply API middlewares (authorization, logging, exception handling)
app.UseApiMiddlewares();
app.MapControllers();

// Serve Scalar UI for OpenAPI documentation (accessible at /scalar)
app.MapGet("/scalar", async context =>
{
    var html = @"
<!DOCTYPE html>
<html>
<head>
    <title>Scalar API Documentation</title>
    <meta charset='utf-8' />
    <meta name='viewport' content='width=device-width, initial-scale=1' />
    <style>
        body {
            margin: 0;
            padding: 0;
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, sans-serif;
        }
    </style>
</head>
<body>
    <script id='api-reference' data-url='/openapi/spec.json'></script>
    <script src='https://cdn.jsdelivr.net/npm/@scalar/api-reference'></script>
</body>
</html>";
    context.Response.ContentType = "text/html; charset=utf-8";
    await context.Response.WriteAsync(html);
});
app.Run();
