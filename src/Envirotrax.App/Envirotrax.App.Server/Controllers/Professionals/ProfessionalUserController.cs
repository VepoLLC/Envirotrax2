
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;


[Route("api/professionals/users")]
public class ProfessionalUserContoller : ProtectedController
{
    private readonly IProfessionalUserService _userService;

    public ProfessionalUserContoller(IProfessionalUserService userService)
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
        return Ok(updated);
    }
}