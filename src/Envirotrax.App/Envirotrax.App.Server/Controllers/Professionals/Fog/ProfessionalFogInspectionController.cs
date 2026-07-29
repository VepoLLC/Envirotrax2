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

    [HttpGet("pdf")]
    public async Task<IActionResult> GetAllPdfAsync(
        [FromQuery] PageInfo pageInfo, [FromQuery] Query query,
        [FromQuery] bool latestOnly = true, CancellationToken cancellationToken = default)
    {
        var inspections = await _fogInspectionService.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
        var pdf = await _fogInspectionService.GeneratePdfAsync(inspections.Data);
        return File(pdf, "application/pdf");
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

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdfAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _fogInspectionService.GetAsync(id, cancellationToken);
        if (inspection == null)
        {
            return NotFound();
        }

        var pdf = await _fogInspectionService.GeneratePdfAsync(inspection);
        return File(pdf, "application/pdf");
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAsync(
        [FromForm] FogInspectionDto dto,
        [FromForm] IFormFile? exteriorImage,
        [FromForm] IFormFile? interiorImage,
        [FromForm] IFormFile? signatureImage,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        dto.Id = 0;

        await using var exteriorStream = exteriorImage?.OpenReadStream();
        await using var interiorStream = interiorImage?.OpenReadStream();
        await using var signatureStream = signatureImage?.OpenReadStream();

        var result = await _fogInspectionService.SubmitAsync(
            dto,
            exteriorStream, exteriorImage?.FileName,
            interiorStream, interiorImage?.FileName,
            signatureStream, signatureImage?.FileName,
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _fogInspectionService.DeleteAsync(id);
        return result == null ? NotFound() : Ok(result);
    }
}
