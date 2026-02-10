
using System.Reflection;
using DeveloperPartners.SortingFiltering;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Envirotrax.App.Server.Filters;

public class CheckFeaturesFilter : IAuthorizationFilter
{
    private IEnumerable<HasFeatureAttribute> GetFeatureAttributes(AuthorizationFilterContext context)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            var featureAttributes = controllerActionDescriptor
                .ControllerTypeInfo
                .GetCustomAttributes<HasFeatureAttribute>()
                .Concat(controllerActionDescriptor.MethodInfo.GetCustomAttributes<HasFeatureAttribute>());

            return featureAttributes;
        }

        return [];
    }

    private bool HasAnyFeatures(AuthorizationFilterContext context, IEnumerable<HasFeatureAttribute> hasFeatureAttributes)
    {
        // If the endpoint requires no permissions, let the user call it.
        if (hasFeatureAttributes.IsNullOrEmpty())
        {
            return true;
        }

        var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>(); ;

        foreach (var feature in hasFeatureAttributes)
        {
            if (authService.HasAnyFeatures(feature.Features))
            {
                return true;
            }
        }

        return false;
    }


    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var featureAttributes = GetFeatureAttributes(context);

        if (!HasAnyFeatures(context, featureAttributes))
        {
            context.Result = new ForbidResult();
        }
    }
}