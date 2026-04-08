
using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Professionals;

public class ProfessionalUserService : Service<ProfessionalUser, ProfessionalUserDto>, IProfessionalUserService
{
    private readonly IProfessionalUserRepository _professionalUserRepository;
    private readonly IAuthService _authService;
    private readonly IInternalApiClientService<AuthApiOptions> _authApiClient;
    private readonly IProfessionalService _professionalService;

    public ProfessionalUserService(
        IMapper mapper,
        IProfessionalUserRepository repository,
        IAuthService authService,
        IInternalApiClientService<AuthApiOptions> authApiClient,
        IProfessionalService professionalService)
        : base(mapper, repository)
    {
        _professionalUserRepository = repository;
        _authService = authService;
        _authApiClient = authApiClient;
        _professionalService = professionalService;
    }

    public Task<ProfessionalUserDto?> GetMyDataAsync(CancellationToken cancellationToken)
    {
        return GetAsync(_authService.UserId, cancellationToken);
    }

    public Task<ProfessionalUserDto> UpdateMyDataAsync(ProfessionalUserDto user)
    {
        user.Id = _authService.UserId;
        return UpdateAsync(user);
    }

    public override async Task<ProfessionalUserDto> AddAsync(ProfessionalUserDto dto)
    {
        var professional = await _professionalService.GetLoggedInProfessionalAsync() ?? throw new InvalidOperationException("User is not logged in to a registered professional.");

        var invitation = new UserInvitationDto
        {
            EmailAddress = dto.EmailAddress,
            InvitedByCompany = professional.Name
        };

        var addedInvitation = await _authApiClient.PostAsync<UserInvitationDto, UserInvitationDto>("/api/users/invitations", new(_authService.UserId)
        {
            Data = invitation
        });

        if (addedInvitation == null)
        {
            throw new InvalidOperationException("Adding user failed.");
        }

        dto.Id = addedInvitation.UserId;

        return await base.AddAsync(dto);
    }

    public override async Task<ProfessionalUserDto?> DeleteAsync(int id)
    {
        await _authApiClient.DeleteAsync<object>(_authService.UserId, $"/api/users/{id}/invitations");
        return await base.DeleteAsync(id);
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
        var user = await _professionalUserRepository.GetAsync(id, CancellationToken.None) ?? throw new InvalidOperationException();
        var professional = await _professionalService.GetLoggedInProfessionalAsync() ?? throw new InvalidOperationException("User is not logged in to a registered professional.");

        var invitation = new UserInvitationDto
        {
            EmailAddress = user.User!.Email!,
            InvitedByCompany = professional.Name
        };

        var addedInvitation = await _authApiClient.PostAsync<UserInvitationDto, UserInvitationDto>("/api/users/invitations", new(_authService.UserId)
        {
            Data = invitation
        });

        if (addedInvitation == null)
        {
            throw new InvalidOperationException("Resending invitation failed.");
        }

        return MapToDto(user);
    }
}

class UserInvitationDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string InvitedByCompany { get; set; } = null!;
}