using System;

namespace FileProcessor.Application.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
