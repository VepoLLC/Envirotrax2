using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/company-licenses")]
[Authorize(Roles = RoleDefinitions.Professionals.Admin)]
public class ProfessionalLicenseController : ProfessionalCrudController<ProfessionalLicenseDto>
{
    private readonly IProfessionalDashboardService _dashboardService;

    public ProfessionalLicenseController(IProfessionalLicenseService service, IProfessionalDashboardService dashboardService)
        : base(service)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("/api/professionals/company-licenses-and-insurances")]
    public async Task<IActionResult> GetAllWithInsurancesAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var rows = await _dashboardService.GetCompanyLicensesAndInsurancesAsync(pageInfo, query, cancellationToken);

        return Ok(rows);
    }
}
