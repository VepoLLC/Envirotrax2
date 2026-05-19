using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow
{
    [Route("api/backflow/testers")]
    [HasFeature(FeatureType.BackflowTesting)]
    [PermissionResource(PermissionType.BackflowTesters)]
    public class BackflowTesterAccountInfoController : WaterSupplierProtectedController
    {
        private readonly IBackflowTesterService _testerService;

        public BackflowTesterAccountInfoController(IBackflowTesterService testerService)
        {
            _testerService = testerService;
        }

        [HttpGet("{id}/account-info")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetAccountInfoAsync(int id, CancellationToken cancellationToken)
        {
            var accountInfo = await _testerService.GetAsync(id, cancellationToken);

            if (accountInfo != null)
            {
                return Ok(accountInfo);
            }

            return NotFound();
        }
    }
}
