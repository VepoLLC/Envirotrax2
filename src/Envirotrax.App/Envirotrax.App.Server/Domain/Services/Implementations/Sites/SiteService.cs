using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public class SiteService : Service<Site, SiteDto>, ISiteService
{
    private readonly ISiteRepository _siteRepository;
    private readonly ISiteLogService _siteLogService;
    private readonly IRecordLogService _recordLogService;
    private readonly IGeocodingService _geocodingService;
    private readonly IGisAreaCoordinateRepository _coordinateRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly ILogger<SiteService> _logger;

    public SiteService(
        IMapper mapper,
        ISiteRepository repository,
        ISiteLogService siteLogService,
        IRecordLogService recordLogService,
        IGeocodingService geocodingService,
        IGisAreaCoordinateRepository coordinateRepository,
        ITimeZoneHelperService timeZoneHelper,
        ILogger<SiteService> logger)
        : base(mapper, repository)
    {
        _siteRepository = repository;
        _siteLogService = siteLogService;
        _recordLogService = recordLogService;
        _geocodingService = geocodingService;
        _coordinateRepository = coordinateRepository;
        _timeZoneHelper = timeZoneHelper;
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

    public async Task<bool> UpdateFromAdminAsync(int siteId, SiteDto dto, CancellationToken cancellationToken)
    {
        var site = await _siteRepository.GetTrackedForUpdateAsync(siteId, cancellationToken);

        if (site == null)
        {
            return false;
        }

        ApplyAdminUpdate(site, dto);

        await _siteRepository.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Copies the approved editable fields from a SiteDto onto the loaded (tracked) Site — a deliberate
    /// ALLOWLIST, so protected DTO columns (WaterSupplier, GIS, audit, NeedsRenewalCheck, …) are ignored.
    /// Runs on the freshly-loaded entity so the NeedsRenewalCheck compare below sees the pre-overwrite values.
    /// </summary>
    private static void ApplyAdminUpdate(Site site, SiteDto dto)
    {
        var renewalTriggerChanged =
            site.PropertyType != dto.PropertyType
            || site.HasOnSiteSewageFacility != dto.HasOnSiteSewageFacility
            || site.HasAuxWaterSupply != dto.HasAuxWaterSupply;

        if (renewalTriggerChanged)
        {
            site.NeedsRenewalCheck = true;
        }

        // Property Information
        site.PropertyType = dto.PropertyType;
        site.BusinessName = dto.BusinessName;
        site.StreetNumber = dto.StreetNumber;
        site.StreetName = dto.StreetName;
        site.PropertyNumber = dto.PropertyNumber;
        site.City = dto.City;
        site.StateId = dto.State?.Id;
        site.ZipCode = dto.ZipCode;

        // Mailing Information
        site.MailingCompanyName = dto.MailingCompanyName;
        site.MailingContactName = dto.MailingContactName;
        site.MailingStreetNumber = dto.MailingStreetNumber;
        site.MailingStreetName = dto.MailingStreetName;
        site.MailingNumber = dto.MailingNumber;
        site.MailingCity = dto.MailingCity;
        site.MailingStateId = dto.MailingState?.Id;
        site.MailingZipCode = dto.MailingZipCode;
        site.MailingPhoneNumber = dto.MailingPhoneNumber;
        site.MailingEmailAddress = dto.MailingEmailAddress;

        // Property Settings
        site.AccountNumber = dto.AccountNumber;
        site.Active = dto.Active;
        site.InvalidMailingAddress = dto.InvalidMailingAddress;
        site.OutOfArea = dto.OutOfArea;
        site.IsFeeExempt = dto.IsFeeExempt;
        site.BypassPropertyNumberValidation = dto.BypassPropertyNumberValidation;
        site.BackflowScheduleMonth = dto.BackflowScheduleMonth;
        site.NeedsCsiInspection = dto.NeedsCsiInspection;
        site.CsiRenewalDate = dto.CsiRenewalDate;
        site.NeedsFogInspection = dto.NeedsFogInspection;
        site.FogInspectionExpirationDate = dto.FogInspectionExpirationDate;
        site.NeedsFogPermit = dto.NeedsFogPermit;
        site.FogPermitExpirationDate = dto.FogPermitExpirationDate;
        site.LastTripTicketDate = dto.LastTripTicketDate;
        site.TripTicketInterval = dto.TripTicketInterval;
        site.FacilityType = dto.FacilityType;
        site.GreaseTrapType = dto.GreaseTrapType;
        site.HasOnSiteSewageFacility = dto.HasOnSiteSewageFacility;
        site.HasAuxWaterSupply = dto.HasAuxWaterSupply;
        site.HasFireSystem = dto.HasFireSystem;
        site.FireSeparateWater = dto.FireSeparateWater;
        site.HasGritTrap = dto.HasGritTrap;
        site.HasIrrigation = dto.HasIrrigation;
        site.IrrigationSeparateWater = dto.IrrigationSeparateWater;
        site.HasDomesticPremisesIsolation = dto.HasDomesticPremisesIsolation;
    }

    public async Task<bool> UpdateWaterSupplierAsync(int siteId, UpdateSiteWaterSupplierDto dto)
    {
        var site = await _siteRepository.GetTrackedForUpdateAsync(siteId, CancellationToken.None);

        if (site == null)
        {
            return false;
        }

        var previousWaterSupplierId = site.WaterSupplierId;

        site.WaterSupplierId = dto.WaterSupplierId;

        site.UserAccountAssignmentId = null;
        site.CsiAccountAssignmentId = null;
        site.CsiAccountAssignmentDate = null;
        site.BackflowAccountAssignmentId = null;
        site.BackflowAccountAssignmentDate = null;
        site.FogAccountAssignmentId = null;
        site.FogAccountAssignmentDate = null;

        site.GisAreaId = 0;
        site.NeedsRenewalCheck = true;

        await _siteRepository.SaveChangesAsync(CancellationToken.None);

        await _recordLogService.AddAsync(RecordLogTableNames.Sites, siteId, dto.WaterSupplierId, RecordLogType.Edit, $"Water Supplier changed from {previousWaterSupplierId} to {dto.WaterSupplierId}");

        return true;
    }

    public async Task<IPagedData<CsiComplianceSiteDto>> GetCsiComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        var sites = await _siteRepository.GetCsiComplianceAsync(pageInfo, query, cancellationToken);
        var dtos = sites.Select(s => Mapper.Map<CsiComplianceSiteDto>(s)).ToList();

        await PopulateComplianceRowsAsync(dtos, dto => dto.CsiRenewalDate, cancellationToken);

        return dtos.ToPagedData(pageInfo);
    }

    public async Task<IPagedData<FogComplianceSiteDto>> GetFogInspectionComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        var sites = await _siteRepository.GetFogInspectionComplianceAsync(pageInfo, query, cancellationToken);
        var dtos = sites.Select(s => Mapper.Map<FogComplianceSiteDto>(s)).ToList();

        await PopulateComplianceRowsAsync(dtos, dto => dto.FogInspectionExpirationDate, cancellationToken);

        return dtos.ToPagedData(pageInfo);
    }

    public async Task<IPagedData<FogPermitComplianceSiteDto>> GetFogPermitComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        var sites = await _siteRepository.GetFogPermitComplianceAsync(pageInfo, query, cancellationToken);
        var dtos = sites.Select(s => Mapper.Map<FogPermitComplianceSiteDto>(s)).ToList();

        await PopulateComplianceRowsAsync(dtos, dto => dto.FogPermitExpirationDate, cancellationToken);

        return dtos.ToPagedData(pageInfo);
    }

    public async Task UpdateCsiAssignmentAsync(int siteId, int? userId)
    {
        var assignmentDate = userId.HasValue ? DateTime.UtcNow : (DateTime?)null;

        await _siteRepository.UpdateCsiAssignmentAsync(siteId, userId, assignmentDate);
    }

    public async Task UpdateBackflowAssignmentAsync(int siteId, int? userId)
    {
        var assignmentDate = userId.HasValue ? DateTime.UtcNow : (DateTime?)null;

        await _siteRepository.UpdateBackflowAssignmentAsync(siteId, userId, assignmentDate);
    }

    public async Task<IPagedData<FogTripTicketComplianceSiteDto>> GetFogTripTicketComplianceAsync(PageInfo pageInfo, Query query, DateTime? dueDateFrom, DateTime? dueDateTo, bool sortDescending, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        var sites = await _siteRepository.GetFogTripTicketComplianceAsync(pageInfo, query, dueDateFrom, dueDateTo, sortDescending, cancellationToken);
        var dtos = sites.Select(s => Mapper.Map<FogTripTicketComplianceSiteDto>(s)).ToList();

        foreach (var dto in dtos)
        {
            dto.DueDate = ComputeTripTicketDueDate(dto.LastTripTicketDate, dto.TripTicketInterval);
        }

        await PopulateComplianceRowsAsync(dtos, dto => dto.DueDate, cancellationToken);

        return dtos.ToPagedData(pageInfo);
    }

    private async Task PopulateComplianceRowsAsync<TDto>(List<TDto> dtos, Func<TDto, DateTime?> dueDateSelector, CancellationToken cancellationToken)
        where TDto : ComplianceSiteDtoBase
    {
        if (dtos.Count == 0)
        {
            return;
        }

        var logs = await _siteLogService.GetBySitesAsync(dtos.Select(d => d.Id), cancellationToken);
        var logsBySite = logs.ToLookup(l => l.Site.Id ?? 0);

        var today = _timeZoneHelper.GetUserLocalTime().Date;

        foreach (var dto in dtos)
        {
            dto.Logs = logsBySite[dto.Id].ToList();
            dto.DaysOverdue = ComputeDaysOverdue(dueDateSelector(dto), today);
            dto.OverdueSeverity = ComputeOverdueSeverity(dto.DaysOverdue);
        }
    }

    private static DateTime? ComputeTripTicketDueDate(DateTime? lastTripTicketDate, int tripTicketInterval)
    {
        if (!lastTripTicketDate.HasValue || tripTicketInterval <= 0)
        {
            return null;
        }

        return lastTripTicketDate.Value.AddDays(tripTicketInterval);
    }

    private static int? ComputeDaysOverdue(DateTime? dueDate, DateTime today)
    {
        if (!dueDate.HasValue)
        {
            return null;
        }

        var days = (today - dueDate.Value.Date).Days;

        return days < 0 ? null : days;
    }

    private static ComplianceOverdueSeverity ComputeOverdueSeverity(int? daysOverdue)
    {
        if (!daysOverdue.HasValue)
        {
            return ComplianceOverdueSeverity.None;
        }

        if (daysOverdue.Value > 90)
        {
            return ComplianceOverdueSeverity.High;
        }

        if (daysOverdue.Value >= 30)
        {
            return ComplianceOverdueSeverity.Moderate;
        }

        if (daysOverdue.Value > 0)
        {
            return ComplianceOverdueSeverity.Low;
        }

        return ComplianceOverdueSeverity.DueToday;
    }

    public async Task UpdateFogAssignmentAsync(int siteId, int? userId)
    {
        var assignmentDate = userId.HasValue ? DateTime.UtcNow : (DateTime?)null;

        await _siteRepository.UpdateFogAssignmentAsync(siteId, userId, assignmentDate);
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
