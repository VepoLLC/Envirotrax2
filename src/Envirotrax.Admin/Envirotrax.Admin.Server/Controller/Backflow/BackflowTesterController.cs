
using DeveloperPartners.SortingFiltering;
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
        var criteria = new Dictionary<string, string>
        {
            ["bpatLicenseNumber"] = bpatLicenseNumber ?? string.Empty,
            ["fireLicenseNumber"] = fireLicenseNumber ?? string.Empty,
            ["insurancePolicyNumber"] = insurancePolicyNumber ?? string.Empty,
            ["userEmail"] = userEmail ?? string.Empty,
            ["contactName"] = contactName ?? string.Empty,
            ["cellNumber"] = cellNumber ?? string.Empty
        };

        var testers = await _testerService.SearchAsync(pageInfo, query, criteria, cancellationToken);

        return Ok(testers);
    }
}
