
using Envirotrax.Auth.Domain.DataTransferObjects;
using Envirotrax.Auth.Domain.Services.Definitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace Envirotrax.Auth.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
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

    [HttpDelete("{userId}/invitations")]
    public async Task<IActionResult> DeleteAllAsync(int userId)
    {
        await _invitationService.DeleteAllAsync(userId);
        return Ok();
    }
}