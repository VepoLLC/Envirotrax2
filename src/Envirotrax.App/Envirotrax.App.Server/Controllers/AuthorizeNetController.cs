
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers;

[ApiController]
[Route("api/authorize-net")]
public class AuthorizeNetController : EnvirotraxBaseController
{
    private readonly IConfiguration _configuration;

    public AuthorizeNetController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("client-config")]
    public IActionResult GetClientConfig()
    {
        return Ok(new
        {
            apiLoginId = _configuration["AuthorizeNet:ApiLoginId"],
            publicClientKey = _configuration["AuthorizeNet:PublicClientKey"]
        });
    }
}
