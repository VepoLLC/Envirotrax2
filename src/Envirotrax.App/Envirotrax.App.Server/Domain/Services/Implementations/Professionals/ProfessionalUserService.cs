
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalUserService : Service<ProfessionalUser, ProfessionalUserDto>, IProfessionalUserService
{
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IAuthService _authService;
    private readonly IInternalApiClientService<AuthApiOptions> _authApiClient;
    private readonly IProfessionalService _professionalService;
    private readonly IProfessionalUserLicenseRepository _licenseRepository;
    private readonly ITimeZoneHelperService _timeZoneHelper;

    public ProfessionalUserService(
        IMapper mapper,
        IProfessionalUserRepository repository,
        IAuthService authService,
        IInternalApiClientService<AuthApiOptions> authApiClient,
        IProfessionalService professionalService,
        IProfessionalUserLicenseRepository licenseRepository,
        ITimeZoneHelperService timeZoneHelper)
        : base(mapper, repository)
    {
        _professionalUserRepository = repository;
        _authService = authService;
        _authApiClient = authApiClient;
        _professionalService = professionalService;
        _licenseRepository = licenseRepository;
        _timeZoneHelper = timeZoneHelper;
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

    public Task<ProfessionalUserDto?> GetMyDataAsync(CancellationToken cancellationToken)
    {
        return GetAsync(_authService.UserId, cancellationToken);
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

        dto.Id = await SendInvitationAsync(dto.EmailAddress, professional.Name);

        return await base.AddAsync(dto);
    }

    public override async Task<ProfessionalUserDto?> DeleteAsync(int id)
    {
        await _authApiClient.DeleteAsync<object>(_authService.UserId, $"/api/users/{id}/invitations");
        return await base.DeleteAsync(id);
    }

    public async Task<ProfessionalUserDto> AddForProfessionalAsync(int professionalId, ProfessionalUserDto dto)
    {
        var professional = await _professionalService.GetAsync(professionalId, CancellationToken.None)
            ?? throw new InvalidOperationException("Professional not found.");

        dto.Id = await SendInvitationAsync(dto.EmailAddress, professional.Name);

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

    public async Task<IPagedData<ProfessionalUserDto>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<ProfessionalUser, ProfessionalUserDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<ProfessionalUser, ProfessionalUserDto>(Mapper);

        var items = await _professionalUserRepository.GetAllByProfessionalAsync(professionalId, pageInfo, query, cancellationToken);

        return items.Select(i => MapToDto(i)!).ToPagedData(pageInfo);
    }

    public async Task<ProfessionalUserDto?> ResendInvitationAsync(int id)
    {
        var user = await _professionalUserRepository.GetAsync(id, CancellationToken.None)
            ?? throw new InvalidOperationException();
        var professional = await _professionalService.GetLoggedInProfessionalAsync()
            ?? throw new InvalidOperationException("User is not logged in to a registered professional.");

        await SendInvitationAsync(user.User!.Email!, professional.Name);

        return MapToDto(user);
    }

    private async Task<int> SendInvitationAsync(string emailAddress, string companyName)
    {
        var invitation = new UserInvitationDto
        {
            EmailAddress = emailAddress,
            InvitedByCompany = companyName
        };

        var addedInvitation = await _authApiClient.PostAsync<UserInvitationDto, UserInvitationDto>("/api/users/invitations", new(_authService.UserId)
        {
            Data = invitation
        });

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