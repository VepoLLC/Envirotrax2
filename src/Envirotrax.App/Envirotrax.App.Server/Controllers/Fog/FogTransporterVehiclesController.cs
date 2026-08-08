using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/transporters")]
    [HasFeature(FeatureType.FogTransportation)]
    [PermissionResource(PermissionType.FogTransporters)]
    public class FogTransporterVehiclesController : WaterSupplierProtectedController
    {
        private readonly IFogVehicleService _vehicleService;

        public FogTransporterVehiclesController(IFogVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [HttpGet("{id}/vehicles")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetVehiclesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _vehicleService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id}/vehicles")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> AddVehicleAsync([FromBody] FogVehicleDto dto)
        {
            var result = await _vehicleService.AddAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}/vehicles/{vehicleId}")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> UpdateVehicleAsync(int vehicleId, [FromBody] FogVehicleDto dto)
        {
            dto.Id = vehicleId;
            var result = await _vehicleService.UpdateAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id}/vehicles/{vehicleId}")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> DeleteVehicleAsync(int vehicleId)
        {
            await _vehicleService.DeleteAsync(vehicleId);
            return Ok();
        }
    }
}
