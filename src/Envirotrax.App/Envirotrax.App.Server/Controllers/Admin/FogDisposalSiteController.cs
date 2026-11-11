
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// Admin maintenance of the FOG disposal site master list. A route shell only - it reuses the existing
/// IFogDisposalSiteService that the professional disposal site page reads from. That endpoint
/// (api/professionals/fog/transportation/disposal-sites) is unreachable by an admin caller and read-only, so the
/// admin surface needs its own route, but not its own logic. Deleted sites are soft-deleted and dropped from
/// the list by the global QueryFilter.
/// </summary>
[Route("api/admin/fog/disposal-sites")]
public class FogDisposalSiteController : AdminBaseController
{
    private readonly IFogDisposalSiteService _disposalSiteService;
    private readonly IRecordLogService _recordLogService;

    public FogDisposalSiteController(IFogDisposalSiteService disposalSiteService, IRecordLogService recordLogService)
    {
        _disposalSiteService = disposalSiteService;
        _recordLogService = recordLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var sites = await _disposalSiteService.GetAllAsync(pageInfo, query, cancellationToken);

        return Ok(sites);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] FogDisposalSiteDto site)
    {
        site.Id = 0;

        var added = await _disposalSiteService.AddAsync(site);

        return Ok(added);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] FogDisposalSiteDto site, CancellationToken cancellationToken)
    {
        // Updating a missing row would surface as a concurrency exception, so check for it first.
        if (!await _disposalSiteService.ExistsAsync(id, cancellationToken))
        {
            return NotFound();
        }

        site.Id = id;

        var updated = await _disposalSiteService.UpdateAsync(site);

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var deleted = await _disposalSiteService.DeleteAsync(id);

        return deleted == null ? NotFound() : Ok();
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.FogDisposalSites, id, cancellationToken);

        return Ok(logs);
    }
}
