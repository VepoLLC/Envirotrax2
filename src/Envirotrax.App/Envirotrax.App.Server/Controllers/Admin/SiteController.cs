
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/sites")]
public class SiteController : AdminBaseController
{
    private readonly ISiteService _siteService;

    public SiteController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] FogCompliancyStatus? fogCompliancyStatus, CancellationToken cancellationToken)
    {
        var sites = await _siteService.SearchAsync(pageInfo, query, fogCompliancyStatus, cancellationToken);

        return Ok(sites);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var site = await _siteService.GetAsync(id, cancellationToken);

        if (site == null)
        {
            return NotFound();
        }

        return Ok(site);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SiteDto dto, CancellationToken cancellationToken)
    {
        var updated = await _siteService.UpdateFromAdminAsync(id, dto, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpPut("{id}/gis-data")]
    public async Task<IActionResult> UpdateGisDataAsync(int id, [FromBody] UpdateSiteGisDataDto dto, CancellationToken cancellationToken)
    {
        await _siteService.UpdateGisDataAsync(id, dto, cancellationToken);

        return Ok();
    }
}
