
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/backflow/testers")]
public class BackflowTesterController : AdminBaseController
{
    private readonly IBackflowTesterService _testerService;

    public BackflowTesterController(IBackflowTesterService testerService)
    {
        _testerService = testerService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync(
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        [FromQuery] string? bpatLicenseNumber,
        [FromQuery] string? fireLicenseNumber,
        [FromQuery] string? insurancePolicyNumber,
        [FromQuery] string? userEmail,
        [FromQuery] string? contactName,
        [FromQuery] string? cellNumber,
        CancellationToken cancellationToken)
    {
        var testers = await _testerService.SearchAsync(
            bpatLicenseNumber,
            fireLicenseNumber,
            insurancePolicyNumber,
            userEmail,
            contactName,
            cellNumber,
            pageInfo,
            query,
            cancellationToken);

        return Ok(testers);
    }
}
