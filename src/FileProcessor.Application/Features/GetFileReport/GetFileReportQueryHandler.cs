using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using FileProcessor.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FileProcessor.Application.Features.GetFileReport;

internal sealed class GetFileReportQueryHandler : IRequestHandler<GetFileReportQuery, FileReportDto>
{
    private readonly IFileRepository _repository;
    private readonly ILogger<GetFileReportQueryHandler> _logger;

    public GetFileReportQueryHandler(IFileRepository repository, ILogger<GetFileReportQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<FileReportDto> Handle(GetFileReportQuery request, CancellationToken cancellationToken)
    {
        var files = await _repository.GetAllAsync();
        var dto = new FileReportDto
        {
            TotalFilesProcessed = files.Count,
            AverageRowsProcessed = files.Any() ? files.Average(f => f.RowsProcessed) : 0
        };

        dto.Files = files.Select(f => new ProcessedFileSummaryDto
        {
            FileName = f.FileName,
            UploadedAt = f.UploadedAt,
            RowsProcessed = f.RowsProcessed,
            ProcessingTimeMilliseconds = f.ProcessingTimeMilliseconds
        }).ToList();

        _logger.LogInformation("Generated report for {Count} files", dto.TotalFilesProcessed);
        return dto;
    }
}
