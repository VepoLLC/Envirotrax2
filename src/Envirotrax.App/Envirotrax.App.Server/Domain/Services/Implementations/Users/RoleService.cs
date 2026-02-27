
using AutoMapper;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Users
{
    public class RoleService : Service<Role, RoleDto>, IRoleService
    {
        public RoleService(IMapper mapper, IRoleRepository repository)
            : base(mapper, repository)
        {
        }
    }
}