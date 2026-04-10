using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorInsurancesController : WaterSupplierProtectedController
    {
        private readonly IProfessionalInsuranceService _insuranceService;

        public CsiInspectorInsurancesController(IProfessionalInsuranceService insuranceService)
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
    }
}
