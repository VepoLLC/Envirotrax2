
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Licenses;

[Route("api/professionals/licenses")]
[Authorize(Roles = RoleDefinitions.Professionals.Admin)]
public class ProfessionalUserLicenseController : ProfessionalCrudController<ProfessionalUserLicenseDto>
{
    private readonly IProfessionalUserLicenseService _licenseService;

    public ProfessionalUserLicenseController(IProfessionalUserLicenseService service)
        : base(service)
    {
        _licenseService = service;
    }

    [HttpGet("/api/professionals/{userId}/licenses")]
    public async Task<IActionResult> GetAllAsync(int userId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query)
    {
        var licenses = await _licenseService.GetAllAsync(userId, pageInfo, query);
        return Ok(licenses);
    }
}
