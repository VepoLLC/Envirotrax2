using Envirotrax.App.Server.Domain.Services.Definitions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
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
}
