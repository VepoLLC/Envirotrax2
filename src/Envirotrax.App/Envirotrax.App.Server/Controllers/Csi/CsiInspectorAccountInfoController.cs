using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorAccountInfoController : WaterSupplierProtectedController
    {
        private readonly ICsiInspectorService _inspectorService;

        public CsiInspectorAccountInfoController(ICsiInspectorService inspectorService)
        {
            _inspectorService = inspectorService;
        }

        [HttpGet("{id}/account-info")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetAccountInfoAsync(int id, CancellationToken cancellationToken)
        {
            var accountInfo = await _inspectorService.GetAsync(id, cancellationToken);

            if (accountInfo != null)
            {
                return Ok(accountInfo);
            }

            return NotFound();
        }
    }
}
