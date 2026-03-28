using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [ApiController]
    public class CsiInspectorController : WaterSupplierCrudController<ProfessionalDto>
    {
        public CsiInspectorController(ICsiInspectorService service)
            : base(service)
        {
        }
    }
}
