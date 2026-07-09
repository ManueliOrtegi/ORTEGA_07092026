using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileProcessor.Application.DTOs;
using FileProcessor.Application.Features.UploadCsvFile;
using FileProcessor.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace FileProcessor.Application.Tests;

/// <summary>
/// Unit tests for UploadCsvFileCommandHandler
/// Tests command handling, repository interaction, and CSV processing integration
/// </summary>
public class UploadHandlerTests
{
    private readonly Mock<IFileRepository> _repoMock;
    private readonly Mock<IFileProcessingService> _processorMock;
    private readonly IDateTimeProvider _dateTimeProvider;

    public UploadHandlerTests()
    {
        _repoMock = new Mock<IFileRepository>();
        _processorMock = new Mock<IFileProcessingService>();
        _dateTimeProvider = new TestDateTimeProvider();
    }

    [Fact]
    public async Task Handle_ValidCommand_ProcessesAndSavesFile()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("John,25,45000");

        var file = CreateMockFormFile(csv.ToString(), "test.csv");

        _processorMock
            .Setup(p => p.ProcessCsvAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync(new ProcessedFileDto
            {
                FileName = "test.csv",
                RowsProcessed = 1,
                AverageSalary = 45000,
                HighestSalary = 45000,
                LowestSalary = 45000,
                ProcessingTimeMilliseconds = 10,
                UploadedAt = DateTime.UtcNow
            });

        var handler = new UploadCsvFileCommandHandler(
            _processorMock.Object,
            _repoMock.Object,
            _dateTimeProvider,
            new NullLogger<UploadCsvFileCommandHandler>());

        var command = new UploadCsvFileCommand(file, 1024 * 1024);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("test.csv", result.FileName);
        Assert.Equal(1, result.RowsProcessed);
        Assert.Equal(45000, result.AverageSalary);

        // Verify repository was called
        _repoMock.Verify(r => r.AddAsync(It.IsAny<FileProcessor.Domain.Entities.ProcessedFile>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsProcessingService()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("Alice,30,60000");
        csv.AppendLine("Bob,35,70000");

        var file = CreateMockFormFile(csv.ToString(), "employees.csv");

        _processorMock
            .Setup(p => p.ProcessCsvAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync(new ProcessedFileDto
            {
                FileName = "employees.csv",
                RowsProcessed = 2,
                AverageSalary = 65000,
                HighestSalary = 70000,
                LowestSalary = 60000,
                ProcessingTimeMilliseconds = 15,
                UploadedAt = DateTime.UtcNow
            });

        var handler = new UploadCsvFileCommandHandler(
            _processorMock.Object,
            _repoMock.Object,
            _dateTimeProvider,
            new NullLogger<UploadCsvFileCommandHandler>());

        var command = new UploadCsvFileCommand(file, 5 * 1024 * 1024);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _processorMock.Verify(
            p => p.ProcessCsvAsync(
                It.IsAny<Stream>(),
                "employees.csv",
                5 * 1024 * 1024),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsProcessedFile()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("John,25,50000");

        var file = CreateMockFormFile(csv.ToString(), "data.csv");

        _processorMock
            .Setup(p => p.ProcessCsvAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync(new ProcessedFileDto
            {
                FileName = "data.csv",
                RowsProcessed = 1,
                AverageSalary = 50000,
                HighestSalary = 50000,
                LowestSalary = 50000,
                ProcessingTimeMilliseconds = 8,
                UploadedAt = DateTime.UtcNow
            });

        var handler = new UploadCsvFileCommandHandler(
            _processorMock.Object,
            _repoMock.Object,
            _dateTimeProvider,
            new NullLogger<UploadCsvFileCommandHandler>());

        var command = new UploadCsvFileCommand(file, 1024 * 1024);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        _repoMock.Verify(
            r => r.AddAsync(It.Is<FileProcessor.Domain.Entities.ProcessedFile>(
                pf => pf.FileName == "data.csv" && pf.RowsProcessed == 1)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_MultipleRows_CalculatesSalaryStats()
    {
        // Arrange
        var csv = new StringBuilder();
        csv.AppendLine("Name,Age,Salary");
        csv.AppendLine("Person1,25,40000");
        csv.AppendLine("Person2,30,50000");
        csv.AppendLine("Person3,35,60000");

        var file = CreateMockFormFile(csv.ToString(), "salaries.csv");

        _processorMock
            .Setup(p => p.ProcessCsvAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<long>()))
            .ReturnsAsync(new ProcessedFileDto
            {
                FileName = "salaries.csv",
                RowsProcessed = 3,
                AverageSalary = 50000,
                HighestSalary = 60000,
                LowestSalary = 40000,
                ProcessingTimeMilliseconds = 20,
                UploadedAt = DateTime.UtcNow
            });

        var handler = new UploadCsvFileCommandHandler(
            _processorMock.Object,
            _repoMock.Object,
            _dateTimeProvider,
            new NullLogger<UploadCsvFileCommandHandler>());

        var command = new UploadCsvFileCommand(file, 1024 * 1024);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(3, result.RowsProcessed);
        Assert.Equal(50000, result.AverageSalary);
        Assert.Equal(60000, result.HighestSalary);
        Assert.Equal(40000, result.LowestSalary);
    }

    /// <summary>
    /// Helper method to create a mock IFormFile for testing
    /// </summary>
    private IFormFile CreateMockFormFile(string content, string fileName)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.ContentType).Returns("text/csv");

        return fileMock.Object;
    }

    /// <summary>
    /// Test date/time provider for consistent test behavior
    /// </summary>
    private class TestDateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
