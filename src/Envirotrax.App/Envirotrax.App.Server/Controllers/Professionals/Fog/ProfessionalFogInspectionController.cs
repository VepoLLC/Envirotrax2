using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Fog;

[Route("api/professionals/fog/inspections")]
[HasFeature(FeatureType.FogInspection)]
[Authorize(Roles = RoleDefinitions.Professionals.FogInspector)]
public class ProfessionalFogInspectionController : ProfessionalProtectedController
{
    private readonly IFogInspectionService _fogInspectionService;

    public ProfessionalFogInspectionController(IFogInspectionService fogInspectionService)
    {
        _fogInspectionService = fogInspectionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(
        [FromQuery] PageInfo pageInfo, [FromQuery] Query query,
        [FromQuery] bool latestOnly = true, CancellationToken cancellationToken = default)
    {
        var result = await _fogInspectionService.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
        return Ok(result);
    }
}
