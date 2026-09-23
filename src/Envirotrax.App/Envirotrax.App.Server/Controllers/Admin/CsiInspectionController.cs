
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/csi/inspections")]
public class CsiInspectionController : AdminBaseController
{
    private readonly ICsiInspectionService _inspectionService;
    private readonly ICsiInspectionImageService _imageService;
    private readonly ICsiInspectionAssemblyService _assemblyService;
    private readonly IRecordLogService _recordLogService;

    public CsiInspectionController(
        ICsiInspectionService inspectionService,
        ICsiInspectionImageService imageService,
        ICsiInspectionAssemblyService assemblyService,
        IRecordLogService recordLogService)
    {
        _inspectionService = inspectionService;
        _imageService = imageService;
        _assemblyService = assemblyService;
        _recordLogService = recordLogService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.SearchForAdminAsync(pageInfo, query, paymentStatus, cancellationToken);

        return Ok(inspections);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.GetAsync(id, cancellationToken);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CsiInspectionAdminUpdateRequest request)
    {
        var inspection = await _inspectionService.UpdateForAdminAsync(id, request);

        return inspection == null ? NotFound() : Ok(inspection);
    }

    [HttpGet("{id}/counts")]
    public async Task<IActionResult> GetCountsAsync(int id, CancellationToken cancellationToken)
    {
        var assemblyCount = await _assemblyService.GetCountByInspectionAsync(id, cancellationToken);
        var recordLogCount = await _recordLogService.GetCountByRecordAsync(RecordLogTableNames.CsiInspections, id, cancellationToken);

        return Ok(new CsiInspectionCountsDto
        {
            AssemblyCount = assemblyCount,
            RecordLogCount = recordLogCount
        });
    }

    [HttpGet("{id}/assemblies")]
    public async Task<IActionResult> GetAssembliesAsync(int id, CancellationToken cancellationToken)
    {
        var assemblies = await _assemblyService.GetByInspectionAsync(id, cancellationToken);

        return Ok(assemblies);
    }

    [HttpGet("{id}/logs")]
    public async Task<IActionResult> GetLogsAsync(int id, CancellationToken cancellationToken)
    {
        var logs = await _recordLogService.GetByRecordAsync(RecordLogTableNames.CsiInspections, id, cancellationToken);

        return Ok(logs);
    }

    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetImagesAsync(int id, CancellationToken cancellationToken)
    {
        var images = await _imageService.GetByInspectionAsync(id, cancellationToken);

        return Ok(images);
    }

    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImageAsync(int id, [FromForm] string? description, [FromForm] IFormFile image, CancellationToken cancellationToken)
    {
        await using var stream = image.OpenReadStream();

        var result = await _imageService.AddImageAsync(id, description, stream, image.FileName, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id}/images/{imageId}")]
    public async Task<IActionResult> DeleteImageAsync(int id, int imageId, CancellationToken cancellationToken)
    {
        var deleted = await _imageService.DeleteImageAsync(imageId, cancellationToken);

        return deleted ? Ok() : NotFound();
    }
}
