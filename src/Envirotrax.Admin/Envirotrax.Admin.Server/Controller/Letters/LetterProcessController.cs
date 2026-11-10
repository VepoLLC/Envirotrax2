using System.ComponentModel.DataAnnotations;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Letters;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Letters;

[Route("api/letter-processes")]
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

    [HttpGet("{processId}/file-url")]
    public async Task<IActionResult> GetFileUrlAsync(string processId, [FromQuery, Required] string fileName, CancellationToken cancellationToken)
    {
        var url = await _letterProcessService.GetFileUrlAsync(processId, fileName, cancellationToken);

        if (url == null)
        {
            return NotFound();
        }

        return Ok(url);
    }

    [HttpDelete("{processId}")]
    public async Task<IActionResult> DeleteAsync(string processId)
    {
        await _letterProcessService.DeleteAsync(processId);

        return Ok();
    }
}
