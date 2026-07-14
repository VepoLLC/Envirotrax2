using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog;

[Route("api/fog/trip-tickets")]
[PermissionResource(PermissionType.FogTripTickets)]
public class FogTripTicketController : WaterSupplierCrudController<FogTripTicketDto>
{
    public FogTripTicketController(IFogTripTicketService service)
        : base(service)
    {
    }
}
