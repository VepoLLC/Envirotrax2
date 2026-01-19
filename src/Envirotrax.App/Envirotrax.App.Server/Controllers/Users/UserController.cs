
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users;

[Route("api/users")]
public class UserController : CrudController<WaterSupplierUserDto>
{
    public UserController(IUserService service)
        : base(service)
    {
    }
}