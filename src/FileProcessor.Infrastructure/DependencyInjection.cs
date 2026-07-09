using FileProcessor.Application.Interfaces;
using FileProcessor.Infrastructure.Data;
using FileProcessor.Infrastructure.Repositories;
using FileProcessor.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FileProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseInMemoryDatabase("FileProcessorDb"));

        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFileProcessingService, CsvProcessingService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}
