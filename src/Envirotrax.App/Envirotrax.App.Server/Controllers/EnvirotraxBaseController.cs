
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers;

[ApiController]
[HasScope(ScopeDefinitions.EnvirotraxApp)]
public class EnvirotraxBaseController : ControllerBase
{

}