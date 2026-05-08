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
    public async Task<IActionResult> UpdateApprovalAsync(int id, [FromBody] CsiInspectionApprovalRequest request, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.UpdateApprovalAsync(id, request, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }
}
