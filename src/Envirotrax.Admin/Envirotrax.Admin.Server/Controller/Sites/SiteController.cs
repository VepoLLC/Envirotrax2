

using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Sites;

[Route("api/sites")]
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
    public async Task<IActionResult> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var site = await _siteService.GetByIdAsync(id, cancellationToken);

        if (site == null)
        {
            return NotFound();
        }

        return Ok(site);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] SiteUpdateDto dto, CancellationToken cancellationToken)
    {
        await _siteService.UpdateAsync(id, dto, cancellationToken);

        return Ok();
    }

    [HttpPut("{id}/gis-data")]
    public async Task<IActionResult> UpdateGisAsync(int id, [FromBody] SiteGisUpdateDto dto, CancellationToken cancellationToken)
    {
        await _siteService.UpdateGisAsync(id, dto, cancellationToken);

        return Ok();
    }
}
