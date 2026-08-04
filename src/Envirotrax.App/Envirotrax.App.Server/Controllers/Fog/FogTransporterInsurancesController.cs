using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/transporters")]
    [HasFeature(FeatureType.FogTransportation)]
    [PermissionResource(PermissionType.FogTransporters)]
    public class FogTransporterInsurancesController : WaterSupplierProtectedController
    {
        private readonly IProfessionalInsuranceService _insuranceService;

        public FogTransporterInsurancesController(IProfessionalInsuranceService insuranceService)
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
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> AddInsuranceAsync([FromForm] CreateInsuranceDto dto)
        {
            using var stream = dto.File.OpenReadStream();

            return Ok(await _insuranceService.AddAsync(stream, dto.File.FileName, dto));
        }

        [HttpPut("{id}/insurances/{insuranceId}")]
        [HasFeature(FeatureType.ManageProfessionalInsurances)]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> UpdateInsuranceAsync(int insuranceId, [FromBody] ProfessionalInsuranceDto dto)
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
    }
}
