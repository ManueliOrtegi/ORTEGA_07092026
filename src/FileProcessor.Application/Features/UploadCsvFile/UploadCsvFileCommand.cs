using MediatR;
using Microsoft.AspNetCore.Http;
using FileProcessor.Application.DTOs;

namespace FileProcessor.Application.Features.UploadCsvFile;

public sealed class UploadCsvFileCommand : IRequest<ProcessedFileDto>
{
    public IFormFile File { get; }
    public long MaxFileSize { get; }

    public UploadCsvFileCommand(IFormFile file, long maxFileSize)
    {
        File = file;
        MaxFileSize = maxFileSize;
    }
}
