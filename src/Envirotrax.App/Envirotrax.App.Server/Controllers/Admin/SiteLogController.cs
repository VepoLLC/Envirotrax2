using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/sites/{siteId}/site-logs")]
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
        var log = await _siteLogService.GetAsync(id, cancellationToken);

        if (log == null)
        {
            return NotFound();
        }

        return Ok(log);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int siteId, [FromBody] SiteLogDto dto)
    {
        var log = await _siteLogService.AddForAdminAsync(siteId, dto);

        if (log == null)
        {
            return NotFound();
        }

        return Ok(log);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int siteId, int id, [FromBody] SiteLogDto dto)
    {
        dto.Id = id;

        var log = await _siteLogService.UpdateAsync(dto, null, null);

        if (log == null)
        {
            return NotFound();
        }

        return Ok(log);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int siteId, int id)
    {
        var deleted = await _siteLogService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return Ok();
    }
}
