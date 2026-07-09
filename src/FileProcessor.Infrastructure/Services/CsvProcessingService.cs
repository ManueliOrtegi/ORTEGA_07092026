using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using FileProcessor.Application.DTOs;
using FileProcessor.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileProcessor.Infrastructure.Services;

internal sealed class CsvProcessingService : IFileProcessingService
{
    private readonly IDateTimeProvider dateTimeProvider;
    private readonly ILogger<CsvProcessingService> logger;

    public CsvProcessingService(IDateTimeProvider dateTimeProvider, ILogger<CsvProcessingService> logger)
    {
        this.dateTimeProvider = dateTimeProvider;
        this.logger = logger;
    }

    public async Task<ProcessedFileDto> ProcessCsvAsync(Stream fileStream, string fileName, long maxFileSize)
    {
        var start = DateTime.UtcNow;
        using var reader = new StreamReader(fileStream);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            BadDataFound = context => { throw new InvalidDataException("Bad CSV data"); }
        };

        using var csv = new CsvReader(reader, config);
        var records = csv.GetRecords<dynamic>().ToList();
        if (!records.Any())
            throw new InvalidDataException("CSV contains no records");

        var salaries = records.Select(r =>
        {
            var dict = (IDictionary<string, object>)r;
            if (!dict.TryGetValue("Salary", out var val)) throw new InvalidDataException("Salary column missing");
            var s = Convert.ToString(val);
            if (!decimal.TryParse(s, out var salary)) throw new InvalidDataException("Invalid Salary value");
            return salary;
        }).ToList();

        var rows = salaries.Count;
        var avg = (double)salaries.Average(s => (double)s);
        var high = salaries.Max();
        var low = salaries.Min();
        var elapsed = (long)(DateTime.UtcNow - start).TotalMilliseconds;

        logger.LogInformation("Processed file {FileName} in {Elapsed}ms", fileName, elapsed);

        return new ProcessedFileDto
        {
            FileName = fileName,
            UploadedAt = dateTimeProvider.UtcNow,
            RowsProcessed = rows,
            AverageSalary = avg,
            HighestSalary = high,
            LowestSalary = low,
            ProcessingTimeMilliseconds = elapsed
        };
    }
}
