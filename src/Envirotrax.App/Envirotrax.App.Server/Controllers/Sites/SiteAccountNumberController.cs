using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Sites;

[Route("api/sites/account-numbers")]
[PermissionResource(PermissionType.Sites)]
public class SiteAccountNumberController : WaterSupplierProtectedController
{
    private readonly ISiteService _siteService;

    public SiteAccountNumberController(ISiteService siteService)
    {
        _siteService = siteService;
    }

    [HttpPost]
    [HasPermission(PermissionAction.CanModify)]
    public async Task<IActionResult> GenerateAsync()
    {
        var accountNumber = await _siteService.GenerateAccountNumberAsync();

        return Ok(accountNumber);
    }
}
