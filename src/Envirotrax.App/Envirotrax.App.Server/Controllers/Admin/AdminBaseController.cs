
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Authorize]
[ApiController]
[HasScope(ScopeDefinitions.AdminInternal)]
public class AdminBaseController : ControllerBase
{

}