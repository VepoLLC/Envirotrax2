
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
            dto.SupplierCount   = await _supplierService.CountAsync(cancellationToken);
            dto.SubAccountCount = await _userService.CountAsync(cancellationToken);
            dto.LicenseCount    = await _licenseService.CountAsync(cancellationToken);
            dto.InsuranceCount  = await _insuranceService.CountAsync(cancellationToken);
        }

        if (canAccessGauges)
        {
            dto.GaugeCount = await _gaugeService.CountAsync(cancellationToken);
        }

        if (canAccessTransportation)
        {
            dto.VehicleCount      = await _vehicleService.CountAsync(cancellationToken);
            dto.DisposalSiteCount = await _disposalSiteService.CountRegisteredDisposalSitesAsync(cancellationToken);
        }

        return Ok(dto);
    }
}
