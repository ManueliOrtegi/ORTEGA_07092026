using System;

namespace FileProcessor.Application.DTOs;

public sealed class ProcessedFileDto
{
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }
    public int RowsProcessed { get; set; }
    public double AverageSalary { get; set; }
    public decimal HighestSalary { get; set; }
    public decimal LowestSalary { get; set; }
    public long ProcessingTimeMilliseconds { get; set; }
}
