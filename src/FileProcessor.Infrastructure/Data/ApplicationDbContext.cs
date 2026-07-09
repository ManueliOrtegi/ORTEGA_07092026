using FileProcessor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileProcessor.Infrastructure.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<ProcessedFile> ProcessedFiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProcessedFile>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.FileName).IsRequired();
        });
    }
}
