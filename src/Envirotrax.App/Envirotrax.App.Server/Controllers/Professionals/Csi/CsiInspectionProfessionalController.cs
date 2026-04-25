using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Csi;

[Route("api/professionals/csi/inspections")]
[HasFeature(FeatureType.CsiInspection)]
[Authorize(Roles = RoleDefinitions.Professionals.CsiInspector)]
public class CsiInspectionProfessionalController : ProfessionalProtectedController
{
    private readonly ICsiInspectionService _inspectionService;

    public CsiInspectionProfessionalController(ICsiInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitAsync([FromBody] CsiInspectionDto request, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.SubmitAsync(request, cancellationToken);
        return Ok(result);
    }
}
