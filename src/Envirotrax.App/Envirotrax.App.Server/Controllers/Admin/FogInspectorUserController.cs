using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/inspectors/{professionalId}/users")]
public class FogInspectorUserController : AdminBaseController
{
    private readonly IProfessionalUserService _userService;

    public FogInspectorUserController(IProfessionalUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllByProfessionalAsync(
            professionalId,
            pageInfo,
            query,
            cancellationToken,
            proUser => proUser.IsFogInspector);

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalUserDto user, CancellationToken cancellationToken)
    {
        user.IsFogInspector = true;

        var added = await _userService.AddForProfessionalAsync(professionalId, user, cancellationToken);

        return Ok(added);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateAsync(int professionalId, int userId, [FromBody] ProfessionalUserDto user)
    {
        var updated = await _userService.UpdateSubAccountAsync(professionalId, userId, user.ContactName, user.JobTitle);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int userId, CancellationToken cancellationToken)
    {
        if (!await BelongsToProfessionalAsync(professionalId, userId, cancellationToken))
        {
            return NotFound();
        }

        var deleted = await _userService.DeleteAsync(userId);

        return deleted == null ? NotFound() : Ok();
    }

    private async Task<bool> BelongsToProfessionalAsync(int professionalId, int userId, CancellationToken cancellationToken)
    {
        var owned = await _userService.GetAllByProfessionalAsync(
            professionalId,
            new PageInfo(),
            new Query(),
            cancellationToken,
            proUser => proUser.UserId == userId);

        return owned.Data.Any();
    }
}
