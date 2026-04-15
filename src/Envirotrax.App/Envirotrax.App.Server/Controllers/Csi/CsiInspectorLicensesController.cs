using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [HasFeature(FeatureType.CsiInspection)]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorLicensesController : WaterSupplierProtectedController
    {
        private readonly IProfessionalUserLicenseService _licenseService;

        public CsiInspectorLicensesController(IProfessionalUserLicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        [HttpGet("{id}/licenses")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetLicensesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _licenseService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }
    }
}
