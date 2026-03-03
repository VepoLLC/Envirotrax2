
using System.Reflection;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Envirotrax.App.Server.Filters
{
    public class CheckPermissionFilter : IAuthorizationFilter
    {
        private IEnumerable<HasPermissionAttribute> GetPermissionAttributes(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
            {
                var permissionAttributes = controllerActionDescriptor
                    .ControllerTypeInfo
                    .GetCustomAttributes<HasPermissionAttribute>()
                    .Concat(controllerActionDescriptor.MethodInfo.GetCustomAttributes<HasPermissionAttribute>());

                var permissionResource = controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<PermissionResourceAttribute>();

                if (permissionResource != null)
                {
                    foreach (var permissionAtribute in permissionAttributes)
                    {
                        if (permissionAtribute.AllowedPermissions.IsNullOrEmpty())
                        {
                            permissionAtribute.AllowedPermissions = permissionResource.AllowedPermissions;
                        }
                    }
                }

                return permissionAttributes;
            }

            return [];
        }

        private bool HasAnyPermissions(AuthorizationFilterContext context, IEnumerable<HasPermissionAttribute> hasPermissionAttributes)
        {
            // If the user is not logged in, we don't check for permission. Use AuthorizeAttribute to require logged in user.
            if (!(context.HttpContext.User?.Identity?.IsAuthenticated).GetValueOrDefault())
            {
                return true;
            }

            // If the endpoint requires no permissions, let the user call it.
            if (hasPermissionAttributes.IsNullOrEmpty())
            {
                return true;
            }

            var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();

            foreach (var permissionAttribute in hasPermissionAttributes)
            {
                if (authService.HasAnyPermission(permissionAttribute.AllowedAction, permissionAttribute.AllowedPermissions))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanCheckPermissions(AuthorizationFilterContext context)
        {
            // If the user is not logged in, we don't check for permission. Use AuthorizeAttribute to require logged in user.
            if (context.HttpContext.User?.Identity?.IsAuthenticated == true)
            {
                if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
                {
                    // If there is an AllowAnonymousAttribute, no need to check for the permission
                    // So we check if there is no AllowAnonymousAttribute, go ahead and check for permsisions.
                    if (controllerActionDescriptor.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>() == null &&
                        controllerActionDescriptor.ControllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>() == null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (CanCheckPermissions(context))
            {
                var permissionAttributes = GetPermissionAttributes(context);

                if (!HasAnyPermissions(context, permissionAttributes))
                {
                    context.Result = new ForbidResult();
                }
            }
        }
    }
}