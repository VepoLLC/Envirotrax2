using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.GisAreas;

[Route("api/gis-areas")]
[PermissionResource(PermissionType.Settings)]
public class GisAreaController : WaterSupplierCrudController<GisAreaDto>
{
    public GisAreaController(IGisAreaService service)
        : base(service)
    {
    }
}
