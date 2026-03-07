
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users;

[Route("api/users")]
[PermissionResource(PermissionType.Users)]
public class UserController : CrudController<WaterSupplierUserDto>
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public UserController(IUserService userService, IAuthService authService)
        : base(userService)
    {
        _userService = userService;
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

    [HttpPost("{id}/invitations")]
    [HasPermission(PermissionAction.CanEdit, PermissionType.Users)]
    public async Task<IActionResult> ResendInvitationAsync(int id)
    {
        var result = await _userService.ResendInvitationAsync(id);
        return Ok(result);
    }
}