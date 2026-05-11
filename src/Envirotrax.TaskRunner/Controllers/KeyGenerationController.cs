
using Envirotrax.TaskRunner.Domain.Services.Definitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.TaskRunner.Controllers;

[AllowAnonymous]
[ApiController]
[Route("api/keys")]
public class KeyGenerationController : ControllerBase
{
    private readonly IKeyHashingService _keyService;
    private readonly IHostEnvironment _environment;

    public KeyGenerationController(IKeyHashingService keyService, IHostEnvironment environment)
    {
        _keyService = keyService;
        _environment = environment;
    }

    [HttpPost]
    public IActionResult GenerateKey()
    {
        if (_environment.IsDevelopment())
        {
            var key = _keyService.GenerateApiKey();

            return Ok(new
            {
                PlainTextKey = key,
                HashedKey = _keyService.HashText(key)
            });
        }

        return Unauthorized();
    }
}