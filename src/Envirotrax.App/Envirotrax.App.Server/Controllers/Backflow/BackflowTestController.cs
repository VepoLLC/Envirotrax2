using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow;

[Route("api/backflow/tests")]
[PermissionResource(PermissionType.BackflowTests)]
public class BackflowTestController : WaterSupplierCrudController<BackflowTestDto>
{
    public BackflowTestController(IBackflowTestService service)
        : base(service)
    {
    }
}
