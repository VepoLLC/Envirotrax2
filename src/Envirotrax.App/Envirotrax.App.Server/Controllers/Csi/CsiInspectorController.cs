using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Csi
{
    [Route("api/csi/inspectors")]
    [HasFeature(FeatureType.CsiInspection)]
    [PermissionResource(PermissionType.CsiInspectors)]
    public class CsiInspectorController : WaterSupplierCrudController<ProfessionalDto>
    {
        private readonly ICsiInspectorService _inspectorService;

        public CsiInspectorController(ICsiInspectorService service)
            : base(service)
        {
            _inspectorService = service;
        }

        [HttpGet("search")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] string? inspectorLicenseNumber, [FromQuery] string? insurancePolicyNumber, [FromQuery] string? userEmail, [FromQuery] string? contactName, CancellationToken cancellationToken)
        {
            var result = await _inspectorService.SearchAsync(inspectorLicenseNumber, insurancePolicyNumber, userEmail, contactName, pageInfo, query, cancellationToken);
            return Ok(result);
        }
    }
}

