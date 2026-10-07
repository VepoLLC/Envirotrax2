using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/sites")]
public class ProfessionalSiteController : ProfessionalProtectedController
{
    private readonly ISiteService _siteService;
    private readonly ISiteScheduleService _siteScheduleService;

    public ProfessionalSiteController(ISiteService siteService, ISiteScheduleService siteScheduleService)
    {
        _siteService = siteService;
        _siteScheduleService = siteScheduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] ProfessionalSiteSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _siteService.SearchForProfessionalAsync(criteria, pageInfo, query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _siteService.GetAsync(id, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("{siteId}/schedule")]
    public async Task<IActionResult> GetSchedulesAsync(int siteId, CancellationToken cancellationToken)
    {
        var result = await _siteScheduleService.GetMyBySiteIdsAsync([siteId], cancellationToken);
        return Ok(result);
    }

    [HttpPut("{siteId}/schedule")]
    public async Task<IActionResult> SetScheduleAsync(int siteId, SiteScheduleDto schedule, CancellationToken cancellationToken)
    {
        var result = await _siteScheduleService.SetMyAsync(siteId, schedule, cancellationToken);

        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpDelete("{siteId}/schedule")]
    public async Task<IActionResult> ClearScheduleAsync(int siteId, [FromQuery] ProfessionalType professionalType)
    {
        await _siteScheduleService.ClearMyAsync(siteId, professionalType);
        return Ok();
    }
}
