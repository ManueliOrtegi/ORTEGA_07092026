using MediatR;
using FileProcessor.Application.DTOs;
using System.Collections.Generic;

namespace FileProcessor.Application.Features.GetFileReport;

public sealed class GetFileReportQuery : IRequest<FileReportDto>
{
}

public sealed class FileReportDto
{
    public int TotalFilesProcessed { get; set; }
    public double AverageRowsProcessed { get; set; }
    public List<ProcessedFileSummaryDto> Files { get; set; } = new();
}

public sealed class ProcessedFileSummaryDto
{
    public string FileName { get; set; } = string.Empty;
    public System.DateTime UploadedAt { get; set; }
    public int RowsProcessed { get; set; }
    public long ProcessingTimeMilliseconds { get; set; }
}
