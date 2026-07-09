using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using FileProcessor.Application.Interfaces;
using FileProcessor.Application.DTOs;

namespace FileProcessor.Application.Features.UploadCsvFile;

internal sealed class UploadCsvFileCommandHandler : IRequestHandler<UploadCsvFileCommand, ProcessedFileDto>
{
    private readonly IFileProcessingService _processor;
    private readonly IFileRepository _repository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<UploadCsvFileCommandHandler> _logger;

    public UploadCsvFileCommandHandler(IFileProcessingService processor, IFileRepository repository, IDateTimeProvider dateTimeProvider, ILogger<UploadCsvFileCommandHandler> logger)
    {
        _processor = processor;
        _repository = repository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ProcessedFileDto> Handle(UploadCsvFileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Upload started for {FileName}", request.File.FileName);
        await using var stream = request.File.OpenReadStream();
        var dto = await _processor.ProcessCsvAsync(stream, request.File.FileName, request.MaxFileSize);

        // persist
        var entity = new FileProcessor.Domain.Entities.ProcessedFile(dto.FileName, dto.UploadedAt, dto.RowsProcessed, dto.AverageSalary, dto.HighestSalary, dto.LowestSalary, dto.ProcessingTimeMilliseconds);
        await _repository.AddAsync(entity);

        _logger.LogInformation("Upload completed for {FileName} processed {Rows} rows", dto.FileName, dto.RowsProcessed);
        return dto;
    }
}
