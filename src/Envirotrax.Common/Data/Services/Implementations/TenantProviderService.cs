
using System.Security.Claims;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace Envirotrax.Common.Data.Services.Implementations
{
    public class TenantProviderService : ITenantProvidersService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public int WaterSupplierId
        {
            get
            {
                var tenantIdClaim = _contextAccessor.HttpContext?.User.FindFirstValue("wsId");

                if (int.TryParse(tenantIdClaim, out var tenantId))
                {
                    return tenantId;
                }

                return default;
            }
        }

        public int ParentWaterSupplierId
        {
            get
            {
                var tenantIdClaim = _contextAccessor.HttpContext?.User.FindFirstValue("pWsId");

                if (int.TryParse(tenantIdClaim, out var tenantId))
                {
                    return tenantId;
                }

                return default;
            }
        }

        public string Domain
        {
            get
            {
                var domainClaim = _contextAccessor.HttpContext?.User.FindFirstValue("dmn");
                return domainClaim ?? string.Empty;
            }
        }

        public int UserId
        {
            get
            {
                var userIdClaim = _contextAccessor.HttpContext?.User.FindFirstValue(OpenIddictConstants.Claims.Subject);

                if (int.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }

                return default;
            }
        }

        public TenantProviderService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        private ClaimsIdentity GetClaimsIdentity()
        {
            var identity = _contextAccessor.HttpContext!.User?.Identity as ClaimsIdentity;

            if (identity == null)
            {
                identity = new ClaimsIdentity();
                _contextAccessor.HttpContext!.User = new ClaimsPrincipal(identity);
            }

            return identity;
        }

        public void SetWaterSupplierId(int supplierId)
        {
            var identity = GetClaimsIdentity();
            identity.AddClaim(new Claim("wsId", supplierId.ToString()).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
        }

        public void SetWaterSupplier(ClaimsPrincipal principal, int supplierId)
        {
            SetWaterSupplierId(supplierId);

            if (principal.Identity is ClaimsIdentity claimsIdentity)
            {
                claimsIdentity.AddClaim(new Claim("wsId", supplierId.ToString()).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }
        }

        public void SetParentWaterSupplier(ClaimsPrincipal principal, int supplierId)
        {
            if (principal.Identity is ClaimsIdentity claimsIdentity)
            {
                claimsIdentity.AddClaim(new Claim("pWsId", supplierId.ToString()).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }
        }

        public void SetDomain(ClaimsPrincipal principal, string domain)
        {
            if (principal.Identity is ClaimsIdentity claimsIdentity)
            {
                claimsIdentity.AddClaim(new Claim("dmn", domain).SetDestinations(OpenIddictConstants.Destinations.AccessToken));
            }
        }

        public bool HasScope(string scopeToCheck)
        {
            var scope = _contextAccessor.HttpContext?.User?.FindFirstValue("scope");

            if (!string.IsNullOrWhiteSpace(scope))
            {
                return scope
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Any(s => s == scopeToCheck);
            }

            return false;
        }
    }
}