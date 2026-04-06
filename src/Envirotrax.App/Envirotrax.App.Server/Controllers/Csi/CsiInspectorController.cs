using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorController : WaterSupplierCrudController<ProfessionalDto>
    {
        private readonly ICsiInspectorService _inspectorService;

        public CsiInspectorController(ICsiInspectorService service)
            : base(service)
        {
            _inspectorService = service;
        }

        [HttpGet("{id}/details")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetDetailsAsync(int id, CancellationToken cancellationToken)
        {
            var details = await _inspectorService.GetDetailsAsync(id, cancellationToken);

            if (details != null)
            {
                return Ok(details);
            }

            return NotFound();
        }
    }
}
