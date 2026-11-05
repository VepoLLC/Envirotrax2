using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/transporters/{professionalId}/vehicles")]
public class FogTransporterVehicleController : AdminBaseController
{
    private readonly IFogTransporterVehicleService _vehicleService;

    public FogTransporterVehicleController(IFogTransporterVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetAllAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(vehicles);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] FogVehicleDto vehicle, CancellationToken cancellationToken)
    {
        var added = await _vehicleService.AddAsync(professionalId, vehicle, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{vehicleId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int vehicleId, [FromBody] FogVehicleDto vehicle, CancellationToken cancellationToken)
    {
        var updated = await _vehicleService.UpdateAsync(professionalId, vehicleId, vehicle, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int vehicleId, CancellationToken cancellationToken)
    {
        await _vehicleService.DeleteAsync(professionalId, vehicleId, cancellationToken);

        return Ok();
    }
}
