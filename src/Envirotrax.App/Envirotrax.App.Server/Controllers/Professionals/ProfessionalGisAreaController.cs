using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/gis-areas")]
public class ProfessionalGisAreaController : ProfessionalProtectedController
{
    private readonly IGisAreaService _gisAreaService;
    private readonly IGisAreaCoordinateService _coordinateService;

    public ProfessionalGisAreaController(IGisAreaService gisAreaService, IGisAreaCoordinateService coordinateService)
    {
        _gisAreaService = gisAreaService;
        _coordinateService = coordinateService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllAsync([FromQuery] int waterSupplierId, CancellationToken cancellationToken)
    {
        var areas = await _gisAreaService.GetAllActiveBySupplierAsync(waterSupplierId, cancellationToken);
        return Ok(areas);
    }

    [HttpGet("coordinates")]
    public async Task<IActionResult> GetAllCoordinatesAsync([FromQuery] int waterSupplierId, CancellationToken cancellationToken)
    {
        var coordinates = await _coordinateService.GetAllBySupplierAsync(waterSupplierId, cancellationToken);
        return Ok(coordinates);
    }

    [HttpGet("default-view")]
    public async Task<IActionResult> GetDefaultMapViewAsync([FromQuery] int waterSupplierId, CancellationToken cancellationToken)
    {
        var mapView = await _gisAreaService.GetDefaultMapViewBySupplierAsync(waterSupplierId, cancellationToken);
        return Ok(mapView);
    }
}
