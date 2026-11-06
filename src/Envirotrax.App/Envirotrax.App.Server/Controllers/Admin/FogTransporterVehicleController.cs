using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

/// <summary>
/// FogVehicle belongs to the professional, not to a water supplier, so the owner always comes from the route.
/// </summary>
[Route("api/admin/fog/transporters/{professionalId}/vehicles")]
public class FogTransporterVehicleController : AdminBaseController
{
    private readonly IFogVehicleService _vehicleService;

    public FogTransporterVehicleController(IFogVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleService.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(vehicles);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] FogVehicleDto vehicle)
    {
        vehicle.Professional = new ReferencedProfessionalDto
        {
            Id = professionalId
        };

        var added = await _vehicleService.AddAsync(vehicle);

        return Ok(added);
    }

    [HttpPut("{vehicleId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int vehicleId, [FromBody] FogVehicleDto vehicle, CancellationToken cancellationToken)
    {
        vehicle.Id = vehicleId;

        var updated = await _vehicleService.UpdateForProfessionalAsync(professionalId, vehicle, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{vehicleId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int vehicleId, CancellationToken cancellationToken)
    {
        var deleted = await _vehicleService.DeleteForProfessionalAsync(professionalId, vehicleId, cancellationToken);

        return deleted == null ? NotFound() : Ok();
    }
}
