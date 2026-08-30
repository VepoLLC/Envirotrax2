

using System;
using System.Security.Claims;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Security;
using Envirotrax.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Envirotrax.Auth.Domain.Services.Implementations;

public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser>
{
    public AppUserClaimsPrincipalFactory(
        UserManager<AppUser> userManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        if (user.IsSuperUser)
        {
            identity.AddClaim(new Claim(ClaimTypes.Role, RoleDefinitions.SuperUser));
        }

        if (user.PasswordExpirationDate.HasValue && user.PasswordExpirationDate.Value <= DateTime.UtcNow)
        {
            identity.AddClaim(new Claim(AppClaimTypes.PasswordExpired, "true"));
        }

        return identity;
    }
}