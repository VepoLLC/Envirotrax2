using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites")]
[PermissionResource(PermissionType.Sites)]
public class SiteController : CrudController<SiteDto>
{
    public SiteController(ISiteService service)
        : base(service)
    {
    }
}
