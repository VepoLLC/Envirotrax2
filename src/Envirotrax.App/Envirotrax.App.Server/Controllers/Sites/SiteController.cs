using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites")]
[PermissionResource(PermissionType.Sites)]
public class SiteController : WaterSupplierCrudController<SiteDto>
{
    private readonly ISiteService _siteService;
    private readonly IRecordLogService _recordLogService;

    public SiteController(ISiteService service, IRecordLogService recordLogService)
        : base(service)
    {
        _siteService = service;
        _recordLogService = recordLogService;
    }

    [HttpPut("{id}/csi-assignment")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateCsiAssignmentAsync(int id, [FromBody] UpdateCsiAssignmentDto dto)
    {
        await _siteService.UpdateCsiAssignmentAsync(id, dto.UserId);
        return Ok();
    }

    [HttpPut("{id}/backflow-assignment")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateBackflowAssignmentAsync(int id, [FromBody] UpdateBackflowAssignmentDto dto)
    {
        await _siteService.UpdateBackflowAssignmentAsync(id, dto.UserId);
        return Ok();
    }

    [HttpPut("{id}/fog-assignment")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateFogAssignmentAsync(int id, [FromBody] UpdateFogAssignmentDto dto)
    {
        await _siteService.UpdateFogAssignmentAsync(id, dto.UserId);
        return Ok();
    }

    [HttpPut("{id}/gis-data")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateGisDataAsync(int id, [FromBody] UpdateSiteGisDataDto dto, CancellationToken cancellationToken)
    {
        await _siteService.UpdateGisDataAsync(id, dto, cancellationToken);
        return Ok();
    }

    [HttpGet("{id}/record-logs")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var site = await _siteService.GetAsync(id, cancellationToken);
        if (site == null)
        {
            return NotFound();
        }

        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.Sites, id, cancellationToken);
        return Ok(logs);
    }
}
