using AutoMapper;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.AutoMapper;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Users;

public class UserService : Service<WaterSupplierUser, WaterSupplierUserDto>, IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuthService _authService;
    private readonly IInternalApiClientService<AuthApiOptions> _authApiClient;
    private readonly IWaterSupplierService _waterSupplierService;
    private readonly IRecordLogService _recordLogService;

    public UserService(
        IMapper mapper,
        IUserRepository repository,
        IUserRoleRepository userRoleRepository,
        IAuthService authService,
        IInternalApiClientService<AuthApiOptions> authApiClient,
        IWaterSupplierService waterSupplierService,
        IRecordLogService recordLogService)
        : base(mapper, repository)
    {
        _userRepository = repository;
        _userRoleRepository = userRoleRepository;
        _authService = authService;
        _authApiClient = authApiClient;
        _waterSupplierService = waterSupplierService;
        _recordLogService = recordLogService;
    }

    public override async Task<WaterSupplierUserDto> UpdateAsync(WaterSupplierUserDto dto)
    {
        var model = MapToModel(dto)!;
        var saved = await _userRepository.UpdateUserAsync(model);

        if (saved.Model == null)
        {
            throw new InvalidOperationException($"User {dto.Id} not found.");
        }

        if (saved.Changes.Length > 0)
        {
            await _recordLogService.AddAsync(RecordLogTableNames.WaterSupplierUserAccounts, saved.Model.UserId, saved.Model.WaterSupplierId, RecordLogType.Edit, saved.Changes);
        }

        return MapToDto(saved.Model)!;
    }

    public override async Task<WaterSupplierUserDto> AddAsync(WaterSupplierUserDto dto)
    {
        var supplier = await _waterSupplierService.GetLoggedInSupplierAsync();

        var invitation = new UserInvitationDto
        {
            EmailAddress = dto.EmailAddress,
            InvitedByCompany = supplier.Name
        };

        var addedInvitation = await _authApiClient.PostAsync<UserInvitationDto, UserInvitationDto>("/api/users/invitations", new(_authService.WaterSupplierId, _authService.UserId)
        {
            Data = invitation
        }, CancellationToken.None);

        if (addedInvitation == null)
        {
            throw new InvalidOperationException("Adding user failed.");
        }

        dto.Id = addedInvitation.UserId;

        return await base.AddAsync(dto);
    }

    public override async Task<WaterSupplierUserDto?> DeleteAsync(int id)
    {
        await _authApiClient.DeleteAsync<object>(_authService.WaterSupplierId, _authService.UserId, $"/api/users/{id}/invitations", CancellationToken.None);

        await _userRoleRepository.DeleteAllForUserAsync(id);

        var deleted = await base.DeleteAsync(id);

        if (deleted != null)
        {
            await _recordLogService.AddAsync(RecordLogTableNames.WaterSupplierUserAccounts, deleted.Id, _authService.WaterSupplierId, RecordLogType.Delete,
                $"Deleted user account — ContactName: '{deleted.ContactName}', EmailAddress: '{deleted.EmailAddress}'");
        }

        return deleted;
    }

    public async Task<WaterSupplierUserDto?> ResendInvitationAsync(int id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetAsync(id, cancellationToken) ?? throw new InvalidOperationException();
        var supplier = await _waterSupplierService.GetLoggedInSupplierAsync(cancellationToken);

        var invitation = new UserInvitationDto
        {
            EmailAddress = user.EmailAddress!,
            InvitedByCompany = supplier.Name
        };

        var addedInvitation = await _authApiClient.PostAsync<UserInvitationDto, UserInvitationDto>("/api/users/invitations", new(_authService.WaterSupplierId, _authService.UserId)
        {
            Data = invitation
        }, cancellationToken);

        if (addedInvitation == null)
        {
            throw new InvalidOperationException("Resending invitation failed.");
        }

        return MapToDto(user);
    }

    public async Task<IPagedData<WaterSupplierUserDto>> GetAllForWaterSupplierAsync(int waterSupplierId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        query.Sort = query.ConvertSortProperties<WaterSupplierUser, WaterSupplierUserDto>(Mapper);
        query.Filter = query.ConvertFilterProperties<WaterSupplierUser, WaterSupplierUserDto>(Mapper);

        var users = await _userRepository.GetAllForWaterSupplierAsync(waterSupplierId, pageInfo, query, cancellationToken);

        return users
            .Select(user => MapToDto(user)!)
            .ToPagedData(pageInfo);
    }
}
class UserInvitationDto
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string InvitedByCompany { get; set; } = null!;
}