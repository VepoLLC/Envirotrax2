using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Sites;

public class SiteService : Service<Site, SiteDto>, ISiteService
{
    private readonly ISiteRepository _siteRepository;
    private readonly ISiteLogRepository _siteLogRepository;
    private readonly ISiteLogService _siteLogService;
    private readonly IRecordLogService _recordLogService;
    private readonly IGeocodingService _geocodingService;
    private readonly IGisAreaCoordinateRepository _coordinateRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly ICsiInspectionRepository _csiInspectionRepository;
    private readonly IBackflowTestRepository _backflowTestRepository;
    private readonly IBackflowOutOfServiceRequestRepository _outOfServiceRequestRepository;
    private readonly IFogInspectionRepository _fogInspectionRepository;
    private readonly IFogTripTicketRepository _fogTripTicketRepository;
    private readonly IMailingInfoRedactionService _mailingInfoRedactionService;
    private readonly ISiteScheduleService _siteScheduleService;
    private readonly IAuthService _authService;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly ILogger<SiteService> _logger;

    public SiteService(
        IMapper mapper,
        ISiteRepository repository,
        ISiteLogRepository siteLogRepository,
        ISiteLogService siteLogService,
        IRecordLogService recordLogService,
        IGeocodingService geocodingService,
        IGisAreaCoordinateRepository coordinateRepository,
        ITimeZoneHelperService timeZoneHelper,
        ICsiInspectionRepository csiInspectionRepository,
        IBackflowTestRepository backflowTestRepository,
        IBackflowOutOfServiceRequestRepository outOfServiceRequestRepository,
        IFogInspectionRepository fogInspectionRepository,
        IFogTripTicketRepository fogTripTicketRepository,
        IMailingInfoRedactionService mailingInfoRedactionService,
        ISiteScheduleService siteScheduleService,
        IAuthService authService,
        IGeneralSettingsService generalSettingsService,
        ILogger<SiteService> logger)
        : base(mapper, repository)
    {
        _siteRepository = repository;
        _siteLogRepository = siteLogRepository;
        _siteLogService = siteLogService;
        _recordLogService = recordLogService;
        _geocodingService = geocodingService;
        _coordinateRepository = coordinateRepository;
        _timeZoneHelper = timeZoneHelper;
        _csiInspectionRepository = csiInspectionRepository;
        _backflowTestRepository = backflowTestRepository;
        _outOfServiceRequestRepository = outOfServiceRequestRepository;
        _fogInspectionRepository = fogInspectionRepository;
        _fogTripTicketRepository = fogTripTicketRepository;
        _mailingInfoRedactionService = mailingInfoRedactionService;
        _siteScheduleService = siteScheduleService;
        _authService = authService;
        _generalSettingsService = generalSettingsService;
        _logger = logger;
    }

    public async Task<SiteTabCountsDto?> GetTabCountsAsync(int siteId, CancellationToken cancellationToken)
    {
        var siteExists = await _siteRepository.ExistsAsync(siteId, cancellationToken);

        if (!siteExists)
        {
            return null;
        }

        var logHistoryCount = await _siteLogService.CountBySiteAsync(siteId, cancellationToken);
        var csiCount = await _csiInspectionRepository.CountBySiteAsync(siteId, cancellationToken);
        var backflowCount = await _backflowTestRepository.CountCurrentInServiceBySiteAsync(siteId, cancellationToken);
        var outOfServiceCount = await _outOfServiceRequestRepository.CountBySiteAsync(siteId, cancellationToken);
        var tripTicketCount = await _fogTripTicketRepository.CountBySiteAsync(siteId, cancellationToken);
        var fogCount = await _fogInspectionRepository.CountBySiteAsync(siteId, cancellationToken);

        return new SiteTabCountsDto
        {
            LogHistoryCount = logHistoryCount,
            CsiCount = csiCount,
            BackflowCount = backflowCount,
            OutOfServiceCount = outOfServiceCount,
            TripTicketCount = tripTicketCount,
            FogCount = fogCount
        };
    }

    // Mirrors V1's site_search.aspx.vb: creating a site always logs a fixed "New site record" entry
    // (no field-level diff — V1 doesn't diff on create either, since there's no prior row to compare against).
    public override async Task<SiteDto> AddAsync(SiteDto dto)
    {
        await PrepareWaterSupplierAccountNumberAsync(dto);

        SiteDto added;

        try
        {
            added = await base.AddAsync(dto);
        }
        catch (DuplicateRecordException exception)
        {
            ThrowIfAccountNumberDuplicated(exception);

            throw;
        }

        if (added.WaterSupplier?.Id is int waterSupplierId)
        {
            // recordLog manual
            await _recordLogService.AddAsync(RecordLogTableNames.Sites, added.Id, waterSupplierId, RecordLogType.Add, "New site record");
        }

        return added;
    }

