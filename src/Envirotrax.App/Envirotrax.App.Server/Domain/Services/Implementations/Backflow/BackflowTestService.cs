using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Backflow;
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
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
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
    private readonly IBackflowRenewalRequirementService _renewalRequirementService;
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
        IBackflowRenewalRequirementService renewalRequirementService,
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
        _renewalRequirementService = renewalRequirementService;
        _logger = logger;
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

                await _testRepository.UpdateTestRenewalAsync(test.Id, renewalRequired, newExpirationDate, cancellationToken);
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
        if (test == null || test.DeletedTime.HasValue) return;

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
