using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Users
{
    [Route("api/users/{userId}/roles")]
    public class UserRoleController : ProtectedController
    {
        private readonly IUserRoleService _userRoleService;

        public UserRoleController(IUserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync(int userId)
        {
            var userRoles = await _userRoleService.GetAllAsync(userId);
            return Ok(userRoles);
        }

        [HttpPost]
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