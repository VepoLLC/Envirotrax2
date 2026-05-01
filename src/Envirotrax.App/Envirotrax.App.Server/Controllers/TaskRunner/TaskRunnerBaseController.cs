
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TaskRunner;

[Authorize]
[ApiController]
[HasScope(ScopeDefinitions.TaskRunner)]
public abstract class TaskRunnerBaseContoller : ControllerBase
{

}