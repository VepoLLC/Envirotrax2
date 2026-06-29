using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _fogInspectionService.GetAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAsync(
        [FromForm] FogInspectionDto dto,
        [FromForm] IFormFile? exteriorImage,
        [FromForm] IFormFile? interiorImage,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        dto.Id = 0;

        await using var exteriorStream = exteriorImage?.OpenReadStream();
        await using var interiorStream = interiorImage?.OpenReadStream();

        var result = await _fogInspectionService.SubmitAsync(
            dto,
            exteriorStream, exteriorImage?.FileName,
            interiorStream, interiorImage?.FileName,
            cancellationToken);

        return Ok(result);
    }
}
