
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers;

[ApiController]
[Route("api/google-maps")]
public class GoogleMapsApiController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public GoogleMapsApiController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("api-key")]
    public IActionResult GetKey()
    {
        return Ok(new
        {
            apiKey = _configuration["GoogleMaps:PublicApiKey"]
        });
    }
}