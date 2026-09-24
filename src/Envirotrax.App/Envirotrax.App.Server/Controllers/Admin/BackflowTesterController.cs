
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// Admin BPAT search. A route shell only - it reuses the existing IBackflowTesterService.SearchAsync that the
/// water-supplier window uses. That endpoint (api/backflow/testers/search) is unreachable by an admin caller
/// (different scope, role and feature gate), so the admin surface needs its own route, but not its own search.
/// IDbContextSelector already routes admin requests to AdminDbContext, which disables tenant filtering.
/// </summary>
[Route("api/admin/backflow/testers")]
public class BackflowTesterController : AdminBaseController
{
    private readonly IBackflowTesterService _testerService;

    public BackflowTesterController(IBackflowTesterService testerService)
    {
        _testerService = testerService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] BackflowTesterSearchDto criteria, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var testers = await _testerService.SearchAsync(criteria, pageInfo, query, cancellationToken);

        return Ok(testers);
    }
}
