using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users
{
    [Route("api/users/{userId}/roles")]
    public class UserRoleController : WaterSupplierProtectedController
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        [HasPermission(PermissionAction.CanView, PermissionType.Users)]
        public async Task<IActionResult> GetAllAsync(int userId)
        {
            var userRoles = await _userRoleService.GetAllAsync(userId);
            return Ok(userRoles);
        }

        [HttpPost]
        [HasPermission(PermissionAction.CanCreate, PermissionType.Users)]
        public async Task<IActionResult> AddAsync(int userId, UserRoleDto userRole)
        {
            if (userId != userRole?.User?.Id)
            {
                return BadRequest();
            }

            var added = await _userRoleService.AddAsync(userRole);
            return Ok(added);
        }

        [HttpDelete("{roleId}")]
        [HasPermission(PermissionAction.CanDelete, PermissionType.Users)]
        public async Task<IActionResult> DeleteAsync(int userId, int roleId)
        {
            var deleted = await _userRoleService.DeleteAsync(userId, roleId);

            if (deleted != null)
            {
                return Ok(deleted);
            }

            return NotFound();
        }
    }
}