

using Envirotrax.Admin.Server.Domain.Services.Definitions.Lookup;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Lookup;

[Route("api/lookup")]
public class LookupController : AdminBaseController
{
    private readonly ILookupService _lookupService;

    public LookupController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    [HttpGet("states")]
    public async Task<IActionResult> GetStatesAsync(CancellationToken cancellationToken)
    {
        var states = await _lookupService.GetStatesAsync(cancellationToken);

        return Ok(states);
    }
}
