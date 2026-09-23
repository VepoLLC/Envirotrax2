using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/inspectors")]
    [HasFeature(FeatureType.FogInspection)]
    [PermissionResource(PermissionType.FogInspectors)]
    public class FogInspectorController : WaterSupplierCrudController<ProfessionalDto>
    {
        private readonly IFogInspectorService _inspectorService;

        public FogInspectorController(IFogInspectorService service)
            : base(service)
        {
            _inspectorService = service;
        }

        [HttpGet("search")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] string? inspectorLicenseNumber, [FromQuery] string? insurancePolicyNumber, CancellationToken cancellationToken)
        {
            var result = await _inspectorService.SearchAsync(inspectorLicenseNumber, insurancePolicyNumber, pageInfo, cancellationToken);
            return Ok(result);
        }
    }
}
