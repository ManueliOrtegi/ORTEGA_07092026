using System;

namespace FileProcessor.Domain.Entities;

public sealed class ProcessedFile
{
    public Guid Id { get; private set; }
    public string FileName { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public int RowsProcessed { get; private set; }
    public double AverageSalary { get; private set; }
    public decimal HighestSalary { get; private set; }
    public decimal LowestSalary { get; private set; }
    public long ProcessingTimeMilliseconds { get; private set; }

    private ProcessedFile() { }

    public ProcessedFile(string fileName, DateTime uploadedAt, int rowsProcessed, double averageSalary, decimal highestSalary, decimal lowestSalary, long processingTimeMilliseconds)
    {
        Id = Guid.NewGuid();
        FileName = fileName ?? throw new ArgumentNullException(nameof(fileName));
        UploadedAt = uploadedAt;
        RowsProcessed = rowsProcessed;
        AverageSalary = averageSalary;
        HighestSalary = highestSalary;
        LowestSalary = lowestSalary;
        ProcessingTimeMilliseconds = processingTimeMilliseconds;
    }

    public void UpdateProcessingStats(int rows, double avg, decimal high, decimal low, long elapsedMs)
    {
        RowsProcessed = rows;
        AverageSalary = avg;
        HighestSalary = high;
        LowestSalary = low;
        ProcessingTimeMilliseconds = elapsedMs;
    }
}
