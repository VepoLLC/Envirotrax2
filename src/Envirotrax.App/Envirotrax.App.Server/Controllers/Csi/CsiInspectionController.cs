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
    public CsiInspectionController(ICsiInspectionService service)
        : base(service)
    {
    }
}
