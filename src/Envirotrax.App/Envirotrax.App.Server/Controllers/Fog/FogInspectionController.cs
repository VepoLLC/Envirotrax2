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
    public FogInspectionController(IFogInspectionService service)
        : base(service)
    {
    }
}
