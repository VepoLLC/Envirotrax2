
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;


[Route("api/professionals/users")]
public class ProfessionalUserContoller : ProfessionalCrudController<ProfessionalUserDto>
{
    private readonly IProfessionalUserService _userService;

    public ProfessionalUserContoller(IProfessionalUserService userService)
        : base(userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public override Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public override Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        return base.GetAsync(id, cancellationToken);
    }

    [HttpPost]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public override Task<IActionResult> AddAsync(ProfessionalUserDto dto)
    {
        return base.AddAsync(dto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public override Task<IActionResult> UpdateAsync(int id, ProfessionalUserDto dto)
    {
        return base.UpdateAsync(id, dto);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public override Task<IActionResult> DeleteAsync(int id)
    {
        return base.DeleteAsync(id);
    }

    [HttpDelete("{id}/reactivate")]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public override Task<IActionResult> ReactivateAsync(int id)
    {
        return base.ReactivateAsync(id);
    }

    [HttpPost("{id}/invitations")]
    [Authorize(Roles = RoleDefinitions.Professionals.Admin)]
    public async Task<IActionResult> ResendInvitationAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _userService.ResendInvitationAsync(id, cancellationToken);
        return Ok(result);
    }
}

[Authorize]
[ApiController]
[Route("api/professionals/users")]
public class MyProfessionalUserContoller : EnvirotraxBaseController
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
