using System;
using FileProcessor.Application.Interfaces;

namespace FileProcessor.Infrastructure.Services;

internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
