using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Implementations.Sites;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogInspectionService : Service<FogInspection, FogInspectionDto>, IFogInspectionService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff"];

    private static readonly HashSet<string> ValidImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "exterior",
        "interior"
    };

    private readonly IFogInspectionRepository _repository;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly ISiteService _siteService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthService _authService;
    private readonly IPdfTemplateService _pdfTemplateService;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IProfessionalSupplierService _professionalSupplierService;
    private readonly IMailingInfoRedactionService _mailingInfoRedactionService;

    public FogInspectionService(
        IMapper mapper,
        IFogInspectionRepository repository,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        ISiteService siteService,
        IFileStorageService fileStorageService,
        IAuthService authService,
        IPdfTemplateService pdfTemplateService,
        IGeneralSettingsService generalSettingsService,
        IProfessionalSupplierService professionalSupplierService,
        IMailingInfoRedactionService mailingInfoRedactionService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _siteService = siteService;
        _fileStorageService = fileStorageService;
        _authService = authService;
        _pdfTemplateService = pdfTemplateService;
        _generalSettingsService = generalSettingsService;
        _professionalSupplierService = professionalSupplierService;
        _mailingInfoRedactionService = mailingInfoRedactionService;
    }

    public Task<byte[]> GeneratePdfAsync(FogInspectionDto inspection)
    {
        return GeneratePdfAsync([inspection]);
    }

    public Task<byte[]> GeneratePdfAsync(IEnumerable<FogInspectionDto> inspections)
    {
        return _pdfTemplateService.GenerateAsync("Fog.FogInspection", inspections);
    }

    public Task<byte[]> GeneratePdfForProfessionalAsync(FogInspectionDto inspection)
    {
        if (inspection.TransactionId == null)
        {
            throw new AppValidationException("Report can't be downloaded until it's paid. Please go to checkout and pay for this transaction, then try downloading again.");
        }

        return GeneratePdfAsync(inspection);
    }

    public override async Task<FogInspectionDto?> DeleteAsync(int id)
    {
        FogInspection? deleted;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            deleted = await _repository.DeleteAsync(id);

            if (deleted == null || !string.IsNullOrEmpty(deleted.TransactionId))
            {
                return null;
            }

            scope.Complete();
        }

        var dto = MapToDto(deleted)!;

        return await _mailingInfoRedactionService.RedactAsync(dto, CancellationToken.None);
    }

    public async Task<FogInspectionDto> SubmitAsync(
        FogInspectionDto request,
        Stream? exteriorStream, string? exteriorFileName,
        Stream? interiorStream, string? interiorFileName,
        Stream? signatureStream, string? signatureFileName,
        CancellationToken cancellationToken)
    {
        var siteId = request.Site!.Id!.Value;
        var waterSupplierId = request.WaterSupplier!.Id!.Value;
        var inspectorUserId = request.Inspector!.Id!.Value;

        var site = await _siteService.GetAsync(siteId, cancellationToken);
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var inspectorUser = await _professionalUserService.GetAsync(inspectorUserId, cancellationToken);

        var inspection = new FogInspection
        {
            WaterSupplierId = waterSupplierId,
            SiteId = siteId,
            InspectionDate = request.InspectionDate,
            FacilityType = request.FacilityType,
            ReasonForInspection = request.ReasonForInspection,

            InterceptorType = request.InterceptorType,
            InterceptorOtherDescription = request.InterceptorOtherDescription,
            InterceptorCapacity = request.InterceptorCapacity,
            InterceptorCapacityType = request.InterceptorCapacityType,
            InterceptorLocationDescription = request.InterceptorLocationDescription,
            InterceptorLatitude = request.InterceptorLatitude,
            InterceptorLongitude = request.InterceptorLongitude,
            InterceptorComments = request.InterceptorComments,

            Maintained = request.Maintained,
            Accessible = request.Accessible,
            PastOverflow = request.PastOverflow,

            InletChamberWettingHeight = request.InletChamberWettingHeight,
            InletChamberGreaseBlanket = request.InletChamberGreaseBlanket,
            InletChamberSediments = request.InletChamberSediments,
            OutletChamberWettingHeight = request.OutletChamberWettingHeight,
            OutletChamberGreaseBlanket = request.OutletChamberGreaseBlanket,
            OutletChamberSediments = request.OutletChamberSediments,
            InletTeeIntact = request.InletTeeIntact,
            OutletTeeIntact = request.OutletTeeIntact,
            InletTeeVisible = request.InletTeeVisible,
            OutletTeeVisible = request.OutletTeeVisible,

            SampledFrom = request.SampledFrom,
            SamplingPointAccessible = request.SamplingPointAccessible,
            SamplingPointClean = request.SamplingPointClean,

            InletTotalCapacityPercent = request.InletTotalCapacityPercent,
            OutletTotalCapacityPercent = request.OutletTotalCapacityPercent,
            TotalCapacityPercent = request.TotalCapacityPercent,

            InspectionResult = request.InspectionResult,

            SignatureContactName = request.SignatureContactName,
            SignatureDate = request.SignatureDate,

            Comments = request.Comments,

            FogGeneratorPhoneNumber = request.FogGeneratorPhoneNumber,
            FogGeneratorEmailAddress = request.FogGeneratorEmailAddress
        };

        await _mailingInfoRedactionService.KeepSiteLocationWhenRedactedAsync(request, site, waterSupplierId, cancellationToken);

        ApplyEnteredLocation(inspection, request);
        ApplySiteValidation(inspection, site);
        ApplyInspectorSnapshot(inspection, professional, inspectorUser, inspectorUserId);
        await ApplyAmountAsync(inspection, site.IsFeeExempt, cancellationToken);

        // Set image paths before AddAsync so they persist with the initial insert (both optional).
        if (exteriorStream != null && exteriorFileName != null)
        {
            inspection.ExteriorImagePath = $"professionals/{professional.Id}/fog-inspections/exterior/{Guid.NewGuid()}{ValidateAndGetExtension(exteriorFileName)}";
        }
        if (interiorStream != null && interiorFileName != null)
        {
            inspection.InteriorImagePath = $"professionals/{professional.Id}/fog-inspections/interior/{Guid.NewGuid()}{ValidateAndGetExtension(interiorFileName)}";
        }
        if (signatureStream != null && signatureFileName != null)
        {
            inspection.SignatureImagePath = $"professionals/{professional.Id}/fog-inspections/signature/{Guid.NewGuid()}{ValidateAndGetExtension(signatureFileName)}";
            inspection.SignatureDate = DateTime.UtcNow;
        }

        FogInspection added;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            added = await _repository.AddAsync(inspection);

            if (exteriorStream != null && inspection.ExteriorImagePath != null)
            {
                await _fileStorageService.UploadAsync(inspection.ExteriorImagePath, exteriorStream);
            }
            if (interiorStream != null && inspection.InteriorImagePath != null)
            {
                await _fileStorageService.UploadAsync(inspection.InteriorImagePath, interiorStream);
            }
            if (signatureStream != null && inspection.SignatureImagePath != null)
            {
                await _fileStorageService.UploadAsync(inspection.SignatureImagePath, signatureStream);
            }

            scope.Complete();
        }

        var dto = Mapper.Map<FogInspectionDto>(added);

        return await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
    }

    private async Task ApplyAmountAsync(FogInspection inspection, bool siteIsFeeExempt, CancellationToken cancellationToken)
    {
        inspection.Amount = 0;
        inspection.AmountShare = 0;

        if (siteIsFeeExempt)
        {
            return;
        }

        var settings = await _generalSettingsService.GetAsync(inspection.WaterSupplierId, cancellationToken);
        var registration = await _professionalSupplierService.GetAsync(inspection.WaterSupplierId, cancellationToken);

        inspection.Amount = registration?.FogInspectorFee ?? settings?.FogInspectorFee ?? 0;
        inspection.AmountShare = settings?.FogInspectorFeeWsShare ?? 0;
    }

    // Checkout "Edit" on an own, still-unpaid inspection: mirrors SubmitAsync's field list and snapshot
    // logic, but against an existing row. Ownership is enforced by ProfessionalDbContext (FogInspection
    // is an ISharedProfessionalModel); the repository's own guard only covers not-found/already-paid.
    public async Task<FogInspectionDto?> UpdateForProfessionalAsync(
        int id,
        FogInspectionDto request,
        Stream? exteriorStream, string? exteriorFileName,
        Stream? interiorStream, string? interiorFileName,
        Stream? signatureStream, string? signatureFileName,
        CancellationToken cancellationToken)
    {
        var existing = await _repository.GetNoIncludesAsync(id, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        var inspectorUserId = request.Inspector!.Id!.Value;

        // An edit can't move the inspection to another site or water supplier (the repository keeps both), so the
        // location is checked, and redaction decided, against the inspection's own ones rather than the request's.
        var site = await _siteService.GetAsync(existing.SiteId, cancellationToken);
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var inspectorUser = await _professionalUserService.GetAsync(inspectorUserId, cancellationToken);

        var inspection = new FogInspection
        {
            Id = id,
            InspectionDate = request.InspectionDate,
            FacilityType = request.FacilityType,
            ReasonForInspection = request.ReasonForInspection,

            InterceptorType = request.InterceptorType,
            InterceptorOtherDescription = request.InterceptorOtherDescription,
            InterceptorCapacity = request.InterceptorCapacity,
            InterceptorCapacityType = request.InterceptorCapacityType,
            InterceptorLocationDescription = request.InterceptorLocationDescription,
            InterceptorLatitude = request.InterceptorLatitude,
            InterceptorLongitude = request.InterceptorLongitude,
            InterceptorComments = request.InterceptorComments,

            Maintained = request.Maintained,
            Accessible = request.Accessible,
            PastOverflow = request.PastOverflow,

            InletChamberWettingHeight = request.InletChamberWettingHeight,
            InletChamberGreaseBlanket = request.InletChamberGreaseBlanket,
            InletChamberSediments = request.InletChamberSediments,
            OutletChamberWettingHeight = request.OutletChamberWettingHeight,
            OutletChamberGreaseBlanket = request.OutletChamberGreaseBlanket,
            OutletChamberSediments = request.OutletChamberSediments,
            InletTeeIntact = request.InletTeeIntact,
            OutletTeeIntact = request.OutletTeeIntact,
            InletTeeVisible = request.InletTeeVisible,
            OutletTeeVisible = request.OutletTeeVisible,

            SampledFrom = request.SampledFrom,
            SamplingPointAccessible = request.SamplingPointAccessible,
            SamplingPointClean = request.SamplingPointClean,

            InletTotalCapacityPercent = request.InletTotalCapacityPercent,
            OutletTotalCapacityPercent = request.OutletTotalCapacityPercent,
            TotalCapacityPercent = request.TotalCapacityPercent,

            InspectionResult = request.InspectionResult,

            SignatureContactName = request.SignatureContactName,
            SignatureDate = request.SignatureDate,

            Comments = request.Comments,

            FogGeneratorPhoneNumber = request.FogGeneratorPhoneNumber,
            FogGeneratorEmailAddress = request.FogGeneratorEmailAddress
        };

        await _mailingInfoRedactionService.KeepSiteLocationWhenRedactedAsync(request, site, existing.WaterSupplierId, cancellationToken);

        ApplyEnteredLocation(inspection, request);
        ApplySiteValidation(inspection, site);
        ApplyInspectorSnapshot(inspection, professional, inspectorUser, inspectorUserId);

        string? newExteriorPath = null;
        string? newInteriorPath = null;
        string? newSignaturePath = null;

        if (exteriorStream != null && exteriorFileName != null)
        {
            newExteriorPath = $"professionals/{professional.Id}/fog-inspections/exterior/{Guid.NewGuid()}{ValidateAndGetExtension(exteriorFileName)}";
        }
        if (interiorStream != null && interiorFileName != null)
        {
            newInteriorPath = $"professionals/{professional.Id}/fog-inspections/interior/{Guid.NewGuid()}{ValidateAndGetExtension(interiorFileName)}";
        }
        if (signatureStream != null && signatureFileName != null)
        {
            newSignaturePath = $"professionals/{professional.Id}/fog-inspections/signature/{Guid.NewGuid()}{ValidateAndGetExtension(signatureFileName)}";
        }

        FogInspection? saved;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            saved = await _repository.UpdateForProfessionalAsync(inspection, newExteriorPath, newInteriorPath, newSignaturePath);

            if (saved == null)
            {
                return null;
            }

            if (newExteriorPath != null)
            {
                await _fileStorageService.UploadAsync(newExteriorPath, exteriorStream!);
            }
            if (newInteriorPath != null)
            {
                await _fileStorageService.UploadAsync(newInteriorPath, interiorStream!);
            }
            if (newSignaturePath != null)
            {
                await _fileStorageService.UploadAsync(newSignaturePath, signatureStream!);
            }

            scope.Complete();
        }

        var dto = Mapper.Map<FogInspectionDto>(saved);

        await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
        await PopulateImageUrlsAsync(dto);

        return dto;
    }

    public override async Task<FogInspectionDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await base.GetAsync(id, cancellationToken);

        if (dto != null)
        {
            await PopulateImageUrlsAsync(dto);
        }

        return dto;
    }

    public async Task<FogInspectionDto?> GetForProfessionalAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await GetAsync(id, cancellationToken);

        if (dto != null)
        {
            await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
        }

        return dto;
    }

    public async Task<IPagedData<FogInspectionDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogInspection, FogInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogInspection, FogInspectionDto>(Mapper);

        var inspections = await _repository.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
        var dtos = inspections.Select(m => Mapper.Map<FogInspectionDto>(m)!).ToPagedData(pageInfo);

        return await _mailingInfoRedactionService.RedactAsync(dtos, cancellationToken);
    }

    private async Task PopulateImageUrlsAsync(FogInspectionDto dto)
    {
        var images = new (string? Path, Action<string> SetUrl)[]
        {
            (dto.ExteriorImagePath, url => dto.ExteriorImageUrl = url),
            (dto.InteriorImagePath, url => dto.InteriorImageUrl = url),
            (dto.SignatureImagePath, url => dto.SignatureImageUrl = url)
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

    private static string ValidateAndGetExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName);

        if (!AllowedFileExtensions.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            throw new ValidationException($"Only {string.Join(", ", AllowedFileExtensions)} files are accepted.");
        }

        return ext;
    }

    private static void ApplyEnteredLocation(FogInspection inspection, FogInspectionDto request)
    {
        inspection.PropertyBusinessName = request.PropertyBusinessName;
        inspection.PropertyType = request.PropertyType;
        inspection.PropertyStreetNumber = request.PropertyStreetNumber;
        inspection.PropertyStreetName = request.PropertyStreetName;
        inspection.PropertyNumber = request.PropertyNumber;
        inspection.PropertyCity = request.PropertyCity;
        inspection.PropertyStateId = SiteInformationComparer.GetStateId(request.PropertyState);
        inspection.PropertyZip = request.PropertyZip;

        inspection.MailingCompanyName = request.MailingCompanyName;
        inspection.MailingContactName = request.MailingContactName;
        inspection.MailingStreetNumber = request.MailingStreetNumber;
        inspection.MailingStreetName = request.MailingStreetName;
        inspection.MailingNumber = request.MailingNumber;
        inspection.MailingCity = request.MailingCity;
        inspection.MailingStateId = SiteInformationComparer.GetStateId(request.MailingState);
        inspection.MailingZip = request.MailingZip;
        inspection.MailingPhoneNumber = request.MailingPhoneNumber;
        inspection.MailingEmailAddress = request.MailingEmailAddress;
    }

    private static void ApplySiteValidation(FogInspection inspection, SiteDto site)
    {
        inspection.ValidationSiteInformationChanged = HasSiteInformationChanged(inspection, site);
        inspection.NeedsValidation = inspection.ValidationSiteInformationChanged;
    }

    private static bool HasSiteInformationChanged(FogInspection inspection, SiteDto site)
    {
        if (inspection.PropertyType != site.PropertyType)
        {
            return true;
        }

        if (inspection.PropertyStateId != SiteInformationComparer.GetStateId(site.State))
        {
            return true;
        }

        if (inspection.MailingStateId != SiteInformationComparer.GetStateId(site.MailingState))
        {
            return true;
        }

        var textFields = new List<(string? EnteredValue, string? SiteValue)>
        {
            (inspection.PropertyBusinessName, site.BusinessName),
            (inspection.PropertyStreetNumber, site.StreetNumber),
            (inspection.PropertyStreetName, site.StreetName),
            (inspection.PropertyNumber, site.PropertyNumber),
            (inspection.PropertyCity, site.City),
            (inspection.PropertyZip, site.ZipCode),
            (inspection.MailingCompanyName, site.MailingCompanyName),
            (inspection.MailingContactName, site.MailingContactName),
            (inspection.MailingStreetNumber, site.MailingStreetNumber),
            (inspection.MailingStreetName, site.MailingStreetName),
            (inspection.MailingNumber, site.MailingNumber),
            (inspection.MailingCity, site.MailingCity),
            (inspection.MailingZip, site.MailingZipCode),
            (inspection.MailingPhoneNumber, site.MailingPhoneNumber),
            (inspection.MailingEmailAddress, site.MailingEmailAddress)
        };

        return SiteInformationComparer.HasTextChanged(textFields);
    }


    public async Task<FogInspectionDto?> UpdateForAdminAsync(int id, FogInspectionAdminUpdateRequest request)
    {
        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var saved = await _repository.UpdateForAdminAsync(id, request);

            if (saved == null)
            {
                return null;
            }

            scope.Complete();
        }

        var updated = await _repository.GetAsync(id, default);

        var dto = Mapper.Map<FogInspectionDto>(updated);
        await PopulateImageUrlsAsync(dto);

        return dto;
    }

    public async Task<FogInspectionDto?> UpdateImageForAdminAsync(int id, string imageType, Stream fileStream, string fileName)
    {
        if (!ValidImageTypes.Contains(imageType))
        {
            throw new ValidationException("Invalid image type.");
        }

        var existing = await _repository.GetAsync(id, default);

        if (existing == null)
        {
            return null;
        }

        var oldPath = GetImagePath(existing, imageType);

        var newPath = $"fog-inspections/{id}/{imageType.ToLowerInvariant()}/{Guid.NewGuid()}{ValidateAndGetExtension(fileName)}";

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var saved = await _repository.UpdateImagePathAsync(id, GetImagePathPropertyName(imageType), newPath);

            if (saved == null)
            {
                return null;
            }

            await _fileStorageService.UploadAsync(newPath, fileStream);

            scope.Complete();
        }

        if (!string.IsNullOrWhiteSpace(oldPath))
        {
            await _fileStorageService.DeleteAsync(oldPath);
        }

        var updated = await _repository.GetAsync(id, default);

        var dto = Mapper.Map<FogInspectionDto>(updated);
        await PopulateImageUrlsAsync(dto);

        return dto;
    }

    private static string? GetImagePath(FogInspection inspection, string imageType) => imageType.ToLowerInvariant() switch
    {
        "exterior" => inspection.ExteriorImagePath,
        "interior" => inspection.InteriorImagePath,
        _ => null
    };

    private static string GetImagePathPropertyName(string imageType) => imageType.ToLowerInvariant() switch
    {
        "exterior" => nameof(FogInspection.ExteriorImagePath),
        "interior" => nameof(FogInspection.InteriorImagePath),
        _ => throw new ValidationException("Invalid image type.")
    };

    private static void ApplyInspectorSnapshot(
        FogInspection inspection,
        ProfessionalDto professional,
        ProfessionalUserDto? inspectorUser,
        int inspectorUserId)
    {
        inspection.ProfessionalId = professional.Id;
        inspection.InspectorId = inspectorUserId;
        inspection.InspectorCompanyName = professional.Name;
        inspection.InspectorContactName = inspectorUser?.ContactName;
        inspection.InspectorJobTitle = inspectorUser?.JobTitle;
        inspection.InspectorAddress = professional.Address;
        inspection.InspectorCity = professional.City;
        inspection.InspectorState = professional.State?.Name;
        inspection.InspectorZip = professional.ZipCode;
        inspection.InspectorWorkNumber = professional.PhoneNumber;
        inspection.InspectorCellNumber = inspectorUser?.PhoneNumber;
        inspection.InspectorFaxNumber = professional.FaxNumber;
    }
}
