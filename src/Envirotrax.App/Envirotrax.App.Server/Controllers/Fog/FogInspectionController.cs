using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/inspections")]
[HasFeature(FeatureType.FogInspection)]
[PermissionResource(PermissionType.FogInspections)]
public class FogInspectionController : WaterSupplierCrudController<FogInspectionDto>
{
    private readonly IFogInspectionService _inspectionService;

    public FogInspectionController(IFogInspectionService service)
        : base(service)
    {
        _inspectionService = service;
    }

    [HttpGet("pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllPdfAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.GetAllAsync(pageInfo, query, cancellationToken);
        var pdfBytes = await _inspectionService.GeneratePdfAsync(inspections.Data);

        return File(pdfBytes, "application/pdf");
    }
}
