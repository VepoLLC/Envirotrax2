
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TaskRunner;

[Route("api/task-runner/sites")]
public class SiteController : TaskRunnerBaseContoller
{
    private readonly ISiteService _siteService;

    public SiteController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpGet("geocode/pending")]
    public async Task<IActionResult> GetAllPendingGeocodingAsync([FromQuery] int batchSize)
    {
        var sites = await _siteService.GetAllPendingGeocodingAsync(batchSize);
        return Ok(sites);
    }
}