using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogInspectionService : Service<FogInspection, FogInspectionDto>, IFogInspectionService
{
    private readonly IFogInspectionRepository _repository;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly ISiteService _siteService;

    public FogInspectionService(
        IMapper mapper,
        IFogInspectionRepository repository,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        ISiteService siteService)
        : base(mapper, repository)
    {
        _repository = repository;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _siteService = siteService;
    }

    public async Task<FogInspectionDto> SubmitAsync(FogInspectionDto request, CancellationToken cancellationToken)
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

        var added = await _repository.AddAsync(inspection);
        return Mapper.Map<FogInspectionDto>(added);
    }

    public async Task<IPagedData<FogInspectionDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, bool latestOnly, CancellationToken cancellationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogInspection, FogInspectionDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogInspection, FogInspectionDto>(Mapper);

        var inspections = await _repository.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);

        return inspections.Select(m => Mapper.Map<FogInspectionDto>(m)!).ToPagedData(pageInfo);
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
