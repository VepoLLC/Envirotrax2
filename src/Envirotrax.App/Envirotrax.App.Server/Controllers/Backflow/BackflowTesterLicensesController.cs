using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow
{
    [Route("api/backflow/testers")]
    [HasFeature(FeatureType.BackflowTesting)]
    [PermissionResource(PermissionType.BackflowTesters)]
    public class BackflowTesterLicensesController : WaterSupplierProtectedController
    {
        private readonly IProfessionalUserLicenseService _licenseService;
        private readonly IProfessionalLicenseTypeService _licenseTypeService;

        public BackflowTesterLicensesController(IProfessionalUserLicenseService licenseService, IProfessionalLicenseTypeService licenseTypeService)
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
            query.Filter.Add(new() { ColumnName = nameof(ProfessionalUserLicenseDto.ProfessionalType), Value = ((int)ProfessionalType.Bpat).ToString() });
            var result = await _licenseService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id}/licenses")]
        [HasFeature(FeatureType.ManageProfessionalLicenses)]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> AddLicenseAsync(int id, [FromBody] ProfessionalUserLicenseDto dto)
        {
            var result = await _licenseService.AddForProfessionalAsync(id, dto);
            return Ok(result);
        }

        [HttpPut("{id}/licenses/{licenseId}")]
        [HasFeature(FeatureType.ManageProfessionalLicenses)]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> UpdateLicenseAsync(int id, int licenseId, [FromBody] ProfessionalUserLicenseDto dto)
        {
            dto.Id = licenseId;
            var result = await _licenseService.UpdateForProfessionalAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}/licenses/{licenseId}")]
        [HasFeature(FeatureType.ManageProfessionalLicenses)]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> DeleteLicenseAsync(int licenseId)
        {
            await _licenseService.DeleteAsync(licenseId);
            return Ok();
        }
    }
}
