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

    [HttpPut("{id}/gis-data")]
    public async Task<IActionResult> UpdateGisDataAsync(int id, [FromBody] UpdateSiteGisDataDto dto, CancellationToken cancellationToken)
    {
        await _siteService.UpdateGisDataAsync(id, dto, cancellationToken);
        return Ok();
    }
}
