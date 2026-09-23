
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/google-maps")]
public class GoogleMapsController : AdminBaseController
{
    private readonly IConfiguration _configuration;

    public GoogleMapsController(IConfiguration configuration)
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
