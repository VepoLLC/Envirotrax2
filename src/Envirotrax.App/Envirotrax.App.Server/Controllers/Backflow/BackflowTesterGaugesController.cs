using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow
{
    [Route("api/backflow/testers")]
    [HasFeature(FeatureType.BackflowTesting)]
    [PermissionResource(PermissionType.BackflowTesters)]
    public class BackflowTesterGaugesController : WaterSupplierProtectedController
    {
        private readonly IBackflowGaugeService _gaugeService;

        public BackflowTesterGaugesController(IBackflowGaugeService gaugeService)
        {
            _gaugeService = gaugeService;
        }

        [HttpGet("{id}/gauges/{gaugeId}/file-url")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetGaugeFileUrlAsync(int gaugeId, CancellationToken cancellationToken)
        {
            var url = await _gaugeService.GenerateFileUrlAsync(gaugeId, cancellationToken);

            if (url != null)
            {
                return Ok(url);
            }

            return NotFound();
        }

        [HttpGet("{id}/gauges")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetGaugesAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _gaugeService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id}/gauges")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> AddGaugeAsync([FromForm] CreateBackflowGaugeDto dto)
        {
            using var stream = dto.File.OpenReadStream();
            return Ok(await _gaugeService.AddWithFileAsync(stream, dto.File.FileName, dto));
        }

        [HttpPut("{id}/gauges/{gaugeId}")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> UpdateGaugeAsync(int gaugeId, [FromBody] BackflowGaugeDto dto)
        {
            dto.Id = gaugeId;
            return Ok(await _gaugeService.UpdateAsync(dto));
        }

        [HttpDelete("{id}/gauges/{gaugeId}")]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> DeleteGaugeAsync(int gaugeId)
        {
            await _gaugeService.DeleteAsync(gaugeId);
            return Ok();
        }
    }
}
