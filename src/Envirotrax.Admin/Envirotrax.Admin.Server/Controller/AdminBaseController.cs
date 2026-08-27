
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers;

[ApiController]
[Authorize(Roles = RoleDefinitions.SuperUser)]
public class AdminBaseController : ControllerBase
{

}