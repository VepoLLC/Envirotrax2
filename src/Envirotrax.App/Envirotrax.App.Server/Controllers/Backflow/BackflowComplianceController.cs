using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

// Backflow Compliance Management: a paged list of expired assemblies (backflow tests) joined to their
// sites, grouped by site on the client. Mirrors CsiComplianceController but is assembly-rooted.
[Route("api/backflow/compliance")]
[HasFeature(FeatureType.BackflowTesting)]
[PermissionResource(PermissionType.BackflowReports)]
public class BackflowComplianceController : WaterSupplierProtectedController
{
    private readonly IBackflowTestService _backflowTestService;

    public BackflowComplianceController(IBackflowTestService backflowTestService)
    {
        _backflowTestService = backflowTestService;
    }

    [HttpGet]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetComplianceAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _backflowTestService.GetComplianceAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }
}
