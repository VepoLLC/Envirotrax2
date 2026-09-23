
using Envirotrax.App.Server.Domain.Services.Definitions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/lookup")]
public class LookupController : AdminBaseController
{
    private readonly ILookupService _lookupService;

    public LookupController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet("states")]
    public async Task<IActionResult> GetStates()
    {
        var states = await _lookupService.GetStatesAsync();
        return Ok(states);
    }
}
