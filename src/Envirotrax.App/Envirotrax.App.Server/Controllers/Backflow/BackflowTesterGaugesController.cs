using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow
{
    [Route("api/backflow/testers")]
    [HasFeature(FeatureType.BackflowTesting)]
    [PermissionResource(PermissionType.BackflowTesters)]
    public class BackflowTesterGaugesController : WaterSupplierProtectedController
    {
        private readonly IBackflowGaugeService _gaugeService;

        public BackflowTesterGaugesController(IBackflowGaugeService gaugeService)
        {
            _gaugeService = gaugeService;
        }

        [HttpGet("{id}/gauges")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetGaugesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _gaugeService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }
    }
}
