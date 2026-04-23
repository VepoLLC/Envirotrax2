using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using DeveloperPartners.SortingFiltering;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionService : Service<CsiInspection, CsiInspectionDto>, ICsiInspectionService
{
    private readonly ICsiInspectionRepository _repository;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly ISiteService _siteService;

    public CsiInspectionService(
        IMapper mapper,
        ICsiInspectionRepository repository,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        IProfessionalUserLicenseService licenseService,
        ISiteService siteService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _licenseService = licenseService;
        _siteService = siteService;
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

        var  csiLicense = licenses.Data.FirstOrDefault();

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
