
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/sites")]
public class SiteController : AdminBaseController
{
    private readonly ISiteService _siteService;
    private readonly IRecordLogService _recordLogService;

    public SiteController(ISiteService siteService, IRecordLogService recordLogService)
    {
        _siteService = siteService;
        _recordLogService = recordLogService;
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

    [HttpPut("{id}/water-supplier")]
    public async Task<IActionResult> UpdateWaterSupplierAsync(int id, [FromBody] UpdateSiteWaterSupplierDto dto)
    {
        var updated = await _siteService.UpdateWaterSupplierAsync(id, dto);

        if (!updated)
        {
            return NotFound();
        }

        return Ok();
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.Sites, id, cancellationToken);

        return Ok(logs);
    }
}
