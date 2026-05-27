using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/inspectors")]
    [HasFeature(FeatureType.FogInspection)]
    [PermissionResource(PermissionType.FogInspectors)]
    public class FogInspectorAccountInfoController : WaterSupplierProtectedController
    {
        private readonly IFogInspectorService _inspectorService;

        public FogInspectorAccountInfoController(IFogInspectorService inspectorService)
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
