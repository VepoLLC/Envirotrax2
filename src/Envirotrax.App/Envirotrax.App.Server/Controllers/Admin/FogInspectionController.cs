using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// Route shells only. Every action calls an existing service method - the inherited GetAllAsync/GetAsync already
/// work cross-tenant because IDbContextSelector resolves AdminDbContext for the AdminInternal scope. The
/// water-supplier FOG routes are unreachable for an admin caller (different scope, plus the FogInspection
/// feature and permission gates), which is why this surface exists at all.
/// </summary>
[Route("api/admin/fog/inspections")]
public class FogInspectionController : AdminBaseController
{
    private readonly IFogInspectionService _inspectionService;
    private readonly IRecordLogService _recordLogService;

    public FogInspectionController(IFogInspectionService inspectionService, IRecordLogService recordLogService)
    {
        _inspectionService = inspectionService;
        _recordLogService = recordLogService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.GetAllAsync(pageInfo, query, cancellationToken);

        return Ok(inspections);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.GetAsync(id, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] FogInspectionAdminUpdateRequest request)
    {
        var inspection = await _inspectionService.UpdateForAdminAsync(id, request);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPost("{id}/images/{imageType}")]
    public async Task<IActionResult> UploadImageAsync(int id, string imageType, [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided.");
        }

        await using var stream = file.OpenReadStream();

        var inspection = await _inspectionService.UpdateImageForAdminAsync(id, imageType, stream, file.FileName);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.FogInspections, id, cancellationToken);

        return Ok(logs);
    }
}
