
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Auth.Controllers;

[ApiController]
[Route("api/users")]
public class UserInvitationController : ControllerBase
{
    private readonly IUserInvitationService _invitationService;

    public UserInvitationController(IUserInvitationService invitationService)
    {
        _invitationService = invitationService;
    }

    [HttpPost("invitations")]
    public async Task<IActionResult> AddAsync(UserInvitationDto invitation)
    {
        var created = await _invitationService.AddAsync(invitation);
        return Ok(created);
    }
}