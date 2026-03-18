using Envirotrax.App.Server.Controllers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users
{
    [Route("api/users/roles")]
    public class RolePermissionController : ProtectedController
    {
        private readonly IRolePermissionService _rolePermissionService;

        public RolePermissionController(IRolePermissionService rolePermissionService)
        {
            _rolePermissionService = rolePermissionService;
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissionsAsync()
        {
            var permissions = await _rolePermissionService.GetAllPermissionsAsync();
            return Ok(permissions);
        }

        [HttpGet("{roleId}/permissions")]
        [HasPermission(PermissionAction.CanView, PermissionType.Roles)]
        public async Task<IActionResult> GetAllAsync(int roleId)
        {
            var rolePermissions = await _rolePermissionService.GetAllAsync(roleId);
            return Ok(rolePermissions);
        }

        [HttpPut("{roleId}/permissions/add-or-update")]
        [HasPermission(PermissionAction.CanCreate | PermissionAction.CanEdit, PermissionType.Roles)]
        public async Task<IActionResult> AddOrUpdateAsync(int roleId, RolePermissionDto rolePermission)
        {
            if (roleId != rolePermission?.Role?.Id)
            {
                return BadRequest();
            }

            var updated = await _rolePermissionService.AddOrUpdateAsync(rolePermission);

            return Ok(updated);
        }

        [HttpPut("{roleId}/permissions/bulk-update")]
        [HasPermission(PermissionAction.CanCreate | PermissionAction.CanEdit, PermissionType.Roles)]
        public async Task<IActionResult> BulkUpdateAsync(int roleId, IEnumerable<RolePermissionDto> rolePermissions)
        {
            if (!rolePermissions.All(r => r.Role?.Id == roleId))
            {
                return BadRequest();
            }

            var updated = await _rolePermissionService.BulkUpdateAsync(rolePermissions);

            return Ok(updated);
        }
    }
}