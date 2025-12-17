
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers
{
    [Authorize]
    [ApiController]
    public class ProtectedController : ControllerBase
    {

    }
}