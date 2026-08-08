using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/transporters")]
    [HasFeature(FeatureType.FogTransportation)]
    [PermissionResource(PermissionType.FogTransporters)]
    public class FogTransporterWaterSuppliersController : WaterSupplierProtectedController
    {
        private readonly IProfessionalSupplierService _supplierService;

        public FogTransporterWaterSuppliersController(IProfessionalSupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        [HttpGet("{id}/water-suppliers")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetWaterSuppliersAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _supplierService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken, pws => pws.HasFogTransportation);
            return Ok(result);
        }

        [HttpPut("{id}/water-suppliers/{supplierId}")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> UpdateWaterSupplierAsync([FromBody] ProfessionalWaterSupplierDto dto)
        {
            var result = await _supplierService.UpdateAsync(dto);
            return Ok(result);
        }
    }
}
