
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users;

[Route("api/users")]
[PermissionResource(PermissionType.Users)]
public class UserController : WaterSupplierCrudController<WaterSupplierUserDto>
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UserController(IUserService userService, IAuthService authService)
        : base(userService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("{id}/invitations")]
    [HasPermission(PermissionAction.CanEdit, PermissionType.Users)]
    public async Task<IActionResult> ResendInvitationAsync(int id)
    {
        var result = await _userService.ResendInvitationAsync(id);
        return Ok(result);
    }
}

// This controller just requires the user to be logged in. They don't have to be a water supplier or professional to call it
[Authorize]
[ApiController]
[Route("api/users")]
public class MyAccessController : ControllerBase
{
    private readonly IAuthService _authService;
    public MyAccessController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("access/my")]
    public IActionResult GetMyAccessInfoAsync()
    {
        var accessDto = new UserAccessDto
        {
            Features = _authService.GetAllMyFeatures(),
            Permissions = _authService.GetAllMyPermissions()
        };

        return Ok(accessDto);
    }
}