    public override async Task<SiteDto> UpdateAsync(SiteDto dto)
    {
        await KeepAccountNumberUnlessChangesAllowedAsync(dto);
        await PrepareWaterSupplierAccountNumberAsync(dto);

        try
        {
            return await base.UpdateAsync(dto);
        }
        catch (DuplicateRecordException exception)
        {
            ThrowIfAccountNumberDuplicated(exception);

            throw;
        }
    }

    public async Task<SiteAccountNumberDto> GenerateAccountNumberAsync()
    {
        var uniqueNumber = await _siteRepository.GetNextAccountNumberAsync();
        var randomNumber = Random.Shared.Next(1000, 9999);

        return new SiteAccountNumberDto
        {
            AccountNumber = $"{_authService.WaterSupplierId}-{randomNumber}-{uniqueNumber}"
        };
    }

    private async Task KeepAccountNumberUnlessChangesAllowedAsync(SiteDto dto)
    {
        var settings = await _generalSettingsService.GetAsync(
            _authService.WaterSupplierId,
            generalSettings => new GeneralSettings { AllowAccountNumberChanges = generalSettings.AllowAccountNumberChanges },
            CancellationToken.None);

        if (settings.AllowAccountNumberChanges)
        {
            return;
        }

        var storedAccountNumber = await _siteRepository.GetAccountNumberAsync(dto.Id, CancellationToken.None);

        if (storedAccountNumber == null)
        {
            return;
        }

        dto.AccountNumber = storedAccountNumber;
    }

    // Stores a blank WS Account Number as NULL, so only real numbers have to be unique. V1 saved blanks as '',
    // which meant a second site left blank failed with the "must be unique" message.
    private Task PrepareWaterSupplierAccountNumberAsync(SiteDto dto)
    {
        dto.WaterSupplierAccountNumber = string.IsNullOrWhiteSpace(dto.WaterSupplierAccountNumber)
            ? null
            : dto.WaterSupplierAccountNumber.Trim();

        return Task.CompletedTask;
    }

    private static void ThrowIfAccountNumberDuplicated(DuplicateRecordException exception)
    {
        if (exception.PropertyNames.Contains(nameof(Site.WaterSupplierAccountNumber)))
        {
            throw new AppValidationException("WS Account Number must be unique.", exception);
        }

        if (exception.PropertyNames.Contains(nameof(Site.AccountNumber)))
        {
            throw new AppValidationException("Account Number must be unique.", exception);
        }
    }

