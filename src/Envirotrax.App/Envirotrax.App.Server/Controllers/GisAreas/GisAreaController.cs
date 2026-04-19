using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.GisAreas;

[Route("api/gis-areas")]
[PermissionResource(PermissionType.Settings)]
public class GisAreaController : WaterSupplierCrudController<GisAreaDto>
{
    private readonly IGisAreaService _gisAreaService;
    private readonly IGisAreaCoordinateService _coordinateService;

    public GisAreaController(IGisAreaService service, IGisAreaService gisAreaService, IGisAreaCoordinateService coordinateService)
        : base(service)
    {
        _gisAreaService = gisAreaService;
        _coordinateService = coordinateService;
    }

    [HttpGet("coordinates")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetAllCoordinatesAsync(CancellationToken cancellationToken)
    {
        var coordinates = await _coordinateService.GetAllAsync(cancellationToken);
        return Ok(coordinates);
    }

    [HttpGet("default-view")]
    [HasPermission(PermissionAction.CanView)]
    public async Task<IActionResult> GetDefaultMapViewAsync(CancellationToken cancellationToken)
    {
        var mapView = await _gisAreaService.GetDefaultMapViewAsync(cancellationToken);
        return Ok(mapView);
    }

    [HttpPut("default-view")]
    [HasFeature(FeatureType.ManageGisAreas)]
    [HasPermission(PermissionAction.CanCreate | PermissionAction.CanEdit)]
    public async Task<IActionResult> UpdateDefaultMapViewAsync(DefaultGiisMapViewDto mapView)
    {
        var savedView = await _gisAreaService.UpdateDefaultMapViewAsync(mapView);
        return Ok(savedView);
    }

    [HasFeature(FeatureType.ManageGisAreas)]
    public override Task<IActionResult> AddAsync(GisAreaDto dto)
    {
        return base.AddAsync(dto);
    }

    [HasFeature(FeatureType.ManageGisAreas)]
    public override Task<IActionResult> UpdateAsync(int id, GisAreaDto dto)
    {
        return base.UpdateAsync(id, dto);
    }

    [HasFeature(FeatureType.ManageGisAreas)]
    public override Task<IActionResult> DeleteAsync(int id)
    {
        return base.DeleteAsync(id);
    }

    [HasFeature(FeatureType.ManageGisAreas)]
    public override Task<IActionResult> ReactivateAsync(int id)
    {
        return base.ReactivateAsync(id);
    }
}
