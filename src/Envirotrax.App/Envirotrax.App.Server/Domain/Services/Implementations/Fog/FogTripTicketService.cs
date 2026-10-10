using System.ComponentModel.DataAnnotations;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions;

using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.Common.Data;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Implementations.Sites;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Fog;

public class FogTripTicketService : Service<FogTripTicket, FogTripTicketDto>, IFogTripTicketService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff"];

    private readonly IFogTripTicketRepository _repository;
    private readonly IAuthService _authService;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserService _professionalUserService;
    private readonly ISiteService _siteService;
    private readonly IFogVehicleService _vehicleService;
    private readonly IFogDisposalSiteService _disposalSiteService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPdfTemplateService _pdfTemplateService;
    private readonly IGeneralSettingsService _generalSettingsService;
    private readonly IProfessionalSupplierService _professionalSupplierService;
    private readonly IProfessionalInsuranceService _insuranceService;
    private readonly IMailingInfoRedactionService _mailingInfoRedactionService;

    public FogTripTicketService(
        IMapper mapper,
        IFogTripTicketRepository repository,
        IAuthService authService,
        IProfessionalService professionalService,
        IProfessionalUserService professionalUserService,
        ISiteService siteService,
        IFogVehicleService vehicleService,
        IFogDisposalSiteService disposalSiteService,
        IFileStorageService fileStorageService,
        IPdfTemplateService pdfTemplateService,
        IGeneralSettingsService generalSettingsService,
        IProfessionalSupplierService professionalSupplierService,
        IProfessionalInsuranceService insuranceService,
        IMailingInfoRedactionService mailingInfoRedactionService)
        : base(mapper, repository)
    {
        _repository = repository;
        _authService = authService;
        _professionalService = professionalService;
        _professionalUserService = professionalUserService;
        _siteService = siteService;
        _vehicleService = vehicleService;
        _disposalSiteService = disposalSiteService;
        _fileStorageService = fileStorageService;
        _pdfTemplateService = pdfTemplateService;
        _generalSettingsService = generalSettingsService;
        _professionalSupplierService = professionalSupplierService;
        _insuranceService = insuranceService;
        _mailingInfoRedactionService = mailingInfoRedactionService;
    }

    public override async Task<FogTripTicketDto?> DeleteAsync(int id)
    {
        FogTripTicket? deleted;

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

    public Task<byte[]> GeneratePdfAsync(FogTripTicketDto ticket)
    {
        return GeneratePdfAsync([ticket]);
    }

    public Task<byte[]> GeneratePdfAsync(IEnumerable<FogTripTicketDto> tickets)
    {
        return _pdfTemplateService.GenerateAsync("Fog.FogTripTicket", tickets);
    }

    public async Task<byte[]> GeneratePdfWithSignaturesAsync(List<FogTripTicketDto> tickets)
    {
        foreach (var ticket in tickets)
        {
            await PopulateSignatureUrlsAsync(ticket);
        }

        return await GeneratePdfAsync(tickets);
    }

    public override async Task<FogTripTicketDto?> GetAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await base.GetAsync(id, cancellationToken);

        if (dto != null)
        {
            await PopulateSignatureUrlsAsync(dto);
        }

        return dto;
    }

    public async Task<FogTripTicketDto?> GetForProfessionalAsync(int id, CancellationToken cancellationToken)
    {
        var dto = await GetAsync(id, cancellationToken);

        if (dto != null)
        {
            await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
        }

        return dto;
    }

    public async Task<FogTripTicketDto?> UpdateApprovalAsync(int id, bool disapproved, CancellationToken cancellationToken)
    {
        var ticket = await _repository.UpdateApprovalAsync(id, disapproved, _authService.UserId, cancellationToken);

        if (ticket == null)
        {
            return null;
        }

        var dto = Mapper.Map<FogTripTicketDto>(ticket);
        await PopulateSignatureUrlsAsync(dto);

        return dto;
    }

    public Task<byte[]> GeneratePdfForProfessionalAsync(FogTripTicketDto ticket)
    {
        if (ticket.TransactionId == null)
        {
            throw new AppValidationException("Report can't be downloaded until it's paid. Please go to checkout and pay for this transaction, then try downloading again.");
        }

        return GeneratePdfAsync(ticket);
    }

    public async Task<IPagedData<FogTripTicketDto>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken cancelationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogTripTicket, FogTripTicketDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogTripTicket, FogTripTicketDto>(Mapper);

        var tickets = await _repository.SearchForProfessionalAsync(pageInfo, query, waterSupplierId, cancelationToken);
        var dtos = tickets.Select(Mapper.Map<FogTripTicketDto>).ToPagedData(pageInfo);

        return await _mailingInfoRedactionService.RedactAsync(dtos, cancelationToken);
    }

    public Task<InsuranceCheckDto> GetInsuranceCheckAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return _insuranceService.CheckForWaterSupplierAsync(_authService.ProfessionalId, waterSupplierId, ProfessionalType.FogTransporter, cancellationToken);
    }

    public async Task<FogTripTicketDto> SubmitAsync(
        FogTripTicketDto request,
        Stream? generatorSignatureStream, string? generatorSignatureFileName,
        Stream? receiverSignatureStream, string? receiverSignatureFileName,
        CancellationToken cancellationToken)
    {
        var siteId = request.Site!.Id!.Value;
        var waterSupplierId = request.WaterSupplier!.Id!.Value;
        var transporterUserId = request.Transporter!.Id!.Value;

        await _insuranceService.EnsureSatisfiedForWaterSupplierAsync(_authService.ProfessionalId, waterSupplierId, ProfessionalType.FogTransporter, cancellationToken);

        var site = await _siteService.GetAsync(siteId, cancellationToken);
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var transporterUser = await _professionalUserService.GetAsync(transporterUserId, cancellationToken);

        var vehicle = request.VehicleId.HasValue
            ? await _vehicleService.GetAsync(request.VehicleId.Value, cancellationToken)
            : null;

        var disposalSite = request.ReceiverDisposalSiteId.HasValue
            ? await _disposalSiteService.GetAsync(request.ReceiverDisposalSiteId.Value, cancellationToken)
            : null;

        if (await _mailingInfoRedactionService.KeepSiteLocationWhenRedactedAsync(request, site, waterSupplierId, cancellationToken))
        {
            // A trip ticket also checks the generator's contact details against the site's.
            request.FogGeneratorPhoneNumber = site!.FogGeneratorPhoneNumber;
            request.FogGeneratorEmailAddress = site.FogGeneratorEmailAddress;
        }

        var ticket = new FogTripTicket
        {
            WaterSupplierId = waterSupplierId,
            SiteId = siteId,

            FogGeneratorContactName = request.FogGeneratorContactName,
            FogGeneratorPhoneNumber = request.FogGeneratorPhoneNumber,
            FogGeneratorEmailAddress = request.FogGeneratorEmailAddress,
            GeneratorContactName = request.GeneratorContactName,

            TransporterLicenseNumber = request.TransporterLicenseNumber,
            TransporterLicenseExpiration = request.TransporterLicenseExpiration,

            InterceptorType = request.InterceptorType,
            InterceptorOtherDescription = request.InterceptorOtherDescription,
            InterceptorCapacity = request.InterceptorCapacity,
            InterceptorCapacityType = request.InterceptorCapacityType,
            InterceptorWasteRemovedAmount = request.InterceptorWasteRemovedAmount,
            InterceptorWasteRemovedType = request.InterceptorWasteRemovedType,
            InterceptorWasteRemovedDate = request.InterceptorWasteRemovedDate,

            ReceiverContactName = request.ReceiverContactName,
            ReceiverWasteDeliveredDate = request.ReceiverWasteDeliveredDate,

            Comments = request.Comments,

            PickupCompleted = true,
            Completed = true
        };

        ApplyEnteredLocation(ticket, request);
        ApplySiteValidation(ticket, site);
        ApplyTransporterSnapshot(ticket, professional!, transporterUser, transporterUserId);
        ApplyVehicleSnapshot(ticket, vehicle);
        ApplyReceiverSnapshot(ticket, disposalSite);
        await ApplyAmountAsync(ticket, site?.IsFeeExempt ?? false, cancellationToken);

        if (generatorSignatureStream != null && generatorSignatureFileName != null)
        {
            ticket.GeneratorSignaturePath = $"professionals/{professional!.Id}/fog-trip-tickets/generator/{Guid.NewGuid()}{ValidateAndGetExtension(generatorSignatureFileName)}";
            ticket.GeneratorSignatureDate = DateTime.UtcNow;
        }
        if (receiverSignatureStream != null && receiverSignatureFileName != null)
        {
            ticket.ReceiverSignaturePath = $"professionals/{professional!.Id}/fog-trip-tickets/receiver/{Guid.NewGuid()}{ValidateAndGetExtension(receiverSignatureFileName)}";
            ticket.ReceiverSignatureDate = DateTime.UtcNow;
        }

        FogTripTicket added;

        using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            added = await _repository.AddAsync(ticket);

            if (generatorSignatureStream != null && ticket.GeneratorSignaturePath != null)
            {
                await _fileStorageService.UploadAsync(ticket.GeneratorSignaturePath, generatorSignatureStream);
            }
            if (receiverSignatureStream != null && ticket.ReceiverSignaturePath != null)
            {
                await _fileStorageService.UploadAsync(ticket.ReceiverSignaturePath, receiverSignatureStream);
            }

            scope.Complete();
        }

        var dto = Mapper.Map<FogTripTicketDto>(added);

        return await _mailingInfoRedactionService.RedactAsync(dto, cancellationToken);
    }

    private async Task ApplyAmountAsync(FogTripTicket ticket, bool siteIsFeeExempt, CancellationToken cancellationToken)
    {
        ticket.Amount = 0;
        ticket.AmountShare = 0;

        if (siteIsFeeExempt)
        {
            return;
        }

        var settings = await _generalSettingsService.GetAsync(ticket.WaterSupplierId, cancellationToken);
        var registration = await _professionalSupplierService.GetAsync(ticket.WaterSupplierId, cancellationToken);

        ticket.Amount = registration?.FogTransportFee ?? settings?.FogTransportFee ?? 0;
        ticket.AmountShare = settings?.FogTransportFeeWsShare ?? 0;
    }

    private async Task PopulateSignatureUrlsAsync(FogTripTicketDto dto)
    {
        var signatures = new (string? Path, Action<string> SetUrl)[]
        {
            (dto.GeneratorSignaturePath, url => dto.GeneratorSignatureUrl = url),
            (dto.ReceiverSignaturePath, url => dto.ReceiverSignatureUrl = url),
            (dto.TransporterSignaturePath, url => dto.TransporterSignatureUrl = url)
        };

        if (!signatures.Any(s => !string.IsNullOrWhiteSpace(s.Path)))
        {
            return;
        }

        var delegationKey = await _fileStorageService.GetUserDelegationKeyAsync();

        foreach (var (path, setUrl) in signatures)
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

    private static void ApplyEnteredLocation(FogTripTicket ticket, FogTripTicketDto request)
    {
        ticket.PropertyBusinessName = request.PropertyBusinessName;
        ticket.PropertyType = request.PropertyType;
        ticket.PropertyStreetNumber = request.PropertyStreetNumber;
        ticket.PropertyStreetName = request.PropertyStreetName;
        ticket.PropertyNumber = request.PropertyNumber;
        ticket.PropertyCity = request.PropertyCity;
        ticket.PropertyStateId = SiteInformationComparer.GetStateId(request.PropertyState);
        ticket.PropertyZip = request.PropertyZip;
    }

    private static void ApplySiteValidation(FogTripTicket ticket, SiteDto? site)
    {
        ticket.ValidationNewSite = false;
        ticket.ValidationSiteInformationChanged = false;
        ticket.NeedsValidation = false;

        if (site == null)
        {
            ticket.ValidationNewSite = true;
            ticket.NeedsValidation = true;

            return;
        }

        ticket.ValidationSiteInformationChanged = HasSiteInformationChanged(ticket, site);
        ticket.NeedsValidation = ticket.ValidationSiteInformationChanged;
    }

    private static bool HasSiteInformationChanged(FogTripTicket ticket, SiteDto site)
    {
        if (ticket.PropertyType != site.PropertyType)
        {
            return true;
        }

        if (ticket.PropertyStateId != SiteInformationComparer.GetStateId(site.State))
        {
            return true;
        }

        var textFields = new List<(string? EnteredValue, string? SiteValue)>
        {
            (ticket.PropertyBusinessName, site.BusinessName),
            (ticket.PropertyStreetNumber, site.StreetNumber),
            (ticket.PropertyStreetName, site.StreetName),
            (ticket.PropertyNumber, site.PropertyNumber),
            (ticket.PropertyCity, site.City),
            (ticket.PropertyZip, site.ZipCode),
            (ticket.FogGeneratorPhoneNumber, site.FogGeneratorPhoneNumber),
            (ticket.FogGeneratorEmailAddress, site.FogGeneratorEmailAddress)
        };

        return SiteInformationComparer.HasTextChanged(textFields);
    }

    private static void ApplyTransporterSnapshot(
        FogTripTicket ticket,
        ProfessionalDto professional,
        ProfessionalUserDto? transporterUser,
        int transporterUserId)
    {
        ticket.ProfessionalId = professional.Id;
        ticket.TransporterId = transporterUserId;
        ticket.TransporterCompanyName = professional.Name;
        ticket.TransporterContactName = transporterUser?.ContactName;
        ticket.TransporterAddress = professional.Address;
        ticket.TransporterCity = professional.City;
        ticket.TransporterState = professional.State?.Name;
        ticket.TransporterZip = professional.ZipCode;
        ticket.TransporterWorkNumber = professional.PhoneNumber;
        ticket.TransporterCellNumber = transporterUser?.PhoneNumber;
        ticket.TransporterFaxNumber = professional.FaxNumber;
        ticket.TransporterEmailAddress = transporterUser?.EmailAddress ?? professional.CompanyEmail;

        ticket.TransporterSignaturePath = transporterUser?.SignaturePath;
        ticket.TransporterSignatureDate = transporterUser?.SignaturePath != null ? DateTime.UtcNow : null;
    }

    private static void ApplyVehicleSnapshot(FogTripTicket ticket, FogVehicleDto? vehicle)
    {
        if (vehicle == null)
        {
            return;
        }

        ticket.VehicleId = vehicle.Id;
        ticket.VehicleLicensePlateNumber = vehicle.LicensePlateNumber;
        ticket.VehicleManufacturer = vehicle.Manufacturer;
        ticket.VehicleYear = vehicle.ManufacturedYear;
        ticket.VehicleCapacity = vehicle.Capacity;
        ticket.VehicleCapacityType = vehicle.CapacityType;
        ticket.VehicleStickerNumber = vehicle.StickerNumber;
    }

    private static void ApplyReceiverSnapshot(FogTripTicket ticket, FogDisposalSiteDto? disposalSite)
    {
        if (disposalSite == null)
        {
            return;
        }

        ticket.ReceiverDisposalSiteId = disposalSite.Id;
        ticket.ReceiverCompanyName = disposalSite.Name;
        ticket.ReceiverAddress = disposalSite.Address;
        ticket.ReceiverCity = disposalSite.City;
        ticket.ReceiverState = disposalSite.State?.Name;
        ticket.ReceiverZip = disposalSite.ZipCode;
        ticket.ReceiverPhoneNumber = disposalSite.PhoneNumber;
        ticket.ReceiverEmailAddress = disposalSite.EmailAddress;
        ticket.ReceiverRegistrationNumber = disposalSite.RegistrationNumber;
        ticket.ReceiverPermitNumber = disposalSite.PermitNumber;
    }
}
