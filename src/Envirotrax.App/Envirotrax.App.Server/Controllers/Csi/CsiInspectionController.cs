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

    public CsiInspectionController(ICsiInspectionService service)
        : base(service)
    {
        _inspectionService = service;
    }

    [HttpPut("{id}/approval")]
    [HasPermission(PermissionAction.CanEdit)]
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
}
