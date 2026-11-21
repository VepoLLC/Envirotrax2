using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Backflow;

[Route("api/backflow/testers/{professionalId}/users")]
public class BackflowTesterUserController : AdminBaseController
{
    private readonly IBackflowTesterUserService _userService;
    private readonly IBackflowTesterGaugeService _gaugeService;

    public BackflowTesterUserController(IBackflowTesterUserService userService, IBackflowTesterGaugeService gaugeService)
    {
        _userService = userService;
        _gaugeService = gaugeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int professionalId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllAsync(professionalId, pageInfo, query, cancellationToken);

        return Ok(users);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetAsync(int professionalId, int userId, CancellationToken cancellationToken)
    {
        var user = await _userService.GetAsync(professionalId, userId, cancellationToken);

        return user == null ? NotFound() : Ok(user);
    }

    [HttpGet("{userId}/gauges")]
    public async Task<IActionResult> GetGaugesAsync(int professionalId, int userId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var gauges = await _gaugeService.GetAllAsync(professionalId, pageInfo, query, cancellationToken, userId);

        return Ok(gauges);
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

    [HttpPut("{userId}/payment-bypass")]
    public async Task<IActionResult> UpdatePaymentBypassAsync(int professionalId, int userId, [FromBody] ProfessionalPaymentBypassDto paymentBypass, CancellationToken cancellationToken)
    {
        var updated = await _userService.UpdatePaymentBypassAsync(professionalId, userId, paymentBypass, cancellationToken);

        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{userId}")]
    public async Task<IActionResult> DeleteAsync(int professionalId, int userId, CancellationToken cancellationToken)
    {
        await _userService.DeleteAsync(professionalId, userId, cancellationToken);

        return Ok();
    }
}
