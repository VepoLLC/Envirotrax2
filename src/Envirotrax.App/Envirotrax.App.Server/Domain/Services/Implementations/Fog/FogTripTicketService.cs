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
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;

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
    private readonly IUserService _userService;
    private readonly IPdfTemplateService _pdfTemplateService;

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
        IUserService userService,
        IPdfTemplateService pdfTemplateService)
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
        _userService = userService;
    }

    public override async Task<FogTripTicketDto?> DeleteAsync(int id)
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

    public Task<byte[]> GeneratePdfAsync(FogTripTicketDto ticket)
    {
        return GeneratePdfAsync([ticket]);
    }

    public Task<byte[]> GeneratePdfAsync(IEnumerable<FogTripTicketDto> tickets)
    {
        return _pdfTemplateService.GenerateAsync("Fog.FogTripTicket", tickets);
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

    public async Task<FogTripTicketDto?> UpdateApprovalAsync(int id, bool disapproved, CancellationToken cancellationToken)
    {
        string? approvedBy = null;

        if (!disapproved)
        {
            var user = await _userService.GetAsync(_authService.UserId, cancellationToken);
            approvedBy = user?.ContactName ?? user?.EmailAddress;
        }

        var ticket = await _repository.UpdateApprovalAsync(id, disapproved, approvedBy, cancellationToken);

        if (ticket == null)
        {
            return null;
        }

        var dto = Mapper.Map<FogTripTicketDto>(ticket);
        await PopulateSignatureUrlsAsync(dto);

        return dto;
    }

    public async Task<IPagedData<FogTripTicketDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, int? waterSupplierId, CancellationToken cancelationToken)
    {
        query.Filter = query.ConvertFilterProperties<FogTripTicket, FogTripTicketDto>(Mapper);
        query.Sort = query.ConvertSortProperties<FogTripTicket, FogTripTicketDto>(Mapper);

        var tickets = await _repository.SearchForProfessionalAsync(pageInfo, query, waterSupplierId, cancelationToken);

        return tickets.Select(Mapper.Map<FogTripTicketDto>).ToPagedData(pageInfo);
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

        var site = await _siteService.GetAsync(siteId, cancellationToken);
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken);
        var transporterUser = await _professionalUserService.GetAsync(transporterUserId, cancellationToken);

        var vehicle = request.VehicleId.HasValue
            ? await _vehicleService.GetAsync(request.VehicleId.Value, cancellationToken)
            : null;

        var disposalSite = request.ReceiverDisposalSiteId.HasValue
            ? await _disposalSiteService.GetAsync(request.ReceiverDisposalSiteId.Value, cancellationToken)
            : null;

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
            Completed = true,
            NeedsValidation = true
        };

        ApplySiteSnapshot(ticket, site);
        ApplyTransporterSnapshot(ticket, professional!, transporterUser, transporterUserId);
        ApplyVehicleSnapshot(ticket, vehicle);
        ApplyReceiverSnapshot(ticket, disposalSite);

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

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        var added = await _repository.AddAsync(ticket);

        if (generatorSignatureStream != null && ticket.GeneratorSignaturePath != null)
        {
            await _fileStorageService.UploadAsync(ticket.GeneratorSignaturePath, generatorSignatureStream);
        }
        if (receiverSignatureStream != null && ticket.ReceiverSignaturePath != null)
        {
            await _fileStorageService.UploadAsync(ticket.ReceiverSignaturePath, receiverSignatureStream);
        }

        scope.Complete();
        return Mapper.Map<FogTripTicketDto>(added);
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

    private static void ApplySiteSnapshot(FogTripTicket ticket, SiteDto? site)
    {
        if (site == null)
        {
            return;
        }

        ticket.PropertyBusinessName = site.BusinessName;
        ticket.PropertyType = site.PropertyType;
        ticket.PropertyStreetNumber = site.StreetNumber;
        ticket.PropertyStreetName = site.StreetName;
        ticket.PropertyNumber = site.PropertyNumber;
        ticket.PropertyCity = site.City;
        ticket.PropertyStateId = site.State?.Id;
        ticket.PropertyZip = site.ZipCode;
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
