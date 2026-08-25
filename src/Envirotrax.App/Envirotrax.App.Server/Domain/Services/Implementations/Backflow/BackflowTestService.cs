using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow;

public class BackflowTestService : Service<BackflowTest, BackflowTestDto>, IBackflowTestService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff"];

    private static readonly HashSet<string> ValidImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "assembly", "serial-number", "bypass-assembly", "bypass-serial-number", "air-gap"
    };

    private readonly IBackflowTestRepository _testRepository;
    private readonly IProfessionalRepository _professionalRepository;
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthService _authService;
    private readonly IPdfTemplateService _pdfTemplateService;
    private readonly ISiteService _siteService;
    private readonly ISiteRepository _siteRepository;
    private readonly ISiteLogService _siteLogService;
    private readonly IBackflowRenewalRequirementService _renewalRequirementService;
    private readonly IBackflowSettingsService _settingsService;
    private readonly IRecordLogService _recordLogService;
    private readonly ILogger<BackflowTestService> _logger;

    public BackflowTestService(
        IMapper mapper,
        IBackflowTestRepository repository,
        IProfessionalRepository professionalRepository,
        IProfessionalUserRepository professionalUserRepository,
        IFileStorageService fileStorageService,
        IAuthService authService,
        IPdfTemplateService pdfTemplateService,
        ISiteService siteService,
        ISiteRepository siteRepository,
        ISiteLogService siteLogService,
        IBackflowRenewalRequirementService renewalRequirementService,
        IBackflowSettingsService settingsService,
        IRecordLogService recordLogService,
        ILogger<BackflowTestService> logger)
        : base(mapper, repository)
    {
        _testRepository = repository;
        _professionalRepository = professionalRepository;
        _professionalUserRepository = professionalUserRepository;
        _fileStorageService = fileStorageService;
        _authService = authService;
        _pdfTemplateService = pdfTemplateService;
        _siteService = siteService;
        _siteRepository = siteRepository;
        _siteLogService = siteLogService;
        _renewalRequirementService = renewalRequirementService;
        _settingsService = settingsService;
        _recordLogService = recordLogService;
        _logger = logger;
    }

    public async Task<IPagedData<BackflowComplianceDto>> GetComplianceAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        // Client filters/sort are keyed on DTO property names (incl. nested site.* paths); translate them to
        // the entity model, mirroring the base test-search path and SiteService.GetCsiComplianceAsync.
        query.Sort = query.ConvertSortProperties<BackflowTest, BackflowTestDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<BackflowTest, BackflowTestDto>(Mapper);

        var tests = await _testRepository.GetComplianceAsync(pageInfo, query, cancellationToken);
        var dtos = tests.Select(t => Mapper.Map<BackflowComplianceDto>(t)).ToList();

        // Attach each row's site logs (top 5 per site) in a single round-trip, then stitch by site id — the
        // same approach the CSI compliance page uses. A site's logs repeat across its assembly rows; the grid
        // renders them once per site group.
        var siteIds = dtos
            .Where(d => d.Site?.Id != null)
            .Select(d => d.Site!.Id!.Value)
            .Distinct()
            .ToList();

        if (siteIds.Count > 0)
        {
            var logs = await _siteLogService.GetBySitesAsync(siteIds, cancellationToken);
            var logsBySite = logs.ToLookup(l => l.Site?.Id ?? 0);

            foreach (var dto in dtos)
            {
                if (dto.Site?.Id != null)
                {
                    dto.Logs = logsBySite[dto.Site.Id.Value].ToList();
                }
            }
        }

        return dtos.ToPagedData(pageInfo);
    }

    public async Task<IPagedData<BackflowTestDto>> SearchAsync(PageInfo pageInfo, Query query, BackflowPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<BackflowTest, BackflowTestDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<BackflowTest, BackflowTestDto>(Mapper);

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowTest.Id)] = SortOperator.Asc;
        }

        var tests = await _testRepository.SearchAsync(pageInfo, query, paymentStatus, cancellationToken);

        return tests.Select(t => MapToDto(t)!).ToPagedData(pageInfo);
    }

    private async Task PopulateBpatSnapshotAsync(BackflowTestDto dto)
    {
        if (dto.Professional?.Id is int professionalId)
        {
            var professional = await _professionalRepository.GetAsync(professionalId, CancellationToken.None);
            if (professional != null)
            {
                dto.BpatCompanyName = professional.Name;
                dto.BpatAddress = professional.Address;
                dto.BpatCity = professional.City;
                dto.BpatZip = professional.ZipCode;
                dto.BpatWorkNumber = professional.PhoneNumber;
                if (professional.StateId.HasValue)
                {
                    dto.BpatState = new ReferencedStateDto { Id = professional.StateId.Value };
                }
            }
        }

        if (dto.Bpat?.Id is int bpatId)
        {
            var bpatUser = await _professionalUserRepository.GetAsync(bpatId, CancellationToken.None);
            if (bpatUser != null)
            {
                dto.BpatContactName = bpatUser.ContactName;
            }
        }
    }

    // TestDate is the derived "overall test date" (mirrors V1's submit behavior): air-gap tests store
    // their date in InitialTestDate; otherwise the final (after-repairs) date wins over the initial one.
    private static void DeriveTestDate(BackflowTestDto dto)
    {
        if (dto.DeviceType == nameof(BackflowDeviceType.AG))
        {
            dto.TestDate = dto.InitialTestDate;
        }
        else
        {
            dto.TestDate = dto.FinalTestDate ?? dto.InitialTestDate;
        }
    }

    private async Task ApplyRenewalAsync(BackflowTestDto dto, bool hasAuxWaterSupply, CancellationToken cancellationToken)
    {
        dto.RenewalRequired = false;
        dto.ExpirationDate = null;

        if (dto.WaterSupplier?.Id is not int waterSupplierId)
        {
            return;
        }

        if (dto.OutOfService || (!string.IsNullOrEmpty(dto.DeviceType) && SkippedDeviceTypes.Contains(dto.DeviceType)))
        {
            return;
        }

        var requirements = await _renewalRequirementService.GetAllByWaterSupplierIdAsync(waterSupplierId, cancellationToken);

        var test = new BackflowTest
        {
            DeviceType = dto.DeviceType,
            PropertyType = dto.PropertyType,
            HazardType = dto.HazardType,
            Ossf = dto.Ossf,
            TestResult = dto.TestResult,
            TestDate = dto.TestDate,
            Site = new Site { HasAuxWaterSupply = hasAuxWaterSupply }
        };

        var (renewalRequired, expirationDate) = ComputeRenewal(test, requirements);

        dto.RenewalRequired = renewalRequired;
        dto.ExpirationDate = expirationDate;
    }

    private static void ApplySiteSnapshot(BackflowTestDto dto, SiteDto site)
    {
        dto.AccountNumber = site.AccountNumber;
        dto.PropertyBusinessName = site.BusinessName;
        dto.PropertyType = (int)site.PropertyType;
        dto.PropertyStreetNumber = site.StreetNumber;
        dto.PropertyStreetName = site.StreetName;
        dto.PropertyNumber = site.PropertyNumber;
        dto.PropertyCity = site.City;
        dto.PropertyState = site.State;
        dto.PropertyZip = site.ZipCode;

        dto.MailingCompanyName = site.MailingCompanyName;
        dto.MailingContactName = site.MailingContactName;
        dto.MailingStreetNumber = site.MailingStreetNumber;
        dto.MailingStreetName = site.MailingStreetName;
        dto.MailingNumber = site.MailingNumber;
        dto.MailingCity = site.MailingCity;
        dto.MailingState = site.MailingState;
        dto.MailingZip = site.MailingZipCode;
        dto.MailingPhoneNumber = site.MailingPhoneNumber;
        dto.MailingEmailAddress = site.MailingEmailAddress;
    }

    public async Task<BackflowTestDto> SubmitWithImagesAsync(
        BackflowTestDto dto,
        Stream? assemblyStream, string? assemblyFileName,
        Stream? serialStream, string? serialFileName,
        Stream? bypassAssemblyStream, string? bypassAssemblyFileName,
        Stream? bypassSerialStream, string? bypassSerialFileName,
        Stream? airGapStream, string? airGapFileName,
        CancellationToken cancellationToken = default)
    {
        var professionalId = _authService.ProfessionalId;
        dto.Professional = new ReferencedProfessionalDto { Id = professionalId };

        await PopulateBpatSnapshotAsync(dto);
        DeriveTestDate(dto);

        bool hasAuxWaterSupply = false;

        if (dto.Site?.Id != null)
        {
            var site = await _siteService.GetAsync(dto.Site.Id.Value, cancellationToken);

            if (site != null)
            {
                ApplySiteSnapshot(dto, site);
                hasAuxWaterSupply = site.HasAuxWaterSupply;
            }
        }

        await ApplyRenewalAsync(dto, hasAuxWaterSupply, cancellationToken);

        // Set paths before AddAsync to avoid a second EF update (double-tracking conflict)
        if (assemblyStream != null && assemblyFileName != null)
        {
            dto.AssemblyImagePath = $"professionals/{professionalId}/backflow-tests/assembly/{Guid.NewGuid()}{ValidateAndGetExtension(assemblyFileName)}";
        }
        if (serialStream != null && serialFileName != null)
        {
            dto.SerialNumberImagePath = $"professionals/{professionalId}/backflow-tests/serial-number/{Guid.NewGuid()}{ValidateAndGetExtension(serialFileName)}";
        }
        if (bypassAssemblyStream != null && bypassAssemblyFileName != null)
        {
            dto.BypassAssemblyImagePath = $"professionals/{professionalId}/backflow-tests/bypass-assembly/{Guid.NewGuid()}{ValidateAndGetExtension(bypassAssemblyFileName)}";
        }
        if (bypassSerialStream != null && bypassSerialFileName != null)
        {
            dto.BypassSerialNumberImagePath = $"professionals/{professionalId}/backflow-tests/bypass-serial-number/{Guid.NewGuid()}{ValidateAndGetExtension(bypassSerialFileName)}";
        }
        if (airGapStream != null && airGapFileName != null)
        {
            dto.AirGapImagePath = $"professionals/{professionalId}/backflow-tests/air-gap/{Guid.NewGuid()}{ValidateAndGetExtension(airGapFileName)}";
        }

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var saved = await base.AddAsync(dto);

        if (assemblyStream != null && dto.AssemblyImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.AssemblyImagePath, assemblyStream);
        }
        if (serialStream != null && dto.SerialNumberImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.SerialNumberImagePath, serialStream);
        }
        if (bypassAssemblyStream != null && dto.BypassAssemblyImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.BypassAssemblyImagePath, bypassAssemblyStream);
        }
        if (bypassSerialStream != null && dto.BypassSerialNumberImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.BypassSerialNumberImagePath, bypassSerialStream);
        }
        if (airGapStream != null && dto.AirGapImagePath != null)
        {
            await _fileStorageService.UploadAsync(dto.AirGapImagePath, airGapStream);
        }

        scope.Complete();
        return saved;
    }

    public async Task<BackflowTestExpiryCountsDto> GetExpiryCountsAsync(CancellationToken cancellationToken = default)
    {
        var counts = await _testRepository.GetExpiryCountsAsync(cancellationToken);

        return new BackflowTestExpiryCountsDto
        {
            Expired = counts.Expired,
            ThisMonth = counts.ThisMonth,
            NextMonth = counts.NextMonth,
            TwoMonths = counts.TwoMonths
        };
    }

    public override async Task<BackflowTestDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await base.GetAsync(id, cancellationToken);

        if (dto != null)
        {
            await PopulateImageUrlsAsync(dto);
        }

        return dto;
    }

    public async Task<BackflowTestAdminDetailsDto?> GetForAdminAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _testRepository.GetAsync(id, cancellationToken);

        if (test == null)
        {
            return null;
        }

        var dto = Mapper.Map<BackflowTestAdminDetailsDto>(test);

        await PopulateImageUrlsAsync(dto);

        var settings = await _settingsService.GetTestingSettingsByWaterSupplierAsync(test.WaterSupplierId, cancellationToken);

        dto.ShowRainSensor = settings.ShowRainSensor;
        dto.ShowOSSF = settings.ShowOSSF;
        dto.ShowPermitNumber = settings.ShowPermitNumber;

        return dto;
    }

    public async Task<BackflowTestAdminDetailsDto?> UpdateForAdminAsync(int id, BackflowTestAdminUpdateRequest request)
    {
        ValidateAdminUpdate(request);

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var saved = await _testRepository.UpdateForAdminAsync(id, request, _authService.UserId);

            if (saved.Model == null)
            {
                return null;
            }

            if (saved.Changes.Length > 0)
            {
                await _recordLogService.AddAsync(RecordLogTableNames.BackflowTests, id, saved.Model.WaterSupplierId, RecordLogType.Edit, saved.Changes);
            }

            scope.Complete();
        }

        return await GetForAdminAsync(id, default);
    }

    private static void ValidateAdminUpdate(BackflowTestAdminUpdateRequest request)
    {
        if (!HasBypassAssembly(request.DeviceType))
        {
            return;
        }

        var bypassIsIncomplete = string.IsNullOrWhiteSpace(request.Manufacturer2)
            || string.IsNullOrWhiteSpace(request.Model2)
            || string.IsNullOrWhiteSpace(request.Size2)
            || string.IsNullOrWhiteSpace(request.SerialNumber2);

        if (bypassIsIncomplete)
        {
            throw new AppValidationException("Bypass Assembly Manufacturer, Model, Size and Serial Number are required for this backflow method.");
        }
    }

    private static bool HasBypassAssembly(string? deviceType)
    {
        return deviceType == nameof(BackflowDeviceType.DCD)
            || deviceType == nameof(BackflowDeviceType.DCD2)
            || deviceType == nameof(BackflowDeviceType.RPPD)
            || deviceType == nameof(BackflowDeviceType.RPPD2);
    }

    public override async Task<BackflowTestDto?> DeleteAsync(int id)
    {
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var deleted = await _testRepository.DeleteAsync(id);

        if (deleted == null || !string.IsNullOrEmpty(deleted.TransactionId))
        {
            return null;
        }

        scope.Complete();
        return MapToDto(deleted);
    }

    public async Task<BackflowTestDto?> UpdateImageAsync(int id, string imageType, Stream fileStream, string fileName, CancellationToken cancellationToken = default)
    {
        if (!ValidImageTypes.Contains(imageType))
        {
            throw new ValidationException("Invalid image type.");
        }

        var dto = await base.GetAsync(id, cancellationToken);
        if (dto == null)
        {
            return null;
        }

        var extension = ValidateAndGetExtension(fileName);
        var oldPath = GetImagePath(dto, imageType);
        var newPath = $"backflow-tests/{id}/{imageType.ToLowerInvariant()}/{Guid.NewGuid()}{extension}";
        SetImagePath(dto, imageType, newPath);

        BackflowTestDto saved;
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var model = MapToModel(dto)!;
            await _testRepository.UpdateImagePathAsync(model, GetImagePathPropertyName(imageType));
            saved = MapToDto(model)!;
            await _fileStorageService.UploadAsync(newPath, fileStream);
            scope.Complete();
        }

        if (!string.IsNullOrWhiteSpace(oldPath))
        {
            await _fileStorageService.DeleteAsync(oldPath);
        }

        await PopulateImageUrlsAsync(saved);

        return saved;
    }

    private async Task PopulateImageUrlsAsync(BackflowTestDto dto)
    {
        var images = new (string? Path, Action<string> SetUrl)[]
        {
            (dto.AssemblyImagePath, url => dto.AssemblyImageUrl = url),
            (dto.SerialNumberImagePath, url => dto.SerialNumberImageUrl = url),
            (dto.BypassAssemblyImagePath, url => dto.BypassAssemblyImageUrl = url),
            (dto.BypassSerialNumberImagePath, url => dto.BypassSerialNumberImageUrl = url),
            (dto.AirGapImagePath, url => dto.AirGapImageUrl = url)
        };

        if (!images.Any(i => !string.IsNullOrWhiteSpace(i.Path)))
        {
            return;
        }

        var delegationKey = await _fileStorageService.GetUserDelegationKeyAsync();

        foreach (var (path, setUrl) in images)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                var url = await _fileStorageService.GenerateSasUrlAsync(delegationKey, path);
                setUrl(url.ToString());
            }
        }
    }

    private static string? GetImagePath(BackflowTestDto dto, string imageType) => imageType.ToLowerInvariant() switch
    {
        "assembly" => dto.AssemblyImagePath,
        "serial-number" => dto.SerialNumberImagePath,
        "bypass-assembly" => dto.BypassAssemblyImagePath,
        "bypass-serial-number" => dto.BypassSerialNumberImagePath,
        "air-gap" => dto.AirGapImagePath,
        _ => null
    };

    private static string GetImagePathPropertyName(string imageType) => imageType.ToLowerInvariant() switch
    {
        "assembly" => nameof(BackflowTest.AssemblyImagePath),
        "serial-number" => nameof(BackflowTest.SerialNumberImagePath),
        "bypass-assembly" => nameof(BackflowTest.BypassAssemblyImagePath),
        "bypass-serial-number" => nameof(BackflowTest.BypassSerialNumberImagePath),
        "air-gap" => nameof(BackflowTest.AirGapImagePath),
        _ => throw new ValidationException("Invalid image type.")
    };

    private static void SetImagePath(BackflowTestDto dto, string imageType, string? path)
    {
        switch (imageType.ToLowerInvariant())
        {
            case "assembly": dto.AssemblyImagePath = path; break;
            case "serial-number": dto.SerialNumberImagePath = path; break;
            case "bypass-assembly": dto.BypassAssemblyImagePath = path; break;
            case "bypass-serial-number": dto.BypassSerialNumberImagePath = path; break;
            case "air-gap": dto.AirGapImagePath = path; break;
        }
    }

    private static string ValidateAndGetExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);

        if (!AllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");
        }

        return ext;
    }

    public Task<byte[]> GeneratePdfAsync(BackflowTestDto test)
    {
        return GeneratePdfAsync(new[] { test });
    }

    public Task<byte[]> GeneratePdfAsync(IEnumerable<BackflowTestDto> tests)
    {
        return _pdfTemplateService.GenerateAsync("Backflow.BackflowTest", tests);
    }

    public Task<byte[]> GeneratePdfForProfessionalAsync(BackflowTestDto test)
    {
        if (test.TransactionId == null)
        {
            throw new AppValidationException("Report can't be downloaded until it's paid. Please go to checkout and pay for this transaction, then try downloading again.");
        }

        return GeneratePdfAsync(test);
    }

    public async Task<IEnumerable<BackflowTestDto>> GetAllPendingTestsForRenewalAsync(int batchSize, CancellationToken cancellationToken)
    {
        var tests = await _testRepository.GetAllPendingRenewalByTestFlagAsync(batchSize, cancellationToken);
        return Mapper.Map<IEnumerable<BackflowTest>, IEnumerable<BackflowTestDto>>(tests);
    }

    public async Task ProcessSiteRenewalAsync(int siteId, CancellationToken cancellationToken)
    {
        var requirements = (await _renewalRequirementService.GetAllAsync(cancellationToken)).ToList();
        var tests = await _testRepository.GetAllCurrentBySiteIdAsync(siteId, cancellationToken);

        foreach (var test in tests)
        {
            if (test.ExpirationDate == null)
            {
                return;
            }

            try
            {
                var matched = requirements.FirstOrDefault(r => DoesTestMatchRequirement(test, r));
                bool renewalRequired = matched != null;
                DateTime? newExpirationDate = null;

                if (matched != null && test.TestResult == BackflowTestResult.Pass)
                {
                    newExpirationDate = (test.TestDate ?? DateTime.UtcNow).AddYears(matched.RenewalYears);
                }

                await _testRepository.UpdateTestRenewalAsync(test.Id, renewalRequired, newExpirationDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Stopping renewal processing for site {SiteId} - failed to update test {TestId}",
                    siteId, test.Id);
                return;
            }
        }

        await _siteRepository.ClearNeedsRenewalCheckAsync(siteId);
    }

    public async Task ProcessTestRenewalAsync(int testId, CancellationToken cancellationToken)
    {
        var test = await _testRepository.GetAsync(testId, cancellationToken);

        if (test == null || test.DeletedTime.HasValue)
        {
            return;
        }

        if (test.OutOfService || (!string.IsNullOrEmpty(test.DeviceType) && SkippedDeviceTypes.Contains(test.DeviceType)))
        {
            await _testRepository.ClearTestNeedsRenewalCheckAsync(testId, cancellationToken);
            return;
        }

        var requirements = (await _renewalRequirementService.GetAllAsync(cancellationToken)).ToList();

        var (renewalRequired, newExpirationDate) = ComputeRenewal(test, requirements);

        await _testRepository.UpdateTestRenewalAndClearFlagAsync(testId, renewalRequired, newExpirationDate, cancellationToken);
    }

    private static readonly HashSet<string> SkippedDeviceTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "Rain", "Freeze", "RainFreeze"
    };

    public async Task<BackflowTestDto?> UpdateRenewalRequiredAsync(int id, bool renewalRequired, CancellationToken cancellationToken = default)
    {
        var test = await _testRepository.UpdateRenewalRequiredAsync(id, renewalRequired, _authService.UserId, cancellationToken);

        return test == null ? null : MapToDto(test);
    }

    public async Task<BackflowTestDto?> UpdateScheduleMonthAsync(int id, int month, CancellationToken cancellationToken = default)
    {
        var test = await _testRepository.UpdateScheduleMonthAsync(id, month, _authService.UserId, cancellationToken);

        return test == null ? null : MapToDto(test);
    }

    public async Task<BackflowTestDto?> UpdateIsCurrentAsync(int id, bool isCurrent, CancellationToken cancellationToken = default)
    {
        var test = await _testRepository.UpdateIsCurrentAsync(id, isCurrent, _authService.UserId, cancellationToken);

        return test == null ? null : MapToDto(test);
    }

    public async Task<BackflowTestDto?> UpdateOutOfServiceAsync(int id, bool outOfService, CancellationToken cancellationToken = default)
    {
        var test = await _testRepository.UpdateOutOfServiceAsync(id, outOfService, _authService.UserId, cancellationToken);

        return test == null ? null : MapToDto(test);
    }

    public async Task<BackflowTestDto?> UpdateDisapprovalAsync(int id, bool disapproved, CancellationToken cancellationToken = default)
    {
        var test = await _testRepository.UpdateDisapprovalAsync(id, disapproved, _authService.UserId, cancellationToken);

        return test == null ? null : MapToDto(test);
    }

    public async Task<BackflowTestDto?> UpdateForceRenewalAsync(int id, BackflowTestForceRenewalRequest request, CancellationToken cancellationToken = default)
    {
        var forceRenewalYears = request.ForceRenewalYears ?? 0;

        var test = await _testRepository.UpdateForceRenewalAsync(id, request.ForceRenewal, forceRenewalYears, _authService.UserId, cancellationToken);

        return test == null ? null : MapToDto(test);
    }

    public async Task<BackflowTestDto?> UpdateRejectionAsync(int id, BackflowTestRejectionRequest request, CancellationToken cancellationToken = default)
    {
        var test = await _testRepository.UpdateRejectionAsync(id, request.Rejected, request.RejectedReason, _authService.UserId, cancellationToken);

        return test == null ? null : MapToDto(test);
    }

    private static (bool RenewalRequired, DateTime? ExpirationDate) ComputeRenewal(
        BackflowTest test,
        IEnumerable<BackflowRenewalRequirementDto> requirements)
    {
        var matched = requirements.FirstOrDefault(r => DoesTestMatchRequirement(test, r));

        bool renewalRequired = matched != null;
        DateTime? expirationDate = null;

        if (matched != null && test.TestResult == BackflowTestResult.Pass)
        {
            expirationDate = (test.TestDate ?? DateTime.UtcNow).AddYears(matched.RenewalYears);
        }

        return (renewalRequired, expirationDate);
    }

    private static bool DoesTestMatchRequirement(BackflowTest test, BackflowRenewalRequirementDto requirement)
    {
        if (!string.IsNullOrEmpty(test.DeviceType) && SkippedDeviceTypes.Contains(test.DeviceType))
            return false;

        if ((PropertyType)test.PropertyType != requirement.PropertyType)
            return false;

        var deviceTypeIsAll = string.IsNullOrEmpty(requirement.DeviceType) || string.Equals(requirement.DeviceType, "All", StringComparison.OrdinalIgnoreCase);
        if (!deviceTypeIsAll && !string.Equals(test.DeviceType, requirement.DeviceType, StringComparison.OrdinalIgnoreCase))
            return false;

        var hazardTypeIsAll = string.IsNullOrEmpty(requirement.HazardType) || string.Equals(requirement.HazardType, "All", StringComparison.OrdinalIgnoreCase);
        if (!hazardTypeIsAll && !string.Equals(test.HazardType, requirement.HazardType, StringComparison.OrdinalIgnoreCase))
            return false;

        if (requirement.HasSiteOssf && !test.Ossf)
            return false;

        if (requirement.AuxWaterSupply && test.Site?.HasAuxWaterSupply != true)
            return false;

        return true;
    }
}
