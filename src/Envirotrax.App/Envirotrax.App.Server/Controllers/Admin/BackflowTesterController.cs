
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/testers")]
public class BackflowTesterController : AdminBaseController
{
    private readonly IBackflowTesterAccountService _testerService;

    public BackflowTesterController(IBackflowTesterAccountService testerService)
    {
        _testerService = testerService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] string? licenseNumber, [FromQuery] string? insuranceNumber, CancellationToken cancellationToken)
    {
        var accounts = await _testerService.SearchForAdminAsync(pageInfo, query, licenseNumber, insuranceNumber, cancellationToken);

        return Ok(accounts);
    }
}
