using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow
{
    [Route("api/backflow/testers")]
    [PermissionResource(PermissionType.BackflowTesters)]
    public class BackflowTesterController : WaterSupplierCrudController<ProfessionalDto>
    {
        public BackflowTesterController(IBackflowTesterService service)
            : base(service)
        {
        }
    }
}
