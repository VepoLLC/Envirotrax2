
namespace Envirotrax.Common.Domain.DataTransferObjects;

public class RolePermissionDto
{
    public PermissionType Permission { get; set; }

    public bool CanView { get; set; }
    public bool CanCreate { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}