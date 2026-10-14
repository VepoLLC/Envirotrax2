
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/disposal-sites")]
public class FogDisposalSiteController : AdminBaseController
{
    private readonly IFogDisposalSiteService _disposalSiteService;

    public FogDisposalSiteController(IFogDisposalSiteService disposalSiteService)
    {
        _disposalSiteService = disposalSiteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var sites = await _disposalSiteService.GetAllAsync(pageInfo, query, cancellationToken);

        return Ok(sites);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] FogDisposalSiteDto site, CancellationToken cancellationToken)
    {
        var added = await _disposalSiteService.AddAsync(site, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] FogDisposalSiteDto site, CancellationToken cancellationToken)
    {
        var updated = await _disposalSiteService.UpdateAsync(id, site, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _disposalSiteService.DeleteAsync(id, cancellationToken);

        return Ok();
    }
}
