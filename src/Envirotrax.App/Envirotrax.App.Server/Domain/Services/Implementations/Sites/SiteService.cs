using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
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
    private readonly ISiteLogService _siteLogService;
    private readonly IGeocodingService _geocodingService;
    private readonly IGisAreaCoordinateRepository _coordinateRepository;
    private readonly ILogger<SiteService> _logger;

    public SiteService(
        IMapper mapper,
        ISiteRepository repository,
        ISiteLogService siteLogService,
        IGeocodingService geocodingService,
        IGisAreaCoordinateRepository coordinateRepository,
        ILogger<SiteService> logger)
        : base(mapper, repository)
    {
        _siteRepository = repository;
        _siteLogService = siteLogService;
        _geocodingService = geocodingService;
        _coordinateRepository = coordinateRepository;
        _logger = logger;
    }

    public async Task<IPagedData<SiteDto>> SearchAsync(PageInfo pageInfo, Query query, FogCompliancyStatus? fogCompliancyStatus, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        bool? fogCompliant = fogCompliancyStatus switch
        {
            FogCompliancyStatus.Compliant => true,
            FogCompliancyStatus.OutOfCompliance => false,
            _ => null
        };

        var sites = await _siteRepository.SearchAsync(pageInfo, query, fogCompliant, cancellationToken);

        return sites
            .Select(s => MapToDto(s)!)
            .ToPagedData(pageInfo);
    }

    public async Task<IEnumerable<SiteDto>> GetAllPendingGeocodingAsync(int batchSize)
    {
        var sites = await _siteRepository.GetAllPendingGeocodingAsync(batchSize);
        return Mapper.Map<IEnumerable<Site>, IEnumerable<SiteDto>>(sites);
    }

    private async Task AssignGisAreaAsync(Site site, GeocodingResponseDto coordinates, CancellationToken cancellationToken)
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

    public async Task<SiteDto?> GeocodeAsync(int siteId, bool assignGisArea, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetAsync(siteId, cancellationToken);

        if (site == null || site.DeletedTime.HasValue)
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

        try
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new InvalidOperationException($"Site has no address, so we can't geocode its location. WaterSupplierId: {site.WaterSupplierId}, SiteId: {site.Id}");
            }

            var coordinates = await _geocodingService.GeocodeAsync(address, cancellationToken);

            site.GisLatitude = coordinates.Latitude;
            site.GisLongitude = coordinates.Longitude;
            site.GisDate = DateTime.UtcNow;
            site.GisStatus = GisStatusType.Geocoded;

            if (assignGisArea)
            {
                await AssignGisAreaAsync(site, coordinates, cancellationToken);
            }

            await _siteRepository.UpdateGisCoordinatesAsync(site);
        }
        catch (Exception ex)
        {
            await HadnleGeocodingErrorAsync(ex, site);
        }

        return MapToDto(site);
    }

    public async Task UpdateGisDataAsync(int siteId, UpdateSiteGisDataDto dto, CancellationToken cancellationToken)
    {
        await _siteRepository.UpdateManualGisDataAsync(siteId, dto.Latitude, dto.Longitude, dto.Status);
    }

    public async Task<IPagedData<CsiComplianceSiteDto>> GetCsiComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        var sites = await _siteRepository.GetCsiComplianceAsync(pageInfo, query, cancellationToken);
        var dtos = sites.Select(s => Mapper.Map<CsiComplianceSiteDto>(s)).ToList();

        if (dtos.Count > 0)
        {
            var logs = await _siteLogService.GetBySitesAsync(dtos.Select(d => d.Id), cancellationToken);
            var logsBySite = logs.ToLookup(l => l.Site.Id ?? 0);

            foreach (var dto in dtos)
            {
                dto.Logs = logsBySite[dto.Id].ToList();
            }
        }

        return dtos.ToPagedData(pageInfo);
    }

    public async Task UpdateCsiAssignmentAsync(int siteId, int? userId)
    {
        var assignmentDate = userId.HasValue ? DateTime.UtcNow : (DateTime?)null;

        await _siteRepository.UpdateCsiAssignmentAsync(siteId, userId, assignmentDate);
    }

    public async Task<IEnumerable<SiteDto>> GetAllPendingRenewalAsync(int batchSize, CancellationToken cancellationToken)
    {
        var sites = await _siteRepository.GetAllPendingRenewalAsync(batchSize);
        return Mapper.Map<IEnumerable<Site>, IEnumerable<SiteDto>>(sites);
    }

    private async Task HadnleGeocodingErrorAsync(Exception ex, Site site)
    {
        _logger.LogError(ex, "Error goecoding site.");

        site.GisStatus = GisStatusType.Error;
        site.GisDate = DateTime.UtcNow;

        await _siteRepository.UpdateGisCoordinatesAsync(site);
    }
}
