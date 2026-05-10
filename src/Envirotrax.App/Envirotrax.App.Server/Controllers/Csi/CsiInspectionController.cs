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
    private readonly ICsiInspectionPdfService _pdfService;

    public CsiInspectionController(ICsiInspectionService service, ICsiInspectionPdfService pdfService)
        : base(service)
    {
        _inspectionService = service;
        _pdfService = pdfService;
    }

    [HttpPut("{id}/approval")]
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
        if (inspection == null)
        {
            return NotFound();
        }

        var pdfBytes = await _pdfService.GenerateAsync(inspection);
        return File(pdfBytes, "application/pdf");
    }
}
