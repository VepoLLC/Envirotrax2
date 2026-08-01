
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.Common;
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
    private readonly IFogVehicleService _vehicleService;
    private readonly IFogTransporterDisposalSiteService _disposalSiteService;

    public ProfessionalDashboardController(
        IProfessionalSupplierService supplierService,
        IProfessionalUserService userService,
        IProfessionalUserLicenseService licenseService,
        IProfessionalInsuranceService insuranceService,
        IBackflowGaugeService gaugeService,
        IFogVehicleService vehicleService,
        IFogTransporterDisposalSiteService disposalSiteService)
    {
        _supplierService = supplierService;
        _userService = userService;
        _licenseService = licenseService;
        _insuranceService = insuranceService;
        _gaugeService = gaugeService;
        _vehicleService = vehicleService;
        _disposalSiteService = disposalSiteService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStatsAsync(CancellationToken cancellationToken)
    {
        var isAdmin        = User.IsInRole(RoleDefinitions.Professionals.Admin);
        var canAccessGauges = isAdmin || User.IsInRole(RoleDefinitions.Professionals.BackflowTester);
        var canAccessTransportation = isAdmin || User.IsInRole(RoleDefinitions.Professionals.FogTransporter);

        var dto = new ProfessionalDashboardStatsDto();

        if (isAdmin)
        {
            var supplierResult  = await _supplierService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
            var userResult      = await _userService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
            var licenseResult   = await _licenseService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
            var insuranceResult = await _insuranceService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);

            dto.SupplierCount   = (int)(supplierResult.PageInfo?.TotalItems  ?? 0);
            dto.SubAccountCount = (int)(userResult.PageInfo?.TotalItems      ?? 0);
            dto.LicenseCount    = (int)(licenseResult.PageInfo?.TotalItems   ?? 0);
            dto.InsuranceCount  = (int)(insuranceResult.PageInfo?.TotalItems ?? 0);
        }

        if (canAccessGauges)
        {
            var gaugeResult  = await _gaugeService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
            dto.GaugeCount   = (int)(gaugeResult.PageInfo?.TotalItems ?? 0);
        }

        if (canAccessTransportation)
        {
            var vehicleResult     = await _vehicleService.GetAllAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);
            var disposalSiteResult = await _disposalSiteService.GetRegisteredDisposalSitesAsync(new PageInfo { PageSize = 1 }, new Query(), cancellationToken);

            dto.VehicleCount      = (int)(vehicleResult.PageInfo?.TotalItems      ?? 0);
            dto.DisposalSiteCount = (int)(disposalSiteResult.PageInfo?.TotalItems ?? 0);
        }

        return Ok(dto);
    }
}
