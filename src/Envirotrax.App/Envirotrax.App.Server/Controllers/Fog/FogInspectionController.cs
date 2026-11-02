using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
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
    private readonly IPdfLinkService _pdfLinkService;

    public FogInspectionController(IFogInspectionService service, IPdfLinkService pdfLinkService)
        : base(service)
    {
        _inspectionService = service;
        _pdfLinkService = pdfLinkService;
    }

    [HttpGet("pdf")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllPdfAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var inspections = await _inspectionService.GetAllAsync(pageInfo, query, cancellationToken);
        var pdfBytes = await _inspectionService.GeneratePdfAsync(inspections.Data);
        var link = await _pdfLinkService.CreateAsync(pdfBytes, "fog-inspections.pdf");

        return Ok(link);
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

        var pdfBytes = await _inspectionService.GeneratePdfAsync(inspection);
        var link = await _pdfLinkService.CreateAsync(pdfBytes, $"fog-inspection-{id}.pdf");

        return Ok(link);
    }
}
