
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
}
