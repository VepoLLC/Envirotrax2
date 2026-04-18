using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.GisAreas;

[Route("api/gis-areas/{areaId}/coordinates")]
public class GisAreaCoordinateController : WaterSupplierProtectedController
{
    private readonly IGisAreaCoordinateService _coordinateService;

    public GisAreaCoordinateController(IGisAreaCoordinateService service)
    {
        _coordinateService = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetByAreaIdAsync(int areaId, CancellationToken cancellationToken)
    {
        var result = await _coordinateService.GetByAreaIdAsync(areaId, cancellationToken);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> AddOrUpdateAsync(int areaId, IEnumerable<GisAreaCoordinateDto> coordinates)
    {
        var result = await _coordinateService.AddOrUpdateAsync(areaId, coordinates);
        return Ok(result);
    }
}
