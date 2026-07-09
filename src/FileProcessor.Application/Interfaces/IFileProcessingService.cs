using FileProcessor.Application.DTOs;
using System.IO;
using System.Threading.Tasks;

namespace FileProcessor.Application.Interfaces;

public interface IFileProcessingService
{
    Task<ProcessedFileDto> ProcessCsvAsync(Stream fileStream, string fileName, long maxFileSize);
}