    public async Task<SiteDto> AddWithGeneratedAccountNumberAsync(SiteDto dto)
    {
        var prefix = $"{dto.WaterSupplier!.Id}-{Random.Shared.Next(1000, 10000)}";
        dto.AccountNumber = prefix;

        var added = await AddAsync(dto);

        added.AccountNumber = $"{prefix}-{added.Id}";

        await _siteRepository.UpdateAccountNumberAsync(added.Id, added.AccountNumber);

        return added;
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

    // GetAsync stays unredacted: professional submissions snapshot the site's real mailing information from it.
    public async Task<IPagedData<ProfessionalSiteDto>> SearchForProfessionalAsync(ProfessionalSiteSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        var sites = (await _siteRepository.SearchForProfessionalAsync(
            criteria,
            pageInfo,
            query,
            _authService.ProfessionalId,
            _authService.UserId,
            cancellationToken)).ToList();

        var schedules = await _siteScheduleService.GetMyBySiteIdsAsync(sites.Select(s => s.Id), cancellationToken);
        var schedulesBySiteId = schedules.ToLookup(s => s.SiteId);

        var result = sites
            .Select(site =>
            {
                var dto = Mapper.Map<ProfessionalSiteDto>(site);
                dto.Schedules = schedulesBySiteId[site.Id].ToList();

                return dto;
            })
            .ToPagedData(pageInfo);

        return await _mailingInfoRedactionService.RedactAsync(result, cancellationToken);
    }

    public async Task<SiteDto?> GetForProfessionalAsync(int id, CancellationToken cancellationToken)
    {
        var site = await GetAsync(id, cancellationToken);

        if (site != null)
        {
            await _mailingInfoRedactionService.RedactAsync(site, cancellationToken);
        }

        return site;
    }

    public async Task<IPagedData<BackflowSubmissionSiteDto>> SearchForBackflowSubmissionAsync(BackflowSubmissionSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<Site, SiteDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<Site, SiteDto>(Mapper);

        List<Site> sites;
        List<BackflowTest> assemblies;

        if (IsSerialNumberSearch(criteria))
        {
            assemblies = await _backflowTestRepository.GetSerialNumberMatchesAsync(criteria, cancellationToken);

            var matchingSiteIds = GetSiteIds(assemblies);

            sites = (await _siteRepository.GetForBackflowSubmissionAsync(matchingSiteIds, pageInfo, query, cancellationToken)).ToList();
        }
        else
        {
            sites = (await _siteRepository.SearchForBackflowSubmissionAsync(criteria, pageInfo, query, cancellationToken)).ToList();

            var siteIds = GetSiteIds(sites);

            assemblies = await _backflowTestRepository.GetSubmissionAssembliesAsync(siteIds, cancellationToken);
        }

        var assembliesBySiteId = assemblies.ToLookup(assembly => assembly.SiteId);

        var siteDtos = new List<BackflowSubmissionSiteDto>();
        var redactableDtos = new List<IRedactableMailingInfoDto>();

        foreach (var site in sites)
        {
            var siteDto = Mapper.Map<BackflowSubmissionSiteDto>(site);

            foreach (var assembly in assembliesBySiteId[site.Id])
            {
                var assemblyDto = Mapper.Map<BackflowTestDto>(assembly);

                siteDto.Assemblies.Add(assemblyDto);
                redactableDtos.Add(assemblyDto);
            }

            siteDtos.Add(siteDto);
            redactableDtos.Add(siteDto);
        }

        await _mailingInfoRedactionService.RedactAsync(redactableDtos, cancellationToken);

        return siteDtos.ToPagedData(pageInfo);
    }

    private static bool IsSerialNumberSearch(BackflowSubmissionSearchDto criteria)
    {
        return string.IsNullOrWhiteSpace(criteria.AccountNumber) && !string.IsNullOrWhiteSpace(criteria.Manufacturer);
    }

    private static List<int> GetSiteIds(List<Site> sites)
    {
        var siteIds = new List<int>();

        foreach (var site in sites)
        {
            siteIds.Add(site.Id);
        }

        return siteIds;
    }

    private static List<int> GetSiteIds(List<BackflowTest> assemblies)
    {
        var siteIds = new List<int>();

        foreach (var assembly in assemblies)
        {
            if (assembly.SiteId != null && !siteIds.Contains(assembly.SiteId.Value))
            {
                siteIds.Add(assembly.SiteId.Value);
            }
        }

        return siteIds;
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
            // Only the outer edge decides whether the site falls into the area, exactly as V1 did
            // (checkPointInArea in WaterSupplierGisArea.vb never looked at the inner polygons), so a
            // site standing inside a hole still gets the area assigned.
            var gisPoints = group
                .Where(c => c.PolygonIndex == 0)
                .OrderBy(c => c.Id)
                .Select(c => new CoordinateDto
                {
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                }).ToList();

            if (_geocodingService.IsPointInArea(gisPoints, coordinates))
            {
                site.GisAreaId = group.Key;
                break;
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

    // TODO(needs sign-off): this now writes a RecordLog entry on every normal site edit (mirroring the
    // Backflow/CSI pattern), whereas before it wrote nothing. Confirm this is the desired behavior before merging.
    public async Task<bool> UpdateFromAdminAsync(int siteId, SiteDto dto, CancellationToken cancellationToken)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var saved = await _siteRepository.UpdateForAdminAsync(siteId, dto);

            if (saved == null)
            {
                return false;
            }

            scope.Complete();
        }

        return true;
    }

    public async Task UpdateAdditionalInformationAsync(int siteId, SiteAdditionalInformationDto dto)
    {
        var saved = await _siteRepository.UpdateAdditionalInformationAsync(siteId, dto);

        if (saved == null)
        {
            throw new AppValidationException("The property for this inspection was not found.");
        }
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

        // The previous supplier's own account number means nothing to the new one, and could collide with one of
        // its sites under the unique index.
        site.WaterSupplierAccountNumber = null;

        site.GisAreaId = 0;
        site.NeedsRenewalCheck = true;

        await _siteRepository.SaveChangesAsync();

        // recordLog manual
        await _recordLogService.AddAsync(RecordLogTableNames.Sites, siteId, dto.WaterSupplierId, RecordLogType.Edit, $"Water Supplier changed from {previousWaterSupplierId} to {dto.WaterSupplierId}");

        return true;
    }

    public async Task<bool> DeleteFromAdminAsync(int siteId)
    {
        var hasRecords = await _siteRepository.HasRecordsAsync(siteId, CancellationToken.None);

        if (hasRecords)
        {
            throw new AppValidationException("Existing backflow, CSI, FOG records must be removed from the site before the record can be deleted.");
        }

        var deleted = await DeleteAsync(siteId);

        return deleted != null;
    }

    public async Task<SiteDto?> CopyAsync(int siteId)
    {
        var site = await _siteRepository.GetAsync(siteId, CancellationToken.None);

        if (site == null || site.DeletedTime.HasValue)
        {
            return null;
        }

        var copy = BuildCopy(site);

        return await AddWithGeneratedAccountNumberAsync(copy);
    }

    private static SiteDto BuildCopy(Site site)
    {
        return new SiteDto
        {
            WaterSupplier = new ReferencedWaterSupplierDto { Id = site.WaterSupplierId },
            SubArea = site.SubArea,
            Active = site.Active,

            PropertyType = site.PropertyType,
            BusinessName = site.BusinessName,
            StreetNumber = site.StreetNumber,
            StreetName = site.StreetName,
            PropertyNumber = site.PropertyNumber,
            City = site.City,
            State = site.StateId.HasValue ? new ReferencedStateDto { Id = site.StateId } : null,
            ZipCode = site.ZipCode,

            MailingCompanyName = site.MailingCompanyName,
            MailingContactName = site.MailingContactName,
            MailingStreetNumber = site.MailingStreetNumber,
            MailingStreetName = site.MailingStreetName,
            MailingNumber = site.MailingNumber,
            MailingCity = site.MailingCity,
            MailingState = site.MailingStateId.HasValue ? new ReferencedStateDto { Id = site.MailingStateId } : null,
            MailingZipCode = site.MailingZipCode,
            MailingPhoneNumber = site.MailingPhoneNumber,
            MailingEmailAddress = site.MailingEmailAddress,
            FogGeneratorPhoneNumber = site.FogGeneratorPhoneNumber,
            FogGeneratorEmailAddress = site.FogGeneratorEmailAddress,
            Comments = site.Comments,

            NeedsCsiInspection = site.NeedsCsiInspection,
            CsiRenewalDate = site.CsiRenewalDate,
            NeedsFogInspection = site.NeedsFogInspection,
            FogInspectionExpirationDate = site.FogInspectionExpirationDate,
            NeedsFogPermit = site.NeedsFogPermit,
            FogPermitExpirationDate = site.FogPermitExpirationDate,
            LastTripTicketDate = site.LastTripTicketDate,
            TripTicketInterval = site.TripTicketInterval,
            IsFeeExempt = site.IsFeeExempt,
            RainFreezeSensorType = site.RainFreezeSensorType,
            HasOnSiteSewageFacility = site.HasOnSiteSewageFacility,
            HasWaterWell = site.HasWaterWell,
            HasAuxWaterSupply = site.HasAuxWaterSupply,
            HasFireSystem = site.HasFireSystem,
            FireSeparateWater = site.FireSeparateWater,
            GreaseTrapType = site.GreaseTrapType,
            HasGritTrap = site.HasGritTrap,
            HasReclaimed = site.HasReclaimed,
            HasIrrigation = site.HasIrrigation,
            IrrigationSeparateWater = site.IrrigationSeparateWater,
            InvalidMailingAddress = site.InvalidMailingAddress,
            OutOfArea = site.OutOfArea,
            FacilityType = site.FacilityType,
            BackflowScheduleMonth = site.BackflowScheduleMonth,

            GisLatitude = site.GisLatitude,
            GisLongitude = site.GisLongitude,
            GisStatus = (int)site.GisStatus,
            GisDate = site.GisDate,
            GisAreaId = site.GisAreaId,

            ImportSiteId = site.ImportSiteId,
            ImportSiteId2 = site.ImportSiteId2,
            ImportId = site.ImportId,
            ExcludeFromBackflowMailing = site.ExcludeFromBackflowMailing,
            ExcludeFromCsiMailing = site.ExcludeFromCsiMailing,
            NeedsValidation = site.NeedsValidation
        };
    }

    public async Task<bool> MergeAsync(int siteId, int sourceSiteId)
    {
        if (siteId == sourceSiteId)
        {
            throw new AppValidationException("Cannot merge a site onto itself.");
        }

        var site = await _siteRepository.GetAsync(siteId, CancellationToken.None);
        var sourceSite = await _siteRepository.GetAsync(sourceSiteId, CancellationToken.None);

        if (site == null || site.DeletedTime.HasValue || sourceSite == null || sourceSite.DeletedTime.HasValue)
        {
            return false;
        }

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            await _csiInspectionRepository.TransferToSiteAsync(sourceSiteId, site);

            var transferredTestCount = await _backflowTestRepository.TransferToSiteAsync(sourceSiteId, site);

            await _fogInspectionRepository.TransferToSiteAsync(sourceSiteId, site);
            await _fogTripTicketRepository.TransferToSiteAsync(sourceSiteId, site);
            await _siteLogRepository.TransferToSiteAsync(sourceSiteId, siteId);

            await _siteRepository.DeleteAsync(sourceSiteId);

            if (transferredTestCount > 0)
            {
                await _siteRepository.SetNeedsRenewalCheckAsync(siteId);
            }

            await _recordLogService.AddAsync(RecordLogTableNames.Sites, siteId, site.WaterSupplierId, RecordLogType.Edit, $"Site merged {sourceSiteId}");

            scope.Complete();
        }

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
