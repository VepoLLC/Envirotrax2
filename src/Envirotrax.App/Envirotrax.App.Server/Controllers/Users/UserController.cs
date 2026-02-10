
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users;

[Route("api/users")]
public class UserController : CrudController<WaterSupplierUserDto>
{
    private readonly IAuthService _authService;

    public UserController(IUserService service, IAuthService authService)
        : base(service)
    {
        _authService = authService;
    }

    [HttpGet("access/my")]
    public IActionResult GetMyAccessInfoAsync()
    {
        var accessDto = new UserAccessDto
        {
            Features = _authService.GetAllMyFeatures()
        };

        return Ok(accessDto);
    }
}