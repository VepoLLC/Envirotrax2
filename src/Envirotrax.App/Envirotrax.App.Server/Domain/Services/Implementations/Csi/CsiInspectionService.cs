using System.Text;
using System.Transactions;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
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
    private readonly IRecordLogService _recordLogService;

    public CsiInspectionService(
        IMapper mapper,
        ICsiInspectionRepository repository,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        IProfessionalUserLicenseService licenseService,
        ISiteService siteService,
        IPdfTemplateService pdfTemplateService,
        IAuthService authService,
        IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _licenseService = licenseService;
        _siteService = siteService;
        _pdfTemplateService = pdfTemplateService;
        _authService = authService;
        _recordLogService = recordLogService;
    }

    public override async Task<CsiInspectionDto?> DeleteAsync(int id)
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

    public async Task<CsiInspectionDto> SubmitAsync(CsiInspectionDto request, CancellationToken cancellationToken)
    {
        var siteId = request.Site!.Id.Value;
        var waterSupplierId = request.WaterSupplier!.Id.Value;
        var inspectorUserId = request.InspectorUser!.Id.Value;

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
            Comments = request.Comments,
            NeedsValidation = true
        };

        ApplySiteSnapshot(inspection, site);
        ApplyInspectorSnapshot(inspection, professional, inspectorUser, csiLicense, inspectorUserId);

        var added = await _repository.AddAsync(inspection);
        return Mapper.Map<CsiInspectionDto>(added);
    }

    public async Task<CsiInspectionDto?> UpdateApprovalAsync(int id, CsiInspectionApprovalRequest request, CancellationToken cancellationToken)
    {
        var inspection = await _repository.UpdateApprovalAsync(id, request, cancellationToken);
        return inspection == null ? null : Mapper.Map<CsiInspectionDto>(inspection);
    }

    public async Task<CsiInspectionDto?> UpdateForAdminAsync(int id, CsiInspectionAdminUpdateRequest request)
    {
        var inspection = await _repository.GetNoIncludesAsync(id, default);

        if (inspection == null)
        {
            return null;
        }

        var waterSupplierId = inspection.WaterSupplierId;
        var changes = BuildChangeDescription(inspection, request);

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            var saved = await _repository.UpdateForAdminAsync(id, request);

            if (saved == null)
            {
                return null;
            }

            if (changes.Length > 0)
            {
                await _recordLogService.AddAsync(RecordLogTableNames.CsiInspections, id, waterSupplierId, RecordLogType.Edit, changes);
            }

            scope.Complete();
        }

        var updated = await _repository.GetAsync(id, default);

        return Mapper.Map<CsiInspectionDto>(updated);
    }

    private static string BuildChangeDescription(CsiInspection inspection, CsiInspectionAdminUpdateRequest request)
    {
        var changes = new StringBuilder();

        void Append(string label, object? oldValue, object? newValue)
        {
            var oldText = FormatChangeValue(oldValue);
            var newText = FormatChangeValue(newValue);

            if (oldText == newText)
            {
                return;
            }

            if (changes.Length > 0)
            {
                changes.AppendLine();
            }

            changes.Append($"{label}: '{oldText}' -> '{newText}'");
        }

        Append("Property Type", inspection.PropertyType, request.PropertyType);
        Append("Property Business Name", inspection.PropertyBusinessName, request.PropertyBusinessName);
        Append("Property Street Number", inspection.PropertyStreetNumber, request.PropertyStreetNumber);
        Append("Property Street Name", inspection.PropertyStreetName, request.PropertyStreetName);
        Append("Property Number", inspection.PropertyNumber, request.PropertyNumber);
        Append("Property City", inspection.PropertyCity, request.PropertyCity);
        Append("Property State", inspection.PropertyStateId, request.PropertyState?.Id);
        Append("Property ZIP", inspection.PropertyZip, request.PropertyZip);

        Append("Mailing Company Name", inspection.MailingCompanyName, request.MailingCompanyName);
        Append("Mailing Contact Name", inspection.MailingContactName, request.MailingContactName);
        Append("Mailing Street Number", inspection.MailingStreetNumber, request.MailingStreetNumber);
        Append("Mailing Street Name", inspection.MailingStreetName, request.MailingStreetName);
        Append("Mailing Number", inspection.MailingNumber, request.MailingNumber);
        Append("Mailing City", inspection.MailingCity, request.MailingCity);
        Append("Mailing State", inspection.MailingStateId, request.MailingState?.Id);
        Append("Mailing ZIP", inspection.MailingZip, request.MailingZip);

        Append("Reason For Inspection", inspection.ReasonForInspection, request.ReasonForInspection);
        Append("Inspection Date", inspection.InspectionDate, request.InspectionDate);

        Append("Compliance 1", inspection.Compliance1, request.Compliance1);
        Append("Compliance 2", inspection.Compliance2, request.Compliance2);
        Append("Compliance 3", inspection.Compliance3, request.Compliance3);
        Append("Compliance 4", inspection.Compliance4, request.Compliance4);
        Append("Compliance 5", inspection.Compliance5, request.Compliance5);
        Append("Compliance 6", inspection.Compliance6, request.Compliance6);

        Append("Service Line Lead", inspection.MaterialServiceLineLead, request.MaterialServiceLineLead);
        Append("Service Line Copper", inspection.MaterialServiceLineCopper, request.MaterialServiceLineCopper);
        Append("Service Line PVC", inspection.MaterialServiceLinePVC, request.MaterialServiceLinePVC);
        Append("Service Line Other", inspection.MaterialServiceLineOther, request.MaterialServiceLineOther);
        Append("Service Line Other Description", inspection.MaterialServiceLineOtherDescription, request.MaterialServiceLineOtherDescription);

        Append("Solder Lead", inspection.MaterialSolderLead, request.MaterialSolderLead);
        Append("Solder Lead Free", inspection.MaterialSolderLeadFree, request.MaterialSolderLeadFree);
        Append("Solder Solvent Weld", inspection.MaterialSolderSolventWeld, request.MaterialSolderSolventWeld);
        Append("Solder Other", inspection.MaterialSolderOther, request.MaterialSolderOther);
        Append("Solder Other Description", inspection.MaterialSolderOtherDescription, request.MaterialSolderOtherDescription);

        Append("On-Site Sewage Facility", inspection.AiOssf, request.AiOssf);
        Append("Water Well", inspection.AiWaterWell, request.AiWaterWell);
        Append("Fire System", inspection.AiFireSystem, request.AiFireSystem);
        Append("Fire System On Separate Supply", inspection.AiFireSystem2, request.AiFireSystem2);
        Append("Grease Trap", inspection.AiGreaseTrap, request.AiGreaseTrap);
        Append("Sand & Grit Trap", inspection.AiSandGrit, request.AiSandGrit);
        Append("Reclaimed Water", inspection.AiReclaimedWater, request.AiReclaimedWater);
        Append("Irrigation System", inspection.AiIrrigationSystem, request.AiIrrigationSystem);
        Append("Irrigation System On Separate Supply", inspection.AiIrrigationSystem2, request.AiIrrigationSystem2);

        Append("Remarks", inspection.Comments, request.Comments);

        return changes.ToString();
    }

    private static string FormatChangeValue(object? value)
    {
        if (value == null)
        {
            return string.Empty;
        }

        if (value is bool flag)
        {
            return flag ? "Yes" : "No";
        }

        if (value is DateTime date)
        {
            return date.ToString("g");
        }

        return value.ToString() ?? string.Empty;
    }

    public async Task<IPagedData<CsiInspectionDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<CsiInspection, CsiInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<CsiInspection, CsiInspectionDto>(Mapper);
        var inspections = await _repository.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
        return inspections.Select(m => Mapper.Map<CsiInspectionDto>(m)!).ToPagedData(pageInfo);
    }

    public async Task<IPagedData<CsiInspectionDto>> SearchForAdminAsync(PageInfo pageInfo, Query query, CsiPaymentStatus? paymentStatus, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<CsiInspection, CsiInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<CsiInspection, CsiInspectionDto>(Mapper);

        var inspections = await _repository.SearchForAdminAsync(pageInfo, query, paymentStatus, cancellationToken);

        return inspections.Select(m => Mapper.Map<CsiInspectionDto>(m)!).ToPagedData(pageInfo);
    }

    private static void ApplySiteSnapshot(CsiInspection inspection, DataTransferObjects.Sites.SiteDto site)
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
        inspection.InspectorFaxNumber = professional.FaxNumber;
        inspection.InspectorLicenseNumber = csiLicense?.LicenseNumber;
        inspection.InspectorLicenseType = csiLicense?.LicenseType?.Name;
    }
}
