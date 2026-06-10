using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/tests")]
[PermissionResource(PermissionType.BackflowTests)]
public class BackflowTestController : WaterSupplierCrudController<BackflowTestDto>
{
    private readonly IBackflowTestService _testService;

    public BackflowTestController(IBackflowTestService service)
        : base(service)
    {
        _testService = service;
    }

    [HttpPost("{id}/images/{imageType}")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UploadImageAsync(int id, string imageType, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        await using var stream = file.OpenReadStream();
        var result = await _testService.UpdateImageAsync(id, imageType, stream, file.FileName, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}
