using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
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

    public SiteController(ISiteService service)
        : base(service)
    {
        _siteService = service;
    }

    [HttpGet("csi-compliance")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetCsiComplianceAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _siteService.GetCsiComplianceAsync(pageInfo, query, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}/csi-assignment")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateCsiAssignmentAsync(int id, [FromBody] UpdateCsiAssignmentDto dto, CancellationToken cancellationToken)
    {
        await _siteService.UpdateCsiAssignmentAsync(id, dto.UserId, cancellationToken);
        return Ok();
    }

    [HttpPut("{id}/gis-data")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateGisDataAsync(int id, [FromBody] UpdateSiteGisDataDto dto, CancellationToken cancellationToken)
    {
        await _siteService.UpdateGisDataAsync(id, dto, cancellationToken);
        return Ok();
    }
}
