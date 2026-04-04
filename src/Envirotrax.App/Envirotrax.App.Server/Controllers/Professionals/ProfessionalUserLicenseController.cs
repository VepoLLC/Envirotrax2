
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/licenses")]
[Authorize(Roles = RoleDefinitions.Professionals.Admin)]
public class ProfessionalUserLicenseController : ProfessionalCrudController<ProfessionalUserLicenseDto>
{
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IProfessionalLicenseTypeService _licenseTypeService;

    public ProfessionalUserLicenseController(IProfessionalUserLicenseService service, IProfessionalLicenseTypeService licenseTypeService)
        : base(service)
    {
        _licenseService = service;
        _licenseTypeService = licenseTypeService;
    }

    [HttpGet("types")]
    public async Task<IActionResult> GetAllTypesAsync([FromQuery] Query query, CancellationToken cancellationToken)
    {
        var types = await _licenseTypeService.GetAllAsync(query, cancellationToken);
        return Ok(types);
    }

    [HttpGet("/api/professionals/{userId}/licenses")]
    public async Task<IActionResult> GetAllAsync(int userId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query)
    {
        var licenses = await _licenseService.GetAllAsync(userId, pageInfo, query);
        return Ok(licenses);
    }
}
