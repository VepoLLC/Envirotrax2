using AutoMapper;
using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Domain.DataTransferObjects.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;

namespace Envirotrax.App.Server.Domain.Services.Implementations.GisAreas;

public class GisAreaService : Service<GisArea, GisAreaDto>, IGisAreaService
{
    private readonly IGisAreaRepository _gisAreaRepository;

    public GisAreaService(IMapper mapper, IGisAreaRepository repository)
        : base(mapper, repository)
    {
        _gisAreaRepository = repository;
    }

    public async Task<DefaultGiisMapViewDto> GetDefaultMapViewAsync(CancellationToken cancellationToken)
    {
        var mapView = await _gisAreaRepository.GetDefaultMapViewAsync(cancellationToken);

        return new DefaultGiisMapViewDto
        {
            GisCenterLatitude = mapView.GisCenterLatitude,
            GisCenterLongitude = mapView.GisCenterLongitude,
            GisCenterZoom = mapView.GisCenterZoom
        };
    }

    public async Task<DefaultGiisMapViewDto> UpdateDefaultMapViewAsync(DefaultGiisMapViewDto mapView)
    {
        await _gisAreaRepository.UpdateDefaultMapViewAsync(new DefaultGisMapView
        {
            GisCenterLatitude = mapView.GisCenterLatitude,
            GisCenterLongitude = mapView.GisCenterLongitude,
            GisCenterZoom = mapView.GisCenterZoom
        });

        return mapView;
    }
}
