using Envirotrax.Common;

namespace Envirotrax.App.Server.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
    public class HasPermissionAttribute : Attribute
    {
        public PermissionAction AllowedAction { get; }
        public PermissionType[] AllowedPermissions { get; set; } = [];

        public HasPermissionAttribute(PermissionAction allowedAction)
        {
            AllowedAction = allowedAction;
        }

        public HasPermissionAttribute(PermissionAction allowedAction, PermissionType allowedPermission)
        {
            AllowedAction = allowedAction;
            AllowedPermissions = [allowedPermission];
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PermissionResourceAttribute : Attribute
    {
        public PermissionType[] AllowedPermissions { get; private set; }

        public PermissionResourceAttribute(params PermissionType[] allowedPermissions)
        {
            AllowedPermissions = allowedPermissions;
        }
    }
}