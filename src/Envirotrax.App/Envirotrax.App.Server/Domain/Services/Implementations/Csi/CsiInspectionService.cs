using System.Transactions;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Implementations.Sites;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionService : Service<CsiInspection, CsiInspectionDto>, ICsiInspectionService
{
    private readonly ICsiInspectionRepository _repository;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly ISiteService _siteService;
    private readonly IPdfTemplateService _pdfTemplateService;
    private readonly IAuthService _authService;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IProfessionalSupplierService _professionalSupplierService;
    private readonly IProfessionalInsuranceService _insuranceService;
    private readonly ICsiInspectionAssemblyService _assemblyService;
    private readonly IMailingInfoRedactionService _mailingInfoRedactionService;

    public CsiInspectionService(
        IMapper mapper,
        ICsiInspectionRepository repository,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        IProfessionalUserLicenseService licenseService,
        ISiteService siteService,
        IPdfTemplateService pdfTemplateService,
        IAuthService authService,
        IGeneralSettingsService generalSettingsService,
        IProfessionalSupplierService professionalSupplierService,
        IProfessionalInsuranceService insuranceService,
        ICsiInspectionAssemblyService assemblyService,
        IMailingInfoRedactionService mailingInfoRedactionService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _licenseService = licenseService;
        _siteService = siteService;
        _pdfTemplateService = pdfTemplateService;
        _authService = authService;
        _generalSettingsService = generalSettingsService;
        _professionalSupplierService = professionalSupplierService;
        _insuranceService = insuranceService;
        _assemblyService = assemblyService;
        _mailingInfoRedactionService = mailingInfoRedactionService;
    }

    public override async Task<CsiInspectionDto?> DeleteAsync(int id)
    {
        CsiInspection? deleted;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            deleted = await _repository.DeleteAsync(id);

            if (deleted == null || !string.IsNullOrEmpty(deleted.TransactionId))
            {
                return null;
            }

            await _assemblyService.DeleteByInspectionAsync(id, deleted.ProfessionalId, default);

            scope.Complete();
        }

        var dto = MapToDto(deleted)!;

        return await _mailingInfoRedactionService.RedactAsync(dto, default);
    }

    public Task<InsuranceCheckDto> GetInsuranceCheckAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return _insuranceService.CheckForWaterSupplierAsync(_authService.ProfessionalId, waterSupplierId, ProfessionalType.CsiInspector, cancellationToken);
    }

    public async Task<CsiInspectionDto> SubmitAsync(CreateCsiInspectionDto request, CancellationToken cancellationToken)
    {
        var siteId = request.Site!.Id.Value;
        var waterSupplierId = request.WaterSupplier!.Id.Value;
        var inspectorUserId = request.InspectorUser!.Id.Value;

        await _insuranceService.EnsureSatisfiedForWaterSupplierAsync(_authService.ProfessionalId, waterSupplierId, ProfessionalType.CsiInspector, cancellationToken);

        var site = await _siteService.GetAsync(siteId, cancellationToken);
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var inspectorUser = await _professionalUserService.GetAsync(inspectorUserId, cancellationToken);
        var licenses = await _licenseService.GetAllAsync(inspectorUserId, new PageInfo(), new Query());

        var csiLicense = licenses.Data.FirstOrDefault();

        var inspection = new CsiInspection
        {
            WaterSupplierId = waterSupplierId,
            SiteId = siteId,
            InspectionDate = request.InspectionDate,
            ReasonForInspection = request.ReasonForInspection,
            Compliance1 = request.Compliance1,
            Compliance2 = request.Compliance2,
            Compliance3 = request.Compliance3,
            Compliance4 = request.Compliance4,
            Compliance5 = request.Compliance5,
            Compliance6 = request.Compliance6,
            MaterialServiceLineLead = request.MaterialServiceLineLead,
            MaterialServiceLineCopper = request.MaterialServiceLineCopper,
            MaterialServiceLinePVC = request.MaterialServiceLinePVC,
            MaterialServiceLineOther = request.MaterialServiceLineOther,
            MaterialServiceLineOtherDescription = request.MaterialServiceLineOtherDescription,
            MaterialSolderLead = request.MaterialSolderLead,
            MaterialSolderLeadFree = request.MaterialSolderLeadFree,
            MaterialSolderSolventWeld = request.MaterialSolderSolventWeld,
            MaterialSolderOther = request.MaterialSolderOther,
            MaterialSolderOtherDescription = request.MaterialSolderOtherDescription,
            Comments = request.Comments
        };

        await _mailingInfoRedactionService.KeepSiteLocationWhenRedactedAsync(request, site, waterSupplierId, cancellationToken);

        ApplyEnteredLocation(inspection, request);
        ApplySiteValidation(inspection, site);
        ApplyInspectorSnapshot(inspection, professional, inspectorUser, csiLicense, inspectorUserId);
        await ApplyAmountAsync(inspection, site.IsFeeExempt, site.PropertyType, cancellationToken);

        CsiInspection added;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            added = await _repository.AddAsync(inspection);
            await _assemblyService.SaveForInspectionAsync(added, request, cancellationToken);

            scope.Complete();
        }

        var dto = Mapper.Map<CsiInspectionDto>(added);

        return await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
    }

    private async Task ApplyAmountAsync(CsiInspection inspection, bool siteIsFeeExempt, PropertyType propertyType, CancellationToken cancellationToken)
    {
        inspection.Amount = 0;
        inspection.AmountShare = 0;

        if (siteIsFeeExempt)
        {
            return;
        }

        var isResidential = propertyType == PropertyType.Residential;

        var settings = await _generalSettingsService.GetAsync(inspection.WaterSupplierId, cancellationToken);
        var fee = isResidential ? settings?.CsiResidentialInspectionFee ?? 0 : settings?.CsiCommercialInspectionFee ?? 0;
        var feeShare = isResidential ? settings?.CsiResidentialInspectionFeeWsShare ?? 0 : settings?.CsiCommercialInspectionFeeWsShare ?? 0;

        var registration = await _professionalSupplierService.GetAsync(inspection.WaterSupplierId, cancellationToken);
        var feeOverride = isResidential ? registration?.CsiResidentialInspectionFee : registration?.CsiCommercialInspectionFee;

        inspection.Amount = feeOverride ?? fee;
        inspection.AmountShare = feeShare;
    }

    // Checkout "Edit" on an own, still-unpaid inspection: mirrors SubmitAsync's field list and snapshot
    // logic, but against an existing row. Ownership is enforced by ProfessionalDbContext (CsiInspection
    // is an ISharedProfessionalModel); the repository's own guard only covers not-found/already-paid.
    public async Task<CsiInspectionDto?> UpdateForProfessionalAsync(int id, CreateCsiInspectionDto request, CancellationToken cancellationToken)
    {
        var existing = await _repository.GetNoIncludesAsync(id, cancellationToken);

        if (existing == null)
        {
            return null;
        }

        var inspectorUserId = request.InspectorUser!.Id!.Value;

        // An edit can't move the inspection to another site or water supplier (the repository keeps both), so the
        // location is checked, and redaction decided, against the inspection's own ones rather than the request's.
        var site = await _siteService.GetAsync(existing.SiteId, cancellationToken);
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var inspectorUser = await _professionalUserService.GetAsync(inspectorUserId, cancellationToken);
        var licenses = await _licenseService.GetAllAsync(inspectorUserId, new PageInfo(), new Query());

        var csiLicense = licenses.Data.FirstOrDefault();

        var inspection = new CsiInspection
        {
            Id = id,
            InspectionDate = request.InspectionDate,
            ReasonForInspection = request.ReasonForInspection,
            Compliance1 = request.Compliance1,
            Compliance2 = request.Compliance2,
            Compliance3 = request.Compliance3,
            Compliance4 = request.Compliance4,
            Compliance5 = request.Compliance5,
            Compliance6 = request.Compliance6,
            MaterialServiceLineLead = request.MaterialServiceLineLead,
            MaterialServiceLineCopper = request.MaterialServiceLineCopper,
            MaterialServiceLinePVC = request.MaterialServiceLinePVC,
            MaterialServiceLineOther = request.MaterialServiceLineOther,
            MaterialServiceLineOtherDescription = request.MaterialServiceLineOtherDescription,
            MaterialSolderLead = request.MaterialSolderLead,
            MaterialSolderLeadFree = request.MaterialSolderLeadFree,
            MaterialSolderSolventWeld = request.MaterialSolderSolventWeld,
            MaterialSolderOther = request.MaterialSolderOther,
            MaterialSolderOtherDescription = request.MaterialSolderOtherDescription,
            Comments = request.Comments
        };

        await _mailingInfoRedactionService.KeepSiteLocationWhenRedactedAsync(request, site, existing.WaterSupplierId, cancellationToken);

        ApplyEnteredLocation(inspection, request);
        ApplySiteValidation(inspection, site);
        ApplyInspectorSnapshot(inspection, professional, inspectorUser, csiLicense, inspectorUserId);

        CsiInspection? saved;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            saved = await _repository.UpdateForProfessionalAsync(inspection);

            if (saved == null)
            {
                return null;
            }

            await _assemblyService.SaveForInspectionAsync(saved, request, cancellationToken);

            scope.Complete();
        }

        var dto = Mapper.Map<CsiInspectionDto>(saved);

        return await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
    }

    public async Task<CsiInspectionDto?> UpdateApprovalAsync(int id, CsiInspectionApprovalRequest request, CancellationToken cancellationToken)
    {
        var saved = await _repository.UpdateApprovalAsync(id, request, cancellationToken);

        if (saved == null)
        {
            return null;
        }

        return Mapper.Map<CsiInspectionDto>(saved);
    }

    public async Task<CsiInspectionDto?> UpdateForAdminAsync(int id, CsiInspectionAdminUpdateRequest request)
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

        return Mapper.Map<CsiInspectionDto>(updated);
    }

    public async Task<IPagedData<CsiInspectionDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<CsiInspection, CsiInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<CsiInspection, CsiInspectionDto>(Mapper);
        var inspections = await _repository.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
        var dtos = inspections.Select(m => Mapper.Map<CsiInspectionDto>(m)!).ToPagedData(pageInfo);

        return await _mailingInfoRedactionService.RedactAsync(dtos, cancellationToken);
    }

    public async Task<CsiInspectionDto?> GetForProfessionalAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await GetAsync(id, cancellationToken);

        if (dto != null)
        {
            await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
        }

        return dto;
    }

    public async Task<IPagedData<CsiInspectionDto>> SearchForAdminAsync(PageInfo pageInfo, Query query, CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<CsiInspection, CsiInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<CsiInspection, CsiInspectionDto>(Mapper);

        var inspections = await _repository.SearchForAdminAsync(pageInfo, query, paymentStatus, cancellationToken);

        return inspections.Select(m => Mapper.Map<CsiInspectionDto>(m)!).ToPagedData(pageInfo);
    }

    private static void ApplyEnteredLocation(CsiInspection inspection, CsiInspectionDto request)
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

    private static void ApplySiteValidation(CsiInspection inspection, DataTransferObjects.Sites.SiteDto site)
    {
        inspection.ValidationSiteInformationChanged = HasSiteInformationChanged(inspection, site);
        inspection.NeedsValidation = inspection.ValidationSiteInformationChanged;
    }

    private static bool HasSiteInformationChanged(CsiInspection inspection, DataTransferObjects.Sites.SiteDto site)
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

    public Task<byte[]> GeneratePdfAsync(CsiInspectionDto inspection)
    {
        return GeneratePdfAsync([inspection]);
    }

    public Task<byte[]> GeneratePdfAsync(IEnumerable<CsiInspectionDto> inspections)
    {
        return _pdfTemplateService.GenerateAsync("Csi.CsiInspection", inspections);
    }

    public Task<byte[]> GeneratePdfForProfessionalAsync(CsiInspectionDto inspection)
    {
        if (inspection.TransactionId == null)
        {
            throw new AppValidationException("Report can't be downloaded until it's paid. Please go to checkout and pay for this transaction, then try downloading again.");
        }

        return GeneratePdfAsync(inspection);
    }

    private static void ApplyInspectorSnapshot(
        CsiInspection inspection,
        ProfessionalDto professional,
        ProfessionalUserDto? inspectorUser,
        ProfessionalUserLicenseDto? csiLicense,
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
        inspection.InspectorLicenseNumber = csiLicense?.LicenseNumber;
        inspection.InspectorLicenseType = csiLicense?.LicenseType?.Name;
    }
}
