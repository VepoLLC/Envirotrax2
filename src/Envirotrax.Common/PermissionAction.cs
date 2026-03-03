
namespace Envirotrax.Common;

[Flags]
public enum PermissionAction
{
    None = 0,
    CamView = 1,
    CanCreate = 2,
    CanEdit = 4,
    CanDelete = 8
}