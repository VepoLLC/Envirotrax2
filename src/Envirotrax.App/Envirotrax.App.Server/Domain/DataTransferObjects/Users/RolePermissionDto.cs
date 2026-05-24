
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users
{
    public class RolePermissionDto
    {
        [Required]
        public ReferencedRoleDto Role { get; set; } = null!;

        [Required]
        public ReferencedPermissionDto Permission { get; set; } = null!;

        public bool CanView { get; set; }
        public bool CanModify { get; set; }
        public bool CanDelete { get; set; }
    }
}