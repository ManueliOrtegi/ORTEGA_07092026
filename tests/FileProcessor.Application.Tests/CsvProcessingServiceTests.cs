using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using FileProcessor.Application.Interfaces;
using FileProcessor.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FileProcessor.Application.Tests;

/// <summary>
/// Unit tests for CsvProcessingService
/// Tests CSV parsing, salary calculations, and error handling
/// </summary>
public class CsvProcessingServiceTests
{
    private readonly IFileProcessingService _service;

    public CsvProcessingServiceTests()
    {
        _service = new CsvProcessingService(new TestDateTimeProvider(), new NullLogger<CsvProcessingService>());
    }

    [Fact]
    public async Task ProcessCsvAsync_ValidCsv_ReturnsCorrectStats()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("John,25,45000");
        csv.AppendLine("Mary,30,50000");
        csv.AppendLine("Alex,40,55000");

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act
        var result = await _service.ProcessCsvAsync(stream, "test.csv", 1024 * 1024);

        // Assert
        Assert.Equal(3, result.RowsProcessed);
        Assert.Equal(50000, result.AverageSalary); // (45000 + 50000 + 55000) / 3
        Assert.Equal(55000, result.HighestSalary);
        Assert.Equal(45000, result.LowestSalary);
        Assert.Equal("test.csv", result.FileName);
        Assert.NotEqual(0, result.ProcessingTimeMilliseconds);
    }

    [Fact]
    public async Task ProcessCsvAsync_SingleRow_CalculatesCorrectly()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("John,25,60000");

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act
        var result = await _service.ProcessCsvAsync(stream, "single.csv", 1024 * 1024);

        // Assert
        Assert.Equal(1, result.RowsProcessed);
        Assert.Equal(60000, result.AverageSalary);
        Assert.Equal(60000, result.HighestSalary);
        Assert.Equal(60000, result.LowestSalary);
    }

    [Fact]
    public async Task ProcessCsvAsync_MultipleSalaries_CalculatesAverageCorrectly()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("Alice,28,100000");
        csv.AppendLine("Bob,35,80000");
        csv.AppendLine("Charlie,42,120000");

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act
        var result = await _service.ProcessCsvAsync(stream, "salaries.csv", 1024 * 1024);

        // Assert
        Assert.Equal(3, result.RowsProcessed);
        Assert.Equal(100000, result.AverageSalary); // (100000 + 80000 + 120000) / 3
        Assert.Equal(120000, result.HighestSalary);
        Assert.Equal(80000, result.LowestSalary);
    }

    [Fact]
    public async Task ProcessCsvAsync_EmptySalaryValue_ThrowsInvalidDataException()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("John,25,"); // Missing salary

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _service.ProcessCsvAsync(stream, "invalid.csv", 1024 * 1024));
    }

    [Fact]
    public async Task ProcessCsvAsync_MissingSalaryColumn_ThrowsInvalidDataException()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age"); // No Salary column
        csv.AppendLine("John,25");

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _service.ProcessCsvAsync(stream, "invalid.csv", 1024 * 1024));
    }

    [Fact]
    public async Task ProcessCsvAsync_InvalidSalaryValue_ThrowsInvalidDataException()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("John,25,notanumber");

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _service.ProcessCsvAsync(stream, "invalid.csv", 1024 * 1024));
    }

    [Fact]
    public async Task ProcessCsvAsync_NoDataRows_ThrowsInvalidDataException()
    {
        // Arrange
        var csv = "Name,Age,Salary\r\n"; // Only header, no data

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() =>
            _service.ProcessCsvAsync(stream, "empty.csv", 1024 * 1024));
    }

    [Fact]
    public async Task ProcessCsvAsync_RecordsProcessingTime()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("John,25,45000");

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv.ToString()));

        // Act
        var result = await _service.ProcessCsvAsync(stream, "test.csv", 1024 * 1024);

        // Assert
        Assert.True(result.ProcessingTimeMilliseconds >= 0);
    }

    /// <summary>
    /// Test date/time provider for consistent test behavior
    /// </summary>
    private class TestDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
