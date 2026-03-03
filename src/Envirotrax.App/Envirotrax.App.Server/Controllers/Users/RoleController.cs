using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users
{
    [Route("api/users/roles")]
    [PermissionResource(PermissionType.Roles)]
    public class RoleContoller : CrudController<RoleDto>
    {
        public RoleContoller(IRoleService service)
            : base(service)
        {
        }
    }
}