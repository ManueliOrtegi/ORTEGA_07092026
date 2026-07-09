using System.Threading.Tasks;
using FileProcessor.Application.Features.UploadCsvFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessor.Api.Controllers;

[ApiController]
[Route("api/files")]
public sealed class FilesController : ControllerBase
{
    private readonly IMediator _mediator;
    public FilesController(IMediator mediator) => _mediator = mediator;

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        var command = new UploadCsvFileCommand(file, 5 * 1024 * 1024);
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("report")]
    public async Task<IActionResult> Report([FromServices] IMediator mediator)
    {
        var dto = await mediator.Send(new FileProcessor.Application.Features.GetFileReport.GetFileReportQuery());
        return Ok(dto);
    }
}
