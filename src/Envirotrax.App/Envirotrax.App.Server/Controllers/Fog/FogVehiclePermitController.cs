using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/vehicles")]
    [HasFeature(FeatureType.FogTransportation)]
    [PermissionResource(PermissionType.FogVehicles)]
    public class FogVehiclePermitController : WaterSupplierProtectedController
    {
        private readonly IFogVehiclePermitService _permitService;

        public FogVehiclePermitController(IFogVehiclePermitService permitService)
        {
            _permitService = permitService;
        }

        [HttpGet]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _permitService.SearchAsync(pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpPut("{vehicleId}/permit")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> SetPermitAsync(int vehicleId, [FromBody] FogVehiclePermitDto dto, CancellationToken cancellationToken)
        {
            var result = await _permitService.SetPermitAsync(vehicleId, dto, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
