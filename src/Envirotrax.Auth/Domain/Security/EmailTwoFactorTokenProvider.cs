using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using OtpNet;

namespace Envirotrax.Auth.Domain.Security;

// Extends the built-in EmailTokenProvider<TUser> (inheriting its CanGenerateTwoFactorTokenAsync
// and GetUserModifierAsync as-is) and only overrides code generation/validation, because that's
// the only place the validity window is decided. ASP.NET Core Identity's own TOTP implementation
// is internal and hardcodes a 3-minute step with a fixed +/-2 step window (~6-9 minutes of
// validity), with no way to widen it, so codes are computed with Otp.NET instead, which exposes a
// configurable verification window: a 3-minute step with a +/-5 step window guarantees a code
// stays valid for at least 15 minutes (worst case ~18 minutes).
public class EmailTwoFactorTokenProvider<TUser> : EmailTokenProvider<TUser> where TUser : class
{
    private const int StepSeconds = 180;
    private static readonly VerificationWindow Window = new(previous: 5, future: 5);

    public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
    {
        var totp = await CreateTotpAsync(purpose, manager, user);
        return totp.ComputeTotp();
    }

    public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
    {
        var totp = await CreateTotpAsync(purpose, manager, user);
        return totp.VerifyTotp(token, out _, Window);
    }

    private async Task<Totp> CreateTotpAsync(string purpose, UserManager<TUser> manager, TUser user)
    {
        var securityToken = await manager.CreateSecurityTokenAsync(user);
        var modifier = await GetUserModifierAsync(purpose, manager, user);

        // Binds the code to the purpose/email the same way Identity's own providers do, so a
        // token generated for one purpose (or a since-changed email) can't validate another.
        var key = HMACSHA1.HashData(securityToken, Encoding.UTF8.GetBytes(modifier));

        return new Totp(key, step: StepSeconds);
    }
}
