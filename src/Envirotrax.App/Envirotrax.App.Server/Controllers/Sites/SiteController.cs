using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites")]
public class SiteController : CrudController<SiteDto>
{
    public SiteController(ISiteService service)
        : base(service)
    {
    }
}
