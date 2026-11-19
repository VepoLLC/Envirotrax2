using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Crm;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Crm;

/// <summary>
/// The users a company's Last Contacted By can name - the users of the water supplier that owns the CRM records.
/// </summary>
[Route("api/crm/users")]
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
