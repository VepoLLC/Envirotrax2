using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Sites;

[Route("api/sites/{siteId}/site-logs")]
public class SiteLogController : AdminBaseController
{
    private readonly ISiteLogService _siteLogService;

    public SiteLogController(ISiteLogService siteLogService)
    {
        _siteLogService = siteLogService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBySiteAsync(int siteId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var logs = await _siteLogService.GetBySiteAsync(siteId, pageInfo, query, cancellationToken);

        return Ok(logs);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int siteId, int id, CancellationToken cancellationToken)
    {
        var log = await _siteLogService.GetAsync(siteId, id, cancellationToken);

        if (log == null)
        {
            return NotFound();
        }

        return Ok(log);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int siteId, [FromQuery] int waterSupplierId, [FromBody] SiteLogDto dto)
    {
        var log = await _siteLogService.AddAsync(siteId, waterSupplierId, dto);

        return Ok(log);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int siteId, int id, [FromQuery] int waterSupplierId, [FromBody] SiteLogDto dto)
    {
        var log = await _siteLogService.UpdateAsync(siteId, id, waterSupplierId, dto);

        return Ok(log);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int siteId, int id)
    {
        await _siteLogService.DeleteAsync(siteId, id);

        return Ok();
    }
}
