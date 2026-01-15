
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Users;

public class UserService : Service<WaterSupplierUser, WaterSupplierUserDto>, IUserService
{
    public UserService(IMapper mapper, IUserRepository repository)
        : base(mapper, repository)
    {
    }

    public override Task<WaterSupplierUserDto> AddAsync(WaterSupplierUserDto dto)
    {
        return base.AddAsync(dto);
    }
}