using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Csi;

[Route("api/csi/inspectors/{professionalId}/users")]
public class CsiInspectorUserController : AdminBaseController
{
    private readonly ICsiInspectorUserService _userService;

    public CsiInspectorUserController(ICsiInspectorUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalUserDto user, CancellationToken cancellationToken)
    {
        var added = await _userService.AddAsync(professionalId, user, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int userId, [FromBody] ProfessionalUserDto user, CancellationToken cancellationToken)
    {
        var updated = await _userService.UpdateAsync(professionalId, userId, user, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int userId, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(professionalId, userId, cancellationToken);

        return Ok();
    }
}
