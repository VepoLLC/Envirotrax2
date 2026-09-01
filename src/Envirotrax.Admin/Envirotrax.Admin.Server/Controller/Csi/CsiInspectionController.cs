
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Csi;

[Route("api/csi/inspections")]
public class CsiInspectionController : AdminBaseController
{
    private readonly ICsiInspectionService _inspectionService;

    public CsiInspectionController(ICsiInspectionService inspectionService)
    {
        _inspectionService = inspectionService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo,[FromQuery] Query query,[FromQuery] CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.SearchAsync(pageInfo, query, paymentStatus, cancellationToken);

        return Ok(inspections);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.GetAsync(id, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromQuery] int waterSupplierId, [FromBody] CsiInspectionUpdateRequest request, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.UpdateAsync(id, waterSupplierId, request, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpGet("{id}/counts")]
    public async Task<IActionResult> GetCountsAsync(int id, CancellationToken cancellationToken)
    {
        var counts = await _inspectionService.GetCountsAsync(id, cancellationToken);

        return Ok(counts);
    }

    [HttpGet("{id}/assemblies")]
    public async Task<IActionResult> GetAssembliesAsync(int id, CancellationToken cancellationToken)
    {
        var assemblies = await _inspectionService.GetAssembliesAsync(id, cancellationToken);

        return Ok(assemblies);
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _inspectionService.GetLogsAsync(id, cancellationToken);

        return Ok(logs);
    }

    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetImagesAsync(int id, CancellationToken cancellationToken)
    {
        var images = await _inspectionService.GetImagesAsync(id, cancellationToken);

        return Ok(images);
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImageAsync(int id, [FromQuery] int waterSupplierId, [FromForm] string? description, [FromForm] IFormFile image, CancellationToken cancellationToken)
    {
        await using var stream = image.OpenReadStream();

        var result = await _inspectionService.AddImageAsync(id, waterSupplierId, stream, image.FileName, description, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImageAsync(int id, int imageId, CancellationToken cancellationToken)
    {
        await _inspectionService.DeleteImageAsync(id, imageId, cancellationToken);

        return Ok();
    }
}
