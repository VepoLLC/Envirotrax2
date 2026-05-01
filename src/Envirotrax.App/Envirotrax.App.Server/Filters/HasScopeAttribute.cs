
using System.Security.Claims;
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

        private bool HasAnyScopes(ClaimsIdentity identity, string[] scopes)
        {
            foreach (var scope in scopes)
            {
                if (identity.HasClaim("scope", scope))
                {
                    return true;
                }
            }

            return false;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity is ClaimsIdentity identity)
            {
                if (identity.IsAuthenticated)
                {
                    if (!HasAnyScopes(identity, AllowedScopes))
                    {
                        context.Result = new ForbidResult();
                    }
                }
            }
        }
    }
}