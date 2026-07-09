using System.Collections.Generic;
using System.Threading.Tasks;
using FileProcessor.Application.Interfaces;
using FileProcessor.Domain.Entities;
using FileProcessor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FileProcessor.Infrastructure.Repositories;

internal sealed class FileRepository : IFileRepository
{
    private readonly ApplicationDbContext _db;

    public FileRepository(ApplicationDbContext db) => _db = db;

    public async Task AddAsync(ProcessedFile file)
    {
        await _db.ProcessedFiles.AddAsync(file);
        await _db.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<ProcessedFile>> GetAllAsync()
    {
        return await _db.ProcessedFiles.AsNoTracking().ToListAsync();
    }

    public Task<int> CountAsync() => _db.ProcessedFiles.CountAsync();
}
