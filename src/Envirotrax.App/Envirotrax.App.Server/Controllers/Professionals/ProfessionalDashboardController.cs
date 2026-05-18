
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals;

[Route("api/professionals/dashboard")]
public class ProfessionalDashboardController : ProfessionalProtectedController
{
    private readonly IProfessionalSupplierService _supplierService;
    private readonly IProfessionalUserService _userService;
    private readonly IProfessionalUserLicenseService _licenseService;
    private readonly IProfessionalInsuranceService _insuranceService;
    private readonly IBackflowGaugeService _gaugeService;

    public ProfessionalDashboardController(
        IProfessionalSupplierService supplierService,
        IProfessionalUserService userService,
        IProfessionalUserLicenseService licenseService,
        IProfessionalInsuranceService insuranceService,
        IBackflowGaugeService gaugeService)
    {
        _supplierService = supplierService;
        _userService = userService;
        _licenseService = licenseService;
        _insuranceService = insuranceService;
        _gaugeService = gaugeService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStatsAsync(CancellationToken cancellationToken)
    {
        var supplierResult  = await _supplierService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
        var userResult      = await _userService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
        var licenseResult   = await _licenseService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
        var insuranceResult = await _insuranceService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
        var gaugeResult     = await _gaugeService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);

        return Ok(new ProfessionalDashboardStatsDto
        {
            SupplierCount   = (int)(supplierResult.PageInfo?.TotalItems  ?? 0),
            SubAccountCount = (int)(userResult.PageInfo?.TotalItems      ?? 0),
            LicenseCount    = (int)(licenseResult.PageInfo?.TotalItems   ?? 0),
            InsuranceCount  = (int)(insuranceResult.PageInfo?.TotalItems ?? 0),
            GaugeCount      = (int)(gaugeResult.PageInfo?.TotalItems     ?? 0)
        });
    }
}
