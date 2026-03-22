
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers;

[ApiController]
[Authorize(Roles = RoleDefinitions.Professional)]
public class ProfessionalProtectedController : ControllerBase
{

}