using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorController : WaterSupplierCrudController<ProfessionalDto>
    {
        private readonly ICsiInspectorService _inspectorService;
        private readonly IProfessionalSupplierService _supplierService;
        private readonly IProfessionalUserService _userService;
        private readonly IProfessionalUserLicenseService _licenseService;
        private readonly IProfessionalInsuranceService _insuranceService;

        public CsiInspectorController(
            ICsiInspectorService service,
            IProfessionalSupplierService supplierService,
            IProfessionalUserService userService,
            IProfessionalUserLicenseService licenseService,
            IProfessionalInsuranceService insuranceService)
            : base(service)
        {
            _inspectorService = service;
            _supplierService = supplierService;
            _userService = userService;
            _licenseService = licenseService;
            _insuranceService = insuranceService;
        }

        [HttpGet("{id}/account-info")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetAccountInfoAsync(int id, CancellationToken cancellationToken)
        {
            var accountInfo = await _inspectorService.GetAccountInfoAsync(id, cancellationToken);

            if (accountInfo != null)
            {
                return Ok(accountInfo);
            }

            return NotFound();
        }

        [HttpGet("{id}/water-suppliers")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetWaterSuppliersAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _supplierService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/sub-accounts")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetSubAccountsAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/licenses")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetLicensesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _licenseService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/insurances")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetInsurancesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _insuranceService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }
    }
}
