
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class UserRoleDto
{
    [Required]
    public ReferencedAppUserDto User { get; set; } = null!;

    [Required]
    public ReferencedRoleDto Role { get; set; } = null!;
}
