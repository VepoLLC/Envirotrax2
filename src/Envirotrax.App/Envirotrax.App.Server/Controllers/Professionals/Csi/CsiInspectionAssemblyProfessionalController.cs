using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Csi;

[Route("api/professionals/csi/inspections")]
[HasFeature(FeatureType.CsiInspection)]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.CsiInspector}")]
public class CsiInspectionAssemblyProfessionalController : ProfessionalProtectedController
{
    private readonly ICsiInspectionAssemblyService _assemblyService;

    public CsiInspectionAssemblyProfessionalController(ICsiInspectionAssemblyService assemblyService)
    {
        _assemblyService = assemblyService;
    }

    [HttpGet("site-assemblies/{siteId}")]
    public async Task<IActionResult> GetForSiteAsync(int siteId, CancellationToken cancellationToken)
    {
        var assemblies = await _assemblyService.GetForSiteAsync(siteId, cancellationToken);

        return Ok(assemblies);
    }

    [HttpGet("{inspectionId}/assemblies")]
    public async Task<IActionResult> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        var assemblies = await _assemblyService.GetByInspectionAsync(inspectionId, cancellationToken);

        return Ok(assemblies);
    }

    [HttpPut("{inspectionId}/assemblies")]
    public async Task<IActionResult> SaveAsync(int inspectionId, [FromBody] List<CsiInspectionAssemblyRequest> requests, CancellationToken cancellationToken)
    {
        var assemblies = await _assemblyService.SaveForProfessionalAsync(inspectionId, requests, cancellationToken);

        return assemblies == null ? NotFound() : Ok(assemblies);
    }
}
