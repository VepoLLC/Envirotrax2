using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Domain.Services.Definitions.Letters;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/letter-processes")]
public class LetterProcessController : AdminBaseController
{
    private readonly ILetterProcessService _letterProcessService;

    public LetterProcessController(ILetterProcessService letterProcessService)
    {
        _letterProcessService = letterProcessService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var processes = await _letterProcessService.GetAllAsync(cancellationToken);

        return Ok(processes);
    }

    [HttpGet("{processId}/files")]
    public async Task<IActionResult> GetFilesAsync(string processId, CancellationToken cancellationToken)
    {
        var files = await _letterProcessService.GetFilesAsync(processId, cancellationToken);

        if (files == null)
        {
            return NotFound();
        }

        return Ok(files);
    }

    [HttpGet("{processId}/emails")]
    public async Task<IActionResult> GetEmailsAsync(string processId, CancellationToken cancellationToken)
    {
        var emails = await _letterProcessService.GetEmailsAsync(processId, cancellationToken);

        if (emails == null)
        {
            return NotFound();
        }

        return Ok(emails);
    }

    [HttpGet("{processId}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(string processId, [FromQuery, Required] string fileName, CancellationToken cancellationToken)
    {
        var url = await _letterProcessService.GenerateFileUrlAsync(processId, fileName, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }

    [HttpDelete("{processId}")]
    public async Task<IActionResult> DeleteAsync(string processId)
    {
        var isDeleted = await _letterProcessService.DeleteAsync(processId);

        if (!isDeleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
