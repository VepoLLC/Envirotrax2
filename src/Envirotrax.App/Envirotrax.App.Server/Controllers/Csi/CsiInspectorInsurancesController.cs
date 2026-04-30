using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [HasFeature(FeatureType.CsiInspection)]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorInsurancesController : WaterSupplierProtectedController
    {
        private readonly IProfessionalInsuranceService _insuranceService;

        public CsiInspectorInsurancesController(IProfessionalInsuranceService insuranceService)
        {
            _insuranceService = insuranceService;
        }

        [HttpGet("{id}/insurances")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetInsurancesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _insuranceService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id}/insurances")]
        [HasFeature(FeatureType.ManageProfessionalInsurances)]
        [HasPermission(PermissionAction.CanEdit)]
        public async Task<IActionResult> AddInsuranceAsync([FromForm] CreateInsuranceDto dto, CancellationToken cancellationToken)
        {
            using var stream = dto.File.OpenReadStream();
            return Ok(await _insuranceService.AddAsync(stream, dto.File.FileName, dto));
        }

        [HttpPut("{id}/insurances/{insuranceId}")]
        [HasFeature(FeatureType.ManageProfessionalInsurances)]
        [HasPermission(PermissionAction.CanEdit)]
        public async Task<IActionResult> UpdateInsuranceAsync(int insuranceId, [FromBody] ProfessionalInsuranceDto dto, CancellationToken cancellationToken)
        {
            dto.Id = insuranceId;
            return Ok(await _insuranceService.UpdateAsync(dto));
        }

        [HttpGet("{id}/insurances/{insuranceId}/file-url")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetInsuranceFileUrlAsync(int insuranceId, CancellationToken cancellationToken)
        {
            var url = await _insuranceService.GenerateFileUrlAsync(insuranceId, cancellationToken);

            if (url != null)
            {
                return Ok(url);
            }

            return NotFound();
        }

        [HttpDelete("{id}/insurances/{insuranceId}")]
        [HasFeature(FeatureType.ManageProfessionalInsurances)]
        [HasPermission(PermissionAction.CanEdit)]
        public async Task<IActionResult> DeleteInsuranceAsync(int insuranceId, CancellationToken cancellationToken)
        {
            await _insuranceService.DeleteAsync(insuranceId);
            return Ok();
        }
    }
}
