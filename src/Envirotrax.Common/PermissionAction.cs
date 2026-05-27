
namespace Envirotrax.Common;

[Flags]
public enum PermissionAction
{
    None = 0,
    CanView = 1,
    CanModify = 4,
    CanDelete = 8
}