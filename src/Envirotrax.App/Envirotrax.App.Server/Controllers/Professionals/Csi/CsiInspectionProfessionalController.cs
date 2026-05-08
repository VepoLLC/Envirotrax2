using DeveloperPartners.SortingFiltering;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.GetAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }
        
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetProfessionalInspectionsAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] bool latestOnly, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
        return Ok(result);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitAsync([FromBody] CsiInspectionDto request, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.SubmitAsync(request, cancellationToken);
        return Ok(result);
    }
}
