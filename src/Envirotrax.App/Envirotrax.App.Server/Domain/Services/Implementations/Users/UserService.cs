
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Domain.Configuration;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Users;

public class UserService : Service<WaterSupplierUser, WaterSupplierUserDto>, IUserService
{
    private readonly IAuthService _authService;
    private readonly IInternalApiClientService<AuthApiOptions> _authApiClient;
    private readonly IWaterSupplierService _waterSupplierService;

    public UserService(
        IMapper mapper,
        IUserRepository repository,
        IAuthService authService,
        IInternalApiClientService<AuthApiOptions> authApiClient,
        IWaterSupplierService waterSupplierService)
        : base(mapper, repository)
    {
        _authService = authService;
        _authApiClient = authApiClient;
        _waterSupplierService = waterSupplierService;
    }

    public override async Task<WaterSupplierUserDto> AddAsync(WaterSupplierUserDto dto)
    {
        var supplier = await _waterSupplierService.GetLoggedInSupplierAsync();

        var invitation = new UserInvitationDto
        {
            CreatorId = _authService.UserId,
            EmailAddress = dto.EmailAddress,
            InvitedByCompany = supplier.Name
        };

        var addedInvitation = await _authApiClient.PostAsync<UserInvitationDto, UserInvitationDto>("/api/users/invitations", new(_authService.WaterSupplierId, _authService.UserId)
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
}
class UserInvitationDto
{
    public int Id { get; set; }

    public int CreatorId { get; set; }

    public int UserId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string InvitedByCompany { get; set; } = null!;
}