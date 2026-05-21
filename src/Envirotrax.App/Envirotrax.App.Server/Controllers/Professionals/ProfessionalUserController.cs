
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;


[Route("api/professionals/users")]
[Authorize(Roles = RoleDefinitions.Professionals.Admin)]
public class ProfessionalUserContoller : ProfessionalCrudController<ProfessionalUserDto>
{
    private readonly IProfessionalUserService _userService;

    public ProfessionalUserContoller(IProfessionalUserService userService)
        : base(userService)
    {
        _userService = userService;
    }

    [HttpPost("{id}/invitations")]
    public async Task<IActionResult> ResendInvitationAsync(int id)
    {
        var result = await _userService.ResendInvitationAsync(id);
        return Ok(result);
    }
}

[Authorize]
[ApiController]
[Route("api/professionals/users")]
public class MyProfessionalUserContoller : ControllerBase
{
    private readonly IProfessionalUserService _userService;

    public MyProfessionalUserContoller(IProfessionalUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyDataAsync(CancellationToken cancellationToken)
    {
        var user = await _userService.GetMyDataAsync(cancellationToken);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("my")]
    public async Task<IActionResult> UpdateMyDataAsync(ProfessionalUserDto user)
    {
        var updated = await _userService.UpdateMyDataAsync(user);

        if (updated == null)
        {
            return Conflict();
        }

        return Ok(updated);
    }
}
