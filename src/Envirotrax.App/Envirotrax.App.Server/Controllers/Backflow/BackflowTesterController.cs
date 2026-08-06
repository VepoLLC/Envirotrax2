using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Backflow
{
    [Route("api/backflow/testers")]
    [HasFeature(FeatureType.BackflowTesting)]
    [PermissionResource(PermissionType.BackflowTesters)]
    public class BackflowTesterController : WaterSupplierCrudController<ProfessionalDto>
    {
        private readonly IBackflowTesterService _testerService;

        public BackflowTesterController(IBackflowTesterService service)
            : base(service)
        {
            _testerService = service;
        }

        [HttpGet("search")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> SearchAsync([FromQuery] PageInfo pageInfo, [FromQuery] string? bpatLicenseNumber, [FromQuery] string? fireLicenseNumber, [FromQuery] string? insurancePolicyNumber, CancellationToken cancellationToken)
        {
            var result = await _testerService.SearchAsync(bpatLicenseNumber, fireLicenseNumber, insurancePolicyNumber, pageInfo, cancellationToken);
            return Ok(result);
        }
    }
}
