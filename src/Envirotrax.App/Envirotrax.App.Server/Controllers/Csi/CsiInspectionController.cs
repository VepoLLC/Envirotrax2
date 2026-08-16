using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi;

[Route("api/csi/inspections")]
[HasFeature(FeatureType.CsiInspection)]
[PermissionResource(PermissionType.CsiInspections)]
public class CsiInspectionController : WaterSupplierCrudController<CsiInspectionDto>
{
    private readonly ICsiInspectionService _inspectionService;
    private readonly ICsiInspectionImageService _imageService;

    public CsiInspectionController(ICsiInspectionService service, ICsiInspectionImageService imageService)
        : base(service)
    {
        _inspectionService = service;
        _imageService = imageService;
    }

    protected override Task<IPagedData<CsiInspectionDto>> ProcessGetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        return _inspectionService.SearchForWaterSupplierAsync(pageInfo, query, ReadSubAccountWaterSupplierId(), cancellationToken);
    }

    private int? ReadSubAccountWaterSupplierId()
    {
        if (int.TryParse(Request.Query["subAccountWaterSupplierId"], out var waterSupplierId))
        {
            return waterSupplierId;
        }

        return null;
    }

    [HttpGet("pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllPdfAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.GetAllAsync(pageInfo, query, cancellationToken);
        var pdfBytes = await _inspectionService.GeneratePdfAsync(inspections.Data);

        return File(pdfBytes, "application/pdf");
    }

    [HttpPut("{id}/approval")]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> UpdateApprovalAsync(int id, [FromBody] CsiInspectionApprovalRequest request, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.UpdateApprovalAsync(id, request, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}/pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetPdfAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.GetAsync(id, cancellationToken);
        if (inspection == null) return NotFound();

        var pdfBytes = await _inspectionService.GeneratePdfAsync(inspection);
        return File(pdfBytes, "application/pdf");
    }

    [HttpGet("{id}/images")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetImagesAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _imageService.GetByInspectionAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}/images/{imageId}")]
    [HasPermission(PermissionAction.CanDelete)]
    public async Task<IActionResult> DeleteImageAsync(int id, int imageId, CancellationToken cancellationToken)
    {
        var deleted = await _imageService.DeleteImageAsync(imageId, cancellationToken);
        return deleted ? Ok() : NotFound();
    }
}
