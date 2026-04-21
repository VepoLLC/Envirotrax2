using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [HasFeature(FeatureType.CsiInspection)]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorWaterSuppliersController : WaterSupplierProtectedController
    {
        private readonly IProfessionalSupplierService _supplierService;

        public CsiInspectorWaterSuppliersController(IProfessionalSupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet("{id}/water-suppliers")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetWaterSuppliersAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _supplierService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }
    }
}
