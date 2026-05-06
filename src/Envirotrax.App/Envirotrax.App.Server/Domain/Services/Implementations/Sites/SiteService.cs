using System.Net.Security;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public class SiteService : Service<Site, SiteDto>, ISiteService
{
    private readonly ISiteRepository _siteRepository;
    private readonly IGeocodingService _geocodingService;
    private readonly IGisAreaCoordinateRepository _coordinateRepository;

    public SiteService(
        IMapper mapper,
        ISiteRepository repository,
        IGeocodingService geocodingService,
        IGisAreaCoordinateRepository coordinateRepository)
        : base(mapper, repository)
    {
        _siteRepository = repository;
        _geocodingService = geocodingService;
        _coordinateRepository = coordinateRepository;
    }

    public async Task<IEnumerable<SiteDto>> GetAllPendingGeocodingAsync(int batchSize)
    {
        var sites = await _siteRepository.GetAllPendingGeocodingAsync(batchSize);
        return Mapper.Map<IEnumerable<Site>, IEnumerable<SiteDto>>(sites);
    }

    public async Task<SiteDto?> GeocodeAsync(int siteId, bool assignGisArea, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetAsync(siteId, cancellationToken);

        if (site == null)
        {
            return null;
        }

        var addressParts = new[]
        {
            site.StreetNumber,
            site.StreetName,
            site.City,
            site.State?.Code,
            site.ZipCode
        }.Where(s => !string.IsNullOrWhiteSpace(s));

        var address = string.Join(" ", addressParts);

        var coordinates = await _geocodingService.GeocodeAsync(address, cancellationToken);

        site.GisLatitude = coordinates.Latitude;
        site.GisLongitude = coordinates.Longitude;
        site.GisDate = DateTime.UtcNow;
        site.GisStatus = GisStatusType.Geocoded;

        if (assignGisArea)
        {
            var gisCoordiantesByArea = await _coordinateRepository.GetByPointAsync(coordinates.Longitude, coordinates.Latitude, cancellationToken);

            foreach (var group in gisCoordiantesByArea)
            {
                var gisPoints = group.Select(c => new CoordinateDto
                {
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                }).ToList();

                if (_geocodingService.IsPointInArea(gisPoints, coordinates))
                {
                    site.GisAreaId = group.Key;
                }
            }
        }

        return MapToDto(site);
    }
}
