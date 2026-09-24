
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/testers")]
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
