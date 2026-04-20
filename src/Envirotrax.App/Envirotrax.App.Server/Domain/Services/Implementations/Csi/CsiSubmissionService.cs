using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiSubmissionService : ICsiSubmissionService
{
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IProfessionalSupplierService _supplierService;
    private readonly ISiteService _siteService;
    private readonly ICsiInspectionRepository _repository;
    private readonly IAuthService _authService;
    private readonly ITenantProvidersService _tenantProvider;
    private readonly IMapper _mapper;

    public CsiSubmissionService(
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        IProfessionalUserLicenseService licenseService,
        IProfessionalSupplierService supplierService,
        ISiteService siteService,
        ICsiInspectionRepository repository,
        IAuthService authService,
        ITenantProvidersService tenantProvider,
        IMapper mapper)
    {
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _licenseService = licenseService;
        _supplierService = supplierService;
        _siteService = siteService;
        _repository = repository;
        _authService = authService;
        _tenantProvider = tenantProvider;
        _mapper = mapper;
    }

    public async Task<CsiSubmissionCreateViewModel?> GetCreateViewModelAsync(int siteId, CancellationToken cancellationToken)
    {
        var site = await _siteService.GetAsync(siteId, cancellationToken);
        
        if (site == null)
        {
            return null;
        }
        
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var professionalUser = await _professionalUserService.GetMyDataAsync(cancellationToken);
        var csiLicense = await GetCsiLicenseAsync(cancellationToken);

        var waterSuppliers = new List<CsiWaterSupplierOptionDto>();
        var csiAccounts = new List<CsiAccountOptionDto>();

        if (_authService.ProfessionalId > 0)
        {
            waterSuppliers = await LoadWaterSupplierOptionsAsync(_authService.ProfessionalId, cancellationToken);
            csiAccounts = await LoadCsiAccountOptionsAsync(_authService.ProfessionalId, cancellationToken);
        }

        return new CsiSubmissionCreateViewModel
        {
            SiteId = siteId,
            Site = MapSiteData(site),
            Inspector = MapInspectorData(professional, professionalUser),
            CsiLicense = MapLicenseData(csiLicense),
            AvailableWaterSuppliers = waterSuppliers,
            DefaultWaterSupplierId = site.WaterSupplier?.Id,
            AvailableCsiAccounts = csiAccounts,
            DefaultCsiAccountUserId = _authService.UserId
        };
    }

    private async Task<List<CsiWaterSupplierOptionDto>> LoadWaterSupplierOptionsAsync(int professionalId, CancellationToken cancellationToken)
    {
        var suppliers = await _supplierService.GetCsiSuppliersForProfessionalAsync(professionalId, cancellationToken);
        return suppliers
            .Where(s => s.WaterSupplier?.Id != null)
            .Select(s => new CsiWaterSupplierOptionDto
            {
                Id = s.WaterSupplier!.Id!.Value,
                Name = s.WaterSupplier.Name ?? string.Empty,
                PwsId = s.WaterSupplier.PwsId,
                Address = s.WaterSupplier.Address,
                City = s.WaterSupplier.City,
                State = s.WaterSupplier.State?.Name,
                ZipCode = s.WaterSupplier.ZipCode,
                PhoneNumber = s.WaterSupplier.PhoneNumber,
                ContactName = s.WaterSupplier.ContactName,
                EmailAddress = s.WaterSupplier.EmailAddress
            })
            .ToList();
    }

    private async Task<List<CsiAccountOptionDto>> LoadCsiAccountOptionsAsync(int professionalId, CancellationToken cancellationToken)
    {
        var inspectors = await _professionalUserService.GetCsiInspectorsForProfessionalAsync(professionalId, cancellationToken);
        var licenses = await _licenseService.GetCsiLicensesForProfessionalAsync(professionalId, cancellationToken);
        var licenseByUserId = licenses.ToDictionary(l => l.User.Id!.Value);

        return inspectors.Select(u => new CsiAccountOptionDto
        {
            UserId = u.Id,
            ContactName = u.ContactName,
            JobTitle = u.JobTitle,
            EmailAddress = u.EmailAddress,
            CsiLicense = licenseByUserId.TryGetValue(u.Id, out var lic) ? MapLicenseData(lic) : null
        }).ToList();
    }

    public async Task<CsiInspectionDto> SubmitAsync(CsiSubmissionSaveRequest request, CancellationToken cancellationToken)
    {
        var site = await _siteService.GetAsync(request.SiteId, cancellationToken);
        
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
       
        var inspectorUserId = request.SelectedCsiAccountUserId!.Value;
        var professionalUser = await _professionalUserService.GetAsync(inspectorUserId, cancellationToken);
        var csiLicense = await _licenseService.GetCsiLicenseForUserAsync(inspectorUserId, cancellationToken);

        var inspection = new CsiInspection
        {
            WaterSupplierId = request.WaterSupplierId,
            SiteId = request.SiteId,
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
        ApplyInspectorSnapshot(inspection, professional, professionalUser, csiLicense, inspectorUserId);

        var added = await _repository.AddAsync(inspection);
        return _mapper.Map<CsiInspectionDto>(added);
    }

    private Task<ProfessionalUserLicenseDto?> GetCsiLicenseAsync(CancellationToken cancellationToken)
    {
        return _licenseService.GetCsiLicenseForUserAsync(_authService.UserId, cancellationToken);
    }

    private static CsiSiteDataDto MapSiteData(DataTransferObjects.Sites.SiteDto site) => new()
    {
        Id = site.Id,
        AccountNumber = site.AccountNumber,
        BusinessName = site.BusinessName,
        PropertyType = site.PropertyType,
        StreetNumber = site.StreetNumber,
        StreetName = site.StreetName,
        PropertyNumber = site.PropertyNumber,
        City = site.City,
        State = site.State,
        ZipCode = site.ZipCode,
        MailingCompanyName = site.MailingCompanyName,
        MailingContactName = site.MailingContactName,
        MailingStreetNumber = site.MailingStreetNumber,
        MailingStreetName = site.MailingStreetName,
        MailingNumber = site.MailingNumber,
        MailingCity = site.MailingCity,
        MailingState = site.MailingState,
        MailingZipCode = site.MailingZipCode,
        MailingPhoneNumber = site.MailingPhoneNumber,
        MailingEmailAddress = site.MailingEmailAddress
    };

    private static CsiInspectorDataDto MapInspectorData(ProfessionalDto? professional, ProfessionalUserDto? professionalUser)
    {
        return new CsiInspectorDataDto
        {
            CompanyName = professional?.Name,
            ContactName = professionalUser?.ContactName,
            JobTitle = professionalUser?.JobTitle,
            Address = professional?.Address,
            City = professional?.City,
            State = professional?.State?.Name,
            ZipCode = professional?.ZipCode,
            PhoneNumber = professional?.PhoneNumber,
            FaxNumber = professional?.FaxNumber,
            EmailAddress = professionalUser?.EmailAddress
        };
    }
       
    private static CsiLicenseDataDto? MapLicenseData(ProfessionalUserLicenseDto? license)
    {
        if (license == null)
        {
            return null;
        }

        var isValid = license.ExpirationDate == null || license.ExpirationDate > DateTime.UtcNow;
        
        return new CsiLicenseDataDto
        {
            LicenseNumber = license.LicenseNumber,
            LicenseTypeName = license.LicenseType?.Name,
            ExpirationDate = license.ExpirationDate,
            IsValid = isValid
        };
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
        ProfessionalUserDto? professionalUser,
        ProfessionalUserLicenseDto? csiLicense,
        int inspectorUserId)
    {
        inspection.ProfessionalId = professional.Id;
        inspection.UserId = inspectorUserId;
        inspection.InspectorCompanyName = professional.Name;
        inspection.InspectorContactName = professionalUser?.ContactName;
        inspection.InspectorJobTitle = professionalUser?.JobTitle;
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
