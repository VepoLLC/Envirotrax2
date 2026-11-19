using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin.Crm;

/// <summary>
/// The users a CRM company's Last Contacted By can name: the users of the Vepo LLC water supplier, which owns the
/// Envirotrax.Admin CRM records. The admin client doesn't know that supplier's id, so it can't use the water
/// supplier users endpoint.
/// </summary>
[Route("api/admin/crm/users")]
public class CrmUserController : AdminBaseController
{
    private readonly ICompanyService _companyService;

    public CrmUserController(ICompanyService companyService)
    {
        _companyService = companyService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var users = await _companyService.GetCrmUsersAsync(pageInfo, query, cancellationToken);

        return Ok(users);
    }
}
