using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/fog/transporters/{professionalId}/users")]
public class FogTransporterUserController : AdminBaseController
{
    private readonly IProfessionalUserService _userService;

    public FogTransporterUserController(IProfessionalUserService userService)
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
            proUser => proUser.IsFogTransporter);

        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(int professionalId, [FromBody] ProfessionalUserDto user, CancellationToken cancellationToken)
    {
        user.IsFogTransporter = true;

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
        var deleted = await _userService.DeleteForProfessionalAsync(professionalId, userId, cancellationToken);

        return deleted == null ? NotFound() : Ok();
    }
}
