using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/registrations")]
public class RegistrationManagementController : WaterSupplierProtectedController
{
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IAuthService _authService;

    public RegistrationManagementController(IProfessionalUserLicenseService licenseService, IAuthService authService)
    {
        _licenseService = licenseService;
        _authService = authService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUnverifiedRegistrationsAsync(
        [FromQuery] PageInfo pageInfo,
        [FromQuery] Query query,
        CancellationToken cancellationToken)
    {
        if (!HasRegistrationAccess())
        {
            return Forbid();
        }

        var result = await _licenseService.GetUnverifiedRegistrationsByWaterSupplierAsync(pageInfo, query, cancellationToken);

        return Ok(result);
    }

    private bool HasRegistrationAccess()
    {
        return _authService.HasAnyFeatures(FeatureType.ManageProfessionalLicenses) ||
               _authService.HasAnyPermission(PermissionAction.CanView, PermissionType.Licenses);
    }
}
