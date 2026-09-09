using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogInspectionService : Service<FogInspection, FogInspectionDto>, IFogInspectionService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff"];

    private readonly IFogInspectionRepository _repository;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly ISiteService _siteService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuthService _authService;
    private readonly IPdfTemplateService _pdfTemplateService;
    private readonly IRecordLogService _recordLogService;

    public FogInspectionService(
        IMapper mapper,
        IFogInspectionRepository repository,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        ISiteService siteService,
        IFileStorageService fileStorageService,
        IAuthService authService,
        IPdfTemplateService pdfTemplateService,
        IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _siteService = siteService;
        _fileStorageService = fileStorageService;
        _authService = authService;
        _pdfTemplateService = pdfTemplateService;
        _recordLogService = recordLogService;
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
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var deleted = await _repository.DeleteAsync(id);

        if (deleted == null || deleted.ProfessionalId != _authService.ProfessionalId || !string.IsNullOrEmpty(deleted.TransactionId))
        {
            return null;
        }

        scope.Complete();
        return MapToDto(deleted);
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
            FogGeneratorEmailAddress = request.FogGeneratorEmailAddress,

            NeedsValidation = true
        };

        ApplySiteSnapshot(inspection, site);
        ApplyInspectorSnapshot(inspection, professional, inspectorUser, inspectorUserId);

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

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var added = await _repository.AddAsync(inspection);

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
        return Mapper.Map<FogInspectionDto>(added);
    }

    // Checkout "Edit" on an own, still-unpaid inspection: mirrors SubmitAsync's field list and snapshot
    // logic, but against an existing row. Ownership + payment-status guard lives in the repository
    // (UpdateForProfessionalAsync returns Model == null for not-found/not-owned/already-paid).
    public async Task<FogInspectionDto?> UpdateForProfessionalAsync(
        int id,
        FogInspectionDto request,
        Stream? exteriorStream, string? exteriorFileName,
        Stream? interiorStream, string? interiorFileName,
        Stream? signatureStream, string? signatureFileName,
        CancellationToken cancellationToken)
    {
        var professionalId = _authService.ProfessionalId;
        var siteId = request.Site!.Id!.Value;
        var inspectorUserId = request.Inspector!.Id!.Value;

        var site = await _siteService.GetAsync(siteId, cancellationToken);
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

        ApplySiteSnapshot(inspection, site);
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

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var saved = await _repository.UpdateForProfessionalAsync(inspection, professionalId, newExteriorPath, newInteriorPath, newSignaturePath);

        if (saved.Model == null)
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

        if (saved.Changes.Length > 0)
        {
            await _recordLogService.AddAsync(RecordLogTableNames.FogInspections, saved.Model.Id, saved.Model.WaterSupplierId, RecordLogType.Edit, saved.Changes, professionalId);
        }

        scope.Complete();

        var dto = Mapper.Map<FogInspectionDto>(saved.Model);
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

    public async Task<IPagedData<FogInspectionDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogInspection, FogInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogInspection, FogInspectionDto>(Mapper);

        var inspections = await _repository.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);

        return inspections.Select(m => Mapper.Map<FogInspectionDto>(m)!).ToPagedData(pageInfo);
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

    private static void ApplySiteSnapshot(FogInspection inspection, SiteDto site)
    {
        inspection.PropertyBusinessName = site.BusinessName;
        inspection.PropertyType = site.PropertyType;
        inspection.PropertyStreetNumber = site.StreetNumber;
        inspection.PropertyStreetName = site.StreetName;
        inspection.PropertyNumber = site.PropertyNumber;
        inspection.PropertyCity = site.City;
        inspection.PropertyStateId = site.State?.Id;
        inspection.PropertyZip = site.ZipCode;
        inspection.MailingCompanyName = site.MailingCompanyName;
        inspection.MailingContactName = site.MailingContactName;
        inspection.MailingStreetNumber = site.MailingStreetNumber;
        inspection.MailingStreetName = site.MailingStreetName;
        inspection.MailingNumber = site.MailingNumber;
        inspection.MailingCity = site.MailingCity;
        inspection.MailingStateId = site.MailingState?.Id;
        inspection.MailingZip = site.MailingZipCode;
        inspection.MailingPhoneNumber = site.MailingPhoneNumber;
        inspection.MailingEmailAddress = site.MailingEmailAddress;
    }

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
        inspection.InspectorFaxNumber = professional.FaxNumber;
    }
}
