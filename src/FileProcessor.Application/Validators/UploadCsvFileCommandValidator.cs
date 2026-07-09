using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace FileProcessor.Application.Validators;

public sealed class UploadCsvFileCommandValidator : AbstractValidator<Microsoft.AspNetCore.Http.IFormFile>
{
    public UploadCsvFileCommandValidator()
    {
        RuleFor(f => f).NotNull().WithMessage("File is required");
        RuleFor(f => f.Length).GreaterThan(0).WithMessage("File is empty");
        RuleFor(f => f.FileName).Must(fn => fn != null && fn.EndsWith(".csv", System.StringComparison.OrdinalIgnoreCase)).WithMessage("Only CSV files are allowed");
        RuleFor(f => f.Length).LessThanOrEqualTo(5 * 1024 * 1024).WithMessage("File size exceeded");
    }
}
