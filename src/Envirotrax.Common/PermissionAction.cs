
namespace Envirotrax.Common;

[Flags]
public enum PermissionAction
{
    None = 0,
    CanView = 1,
    CanCreate = 2,
    CanEdit = 4,
    CanDelete = 8
}