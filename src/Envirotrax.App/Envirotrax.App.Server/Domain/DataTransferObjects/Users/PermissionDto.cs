
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Users;

public class ReferencedPermissionDto
{
    [Required]
    public PermissionType? Id { get; set; }

    public string? Name { get; set; }

    public string? Category { get; set; }

    public int SortOrder { get; set; }
}
