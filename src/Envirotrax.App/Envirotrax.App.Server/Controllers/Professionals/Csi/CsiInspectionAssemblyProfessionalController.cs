using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
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

    [HttpPost("initialize")]
    public async Task<IActionResult> InitializeAsync([FromQuery] int siteId, [FromQuery] string submissionId, CancellationToken cancellationToken)
    {
        var assemblies = await _assemblyService.InitializeAsync(siteId, submissionId, cancellationToken);

        return Ok(assemblies);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync([FromBody] CsiInspectionAssemblyRequest request, CancellationToken cancellationToken)
    {
        var assembly = await _assemblyService.AddAsync(request, cancellationToken);

        return Ok(assembly);
    }

    [HttpPut("visually-identified")]
    public async Task<IActionResult> UpdateVisuallyIdentifiedAsync([FromBody] CsiInspectionVisuallyIdentifiedRequest request, CancellationToken cancellationToken)
    {
        await _assemblyService.UpdateVisuallyIdentifiedAsync(request, cancellationToken);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id, [FromQuery] string submissionId, CancellationToken cancellationToken)
    {
        var deleted = await _assemblyService.DeleteAsync(id, submissionId, cancellationToken);

        return deleted ? Ok() : NotFound();
    }
}
