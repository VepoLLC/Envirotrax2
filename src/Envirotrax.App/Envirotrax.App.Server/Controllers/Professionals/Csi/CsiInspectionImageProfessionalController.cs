using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Csi;

[Route("api/professionals/csi/inspections/{inspectionId}/images")]
[HasFeature(FeatureType.CsiInspection)]
[Authorize(Roles = RoleDefinitions.Professionals.CsiInspector)]
public class CsiInspectionImageProfessionalController : ProfessionalProtectedController
{
    private readonly ICsiInspectionImageService _service;

    public CsiInspectionImageProfessionalController(ICsiInspectionImageService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        var result = await _service.GetByInspectionAsync(inspectionId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddImageAsync(
        int inspectionId,
        [FromForm] string? description,
        [FromForm] IFormFile image,
        CancellationToken cancellationToken)
    {
        await using var stream = image.OpenReadStream();
        var result = await _service.AddImageAsync(inspectionId, description, stream, image.FileName, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{imageId}")]
    public async Task<IActionResult> DeleteImageAsync(int inspectionId, int imageId, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteImageAsync(imageId, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
