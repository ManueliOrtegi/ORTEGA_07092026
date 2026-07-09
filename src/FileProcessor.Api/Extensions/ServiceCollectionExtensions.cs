using FileProcessor.Api.Options;
using FileProcessor.Application;
using FileProcessor.Infrastructure;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSwag;
using NSwag.Generation.Processors.Security;

namespace FileProcessor.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiKeyOptions>(configuration.GetSection("ApiKey"));
        services.AddInfrastructure(configuration);
        services.AddMediatR(typeof(FileProcessor.Application.AssemblyMarker));
        services.AddControllers().AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining(typeof(FileProcessor.Application.AssemblyMarker)));
        services.AddEndpointsApiExplorer();
        services.AddOpenApiDocument(cfg =>
        {
            cfg.Title = "FileProcessor API";
            cfg.AddSecurity("ApiKey", new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "X-API-Key",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "API key required to access endpoints"
            });
            cfg.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("ApiKey"));
        });
        return services;
    }
}
