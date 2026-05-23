
namespace Envirotrax.Common.Domain.DataTransferObjects;

public class RolePermissionDto
{
    public PermissionType Permission { get; set; }

    public bool CanView { get; set; }
    public bool CanModify { get; set; }
    public bool CanDelete { get; set; }
}