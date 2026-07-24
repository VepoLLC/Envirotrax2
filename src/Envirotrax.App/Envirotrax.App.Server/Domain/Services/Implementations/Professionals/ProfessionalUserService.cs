
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalUserService : Service<ProfessionalUser, ProfessionalUserDto>, IProfessionalUserService
{
    private static readonly string[] AllowedFileExtensions = [".jpg", ".jpeg", ".gif", ".png", ".bmp", ".tiff"];

    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IAuthService _authService;
    private readonly IInternalApiClientService<AuthApiOptions> _authApiClient;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserLicenseRepository _licenseRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;
    private readonly IFileStorageService _fileStorageService;

    public ProfessionalUserService(
        IMapper mapper,
        IProfessionalUserRepository repository,
        IAuthService authService,
        IInternalApiClientService<AuthApiOptions> authApiClient,
        IProfessionalService professionalService,
        IProfessionalUserLicenseRepository licenseRepository,
        ITimeZoneHelperService timeZoneHelper,
        IFileStorageService fileStorageService)
        : base(mapper, repository)
    {
        _professionalUserRepository = repository;
        _authService = authService;
        _authApiClient = authApiClient;
        _professionalService = professionalService;
        _licenseRepository = licenseRepository;
        _timeZoneHelper = timeZoneHelper;
        _fileStorageService = fileStorageService;
    }

    public override async Task<IPagedData<ProfessionalUserDto>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var result = await base.GetAllAsync(pageInfo, query, cancellationToken);
        var users = result.Data.ToList();
        await EnrichWithBpatLicensesAsync(users, cancellationToken);
        return users.ToPagedData(pageInfo);
    }

    private async Task EnrichWithBpatLicensesAsync(IEnumerable<ProfessionalUserDto> users, CancellationToken cancellationToken)
    {
        var licenses = await _licenseRepository.GetBpatLicensesForProfessionalAsync(_authService.ProfessionalId, cancellationToken);
        var localTime = _timeZoneHelper.GetUserLocalTime();

        foreach (var user in users)
        {
            var license = licenses.FirstOrDefault(l => l.UserId == user.Id);
            if (license != null)
            {
                user.BpatLicenseNumber = license.LicenseNumber;
                user.BpatLicenseTypeName = license.LicenseType?.Name;
                user.BpatLicenseExpirationDate = license.ExpirationDate;
                user.BpatLicenseExpirationType = localTime > license.ExpirationDate
                    ? ExpirationType.Expired
                    : localTime.AddDays(30) >= license.ExpirationDate
                        ? ExpirationType.AboutToExpire
                        : ExpirationType.Valid;
            }
        }
    }

    public async Task<ProfessionalUserDto?> GetMyDataAsync(CancellationToken cancellationToken)
    {
        var dto = await GetAsync(_authService.UserId, cancellationToken);

        if (dto != null)
        {
            dto.SignatureUrl = await BuildSignatureUrlAsync(dto.SignaturePath);
        }

        return dto;
    }

    public async Task<string?> GetSignatureUrlAsync(int userId, CancellationToken cancellationToken)
    {
        var dto = await GetAsync(userId, cancellationToken);

        return await BuildSignatureUrlAsync(dto?.SignaturePath);
    }

    public async Task<string?> SaveMySignatureAsync(Stream signatureStream, string signatureFileName)
    {
        var userId = _authService.UserId;
        var professionalId = _authService.ProfessionalId;

        var extension = ValidateAndGetExtension(signatureFileName);
        var path = $"professionals/{professionalId}/users/{userId}/signature/{Guid.NewGuid()}{extension}";

        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _professionalUserRepository.UpdateSignaturePathAsync(userId, path);
        await _fileStorageService.UploadAsync(path, signatureStream);

        scope.Complete();

        return await BuildSignatureUrlAsync(path);
    }

    private async Task<string?> BuildSignatureUrlAsync(string? signaturePath)
    {
        if (string.IsNullOrWhiteSpace(signaturePath))
        {
            return null;
        }

        var url = await _fileStorageService.GenerateSasUrlAsync(signaturePath);

        return url.ToString();
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

    public async Task<ProfessionalUserDto?> UpdateMyDataAsync(ProfessionalUserDto user)
    {
        user.Id = _authService.UserId;

        var model = MapToModel(user);
        var updated = await _professionalUserRepository.UpdateNonSensitiveDataAsync(model!);

        return MapToDto(updated);
    }

    public override async Task<ProfessionalUserDto> AddAsync(ProfessionalUserDto dto)
    {
        var professional = await _professionalService.GetLoggedInProfessionalAsync()
            ?? throw new InvalidOperationException("User is not logged in to a registered professional.");

        dto.Id = await SendInvitationAsync(dto.EmailAddress, professional.Name, CancellationToken.None);

        return await base.AddAsync(dto);
    }

    public override async Task<ProfessionalUserDto?> DeleteAsync(int id)
    {
        await _authApiClient.DeleteAsync<object>(_authService.UserId, $"/api/users/{id}/invitations", CancellationToken.None);
        return await base.DeleteAsync(id);
    }

    public async Task<ProfessionalUserDto> AddForProfessionalAsync(int professionalId, ProfessionalUserDto dto, CancellationToken cancellationToken)
    {
        var professional = await _professionalService.GetAsync(professionalId, cancellationToken)
            ?? throw new InvalidOperationException("Professional not found.");

        dto.Id = await SendInvitationAsync(dto.EmailAddress, professional.Name, cancellationToken);

        var model = MapToModel(dto)!;
        model.ProfessionalId = professionalId;

        var added = await _professionalUserRepository.AddAsync(model);
        return MapToDto(added)!;
    }

    public async Task<ProfessionalUserDto?> UpdateSubAccountAsync(int professionalId, int userId, string? contactName, string? jobTitle)
    {
        var updated = await _professionalUserRepository.UpdateSubAccountAsync(professionalId, userId, contactName, jobTitle);
        return MapToDto(updated);
    }

    public async Task<IPagedData<ProfessionalUserDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken, Expression<Func<ProfessionalUser, bool>>? roleFilter = null)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUser, ProfessionalUserDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUser, ProfessionalUserDto>(Mapper);

        var items = await _professionalUserRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken, roleFilter);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }

    public async Task<ProfessionalUserDto?> ResendInvitationAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _professionalUserRepository.GetAsync(id, cancellationToken) ?? throw new InvalidOperationException();
        var professional = await _professionalService.GetLoggedInProfessionalAsync(cancellationToken) ?? throw new InvalidOperationException("User is not logged in to a registered professional.");

        await SendInvitationAsync(user.User!.Email!, professional.Name, cancellationToken);

        return MapToDto(user);
    }

    private async Task<int> SendInvitationAsync(string emailAddress, string companyName, CancellationToken cancellationToken)
    {
        var invitation = new UserInvitationDto
        {
            EmailAddress = emailAddress,
            InvitedByCompany = companyName
        };

        var addedInvitation = await _authApiClient.PostAsync<UserInvitationDto, UserInvitationDto>("/api/users/invitations", new(_authService.UserId)
        {
            Data = invitation
        }, cancellationToken);

        return addedInvitation?.UserId ?? throw new InvalidOperationException("Adding user failed.");
    }
}

class UserInvitationDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string InvitedByCompany { get; set; } = null!;
}