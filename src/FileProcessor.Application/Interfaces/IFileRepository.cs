using FileProcessor.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileProcessor.Application.Interfaces;

public interface IFileRepository
{
    Task AddAsync(ProcessedFile file);
    Task<IReadOnlyList<ProcessedFile>> GetAllAsync();
    Task<int> CountAsync();
}
