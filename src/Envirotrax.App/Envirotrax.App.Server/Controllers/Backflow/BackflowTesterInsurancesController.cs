using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow
{
    [Route("api/backflow/testers")]
    [HasFeature(FeatureType.BackflowTesting)]
    [PermissionResource(PermissionType.BackflowTesters)]
    public class BackflowTesterInsurancesController : WaterSupplierProtectedController
    {
        private readonly IProfessionalInsuranceService _insuranceService;

        public BackflowTesterInsurancesController(IProfessionalInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        [HttpGet("{id}/insurances")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetInsurancesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _insuranceService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/insurances/{insuranceId}/file-url")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetInsuranceFileUrlAsync(int insuranceId, CancellationToken cancellationToken)
        {
            var url = await _insuranceService.GenerateFileUrlAsync(insuranceId, cancellationToken);

            if (url != null)
            {
                return Ok(url);
            }

            return NotFound();
        }
    }
}
