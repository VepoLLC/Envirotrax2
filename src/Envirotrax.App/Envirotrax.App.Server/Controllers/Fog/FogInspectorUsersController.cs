using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Fog
{
    [Route("api/fog/inspectors")]
    [HasFeature(FeatureType.FogInspection)]
    [PermissionResource(PermissionType.FogInspectors)]
    public class FogInspectorUsersController : WaterSupplierProtectedController
    {
        private readonly IProfessionalUserService _userService;

        public FogInspectorUsersController(IProfessionalUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}/users")]
        [HasPermission(PermissionAction.CanView)]
        public async Task<IActionResult> GetSubAccountsAsync(int id, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllByProfessionalAsync(id, pageInfo, query, cancellationToken, pu => pu.IsFogInspector);
            return Ok(result);
        }

        [HttpPut("{id}/users/{userId}")]
        [HasFeature(FeatureType.ManageProfessionalUsers)]
        [HasPermission(PermissionAction.CanModify)]
        public async Task<IActionResult> UpdateSubAccountAsync(int id, int userId, [FromBody] ProfessionalUserDto dto)
        {
            var result = await _userService.UpdateSubAccountAsync(id, userId, dto.ContactName, dto.JobTitle);
            return Ok(result);
        }
    }
}
