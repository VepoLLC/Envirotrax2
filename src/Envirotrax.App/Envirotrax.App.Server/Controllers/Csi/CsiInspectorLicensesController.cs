using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [HasFeature(FeatureType.CsiInspection)]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorLicensesController : WaterSupplierProtectedController
    {
        private readonly IProfessionalUserLicenseService _licenseService;
        private readonly IProfessionalLicenseTypeService _licenseTypeService;

        public CsiInspectorLicensesController(IProfessionalUserLicenseService licenseService, IProfessionalLicenseTypeService licenseTypeService)
        {
            _licenseService = licenseService;
            _licenseTypeService = licenseTypeService;
        }

        [HttpGet("licenses/types")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetLicenseTypesAsync([FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _licenseTypeService.GetAllAsync(query, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}/licenses")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetLicensesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _licenseService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id}/licenses")]
        [HasFeature(FeatureType.ManageProfessionalLicenses)]
        [HasPermission(PermissionAction.CanEdit)]
        public async Task<IActionResult> AddLicenseAsync(int id, [FromBody] ProfessionalUserLicenseDto dto, CancellationToken cancellationToken)
        {
            var result = await _licenseService.AddForProfessionalAsync(id, dto);
            return Ok(result);
        }

        [HttpPut("{id}/licenses/{licenseId}")]
        [HasFeature(FeatureType.ManageProfessionalLicenses)]
        [HasPermission(PermissionAction.CanEdit)]
        public async Task<IActionResult> UpdateLicenseAsync(int id, int licenseId, [FromBody] ProfessionalUserLicenseDto dto, CancellationToken cancellationToken)
        {
            dto.Id = licenseId;
            var result = await _licenseService.UpdateForProfessionalAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}/licenses/{licenseId}")]
        [HasFeature(FeatureType.ManageProfessionalLicenses)]
        [HasPermission(PermissionAction.CanEdit)]
        public async Task<IActionResult> DeleteLicenseAsync(int id, int licenseId, CancellationToken cancellationToken)
        {
            await _licenseService.DeleteAsync(licenseId);
            return Ok();
        }
    }
}
