
using System.Security.Claims;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Envirotrax.App.Server.Filters
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class HasScopeAttribute : Attribute, IAuthorizationFilter
    {
        public string[] AllowedScopes { get; }

        public HasScopeAttribute(params string[] allowedScopes)
        {
            AllowedScopes = allowedScopes;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity is ClaimsIdentity identity)
            {
                if (identity.IsAuthenticated)
                {
                    var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();

                    if (!authService.HasAnyScopes(AllowedScopes))
                    {
                        context.Result = new ForbidResult();
                    }
                }
            }
        }
    }
}