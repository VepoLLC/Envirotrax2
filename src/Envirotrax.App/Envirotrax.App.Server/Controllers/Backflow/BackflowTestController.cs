using DeveloperPartners.SortingFiltering;
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

    [HttpGet("pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllPdfAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var tests = await _testService.GetAllAsync(pageInfo, query, cancellationToken);
        var pdf = await _testService.GeneratePdfAsync(tests.Data);
        return File(pdf, "application/pdf");
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdfAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _testService.GetAsync(id, cancellationToken);
        if (test == null)
        {
            return NotFound();
        }

        var pdf = await _testService.GeneratePdfAsync(test);
        return File(pdf, "application/pdf");
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
