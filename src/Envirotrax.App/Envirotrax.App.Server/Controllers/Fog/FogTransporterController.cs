using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/transporters")]
    [HasFeature(FeatureType.FogTransportation)]
    [PermissionResource(PermissionType.FogTransporters)]
    public class FogTransporterController : WaterSupplierCrudController<ProfessionalDto>
    {
        private readonly IFogTransporterService _transporterService;

        public FogTransporterController(IFogTransporterService service)
            : base(service)
        {
            _transporterService = service;
        }

        [HttpGet("search")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] string? registrationNumber, [FromQuery] string? insurancePolicyNumber, CancellationToken cancellationToken)
        {
            var result = await _transporterService.SearchAsync(registrationNumber, insurancePolicyNumber, pageInfo, cancellationToken);
            return Ok(result);
        }

        protected override async Task<IPagedData<ProfessionalDto>> ProcessGetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
        {
            return await _transporterService.GetAllWithDetailsAsync(pageInfo, query, cancellationToken);
        }
    }
}
