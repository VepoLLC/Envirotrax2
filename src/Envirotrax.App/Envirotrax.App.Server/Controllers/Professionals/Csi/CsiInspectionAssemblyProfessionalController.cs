using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Csi;

[Route("api/professionals/csi/inspections/assemblies")]
[HasFeature(FeatureType.CsiInspection)]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.CsiInspector}")]
public class CsiInspectionAssemblyProfessionalController : ProfessionalProtectedController
{
    private readonly ICsiInspectionAssemblyService _assemblyService;

    public CsiInspectionAssemblyProfessionalController(ICsiInspectionAssemblyService assemblyService)
    {
        _assemblyService = assemblyService;
    }

    // The rows the inspection form starts with; they are saved with the inspection's submit/update.
    [HttpGet]
    public async Task<IActionResult> GetForFormAsync([FromQuery] int siteId, [FromQuery] int? inspectionId, CancellationToken cancellationToken)
    {
        var assemblies = await _assemblyService.GetForFormAsync(siteId, inspectionId, cancellationToken);

        return Ok(assemblies);
    }
}
