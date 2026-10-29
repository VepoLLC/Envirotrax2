using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/inspections")]
public class FogInspectionController : AdminBaseController
{
    private readonly IFogInspectionService _inspectionService;

    public FogInspectionController(IFogInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.SearchAsync(pageInfo, query, cancellationToken);

        return Ok(inspections);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.GetAsync(id, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromQuery] int waterSupplierId, [FromBody] FogInspectionUpdateRequest request, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.UpdateAsync(id, waterSupplierId, request, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPut("{id}/validation-hold")]
    public async Task<IActionResult> UpdateValidationHoldAsync(int id, [FromQuery] int waterSupplierId, [FromBody] bool onHold, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.UpdateValidationHoldAsync(id, waterSupplierId, onHold, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPut("{id}/needs-validation")]
    public async Task<IActionResult> UpdateNeedsValidationAsync(int id, [FromQuery] int waterSupplierId, [FromBody] bool needsValidation, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.UpdateNeedsValidationAsync(id, waterSupplierId, needsValidation, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPost("{id}/images/{imageType}")]
    public async Task<IActionResult> UploadImageAsync(int id, string imageType, [FromQuery] int waterSupplierId, [FromForm] IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        await using var stream = file.OpenReadStream();

        var inspection = await _inspectionService.UploadImageAsync(id, waterSupplierId, imageType, stream, file.FileName, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _inspectionService.GetLogsAsync(id, cancellationToken);

        return Ok(logs);
    }
}
