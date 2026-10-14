using System.Threading.RateLimiting;

namespace Envirotrax.App.Server.Configuration;

public static class RateLimitPolicies
{
    // Anonymous endpoints guarded by a short code (renewal opt-in passcode, email links), limited
    // per IP address so the codes cannot be brute-forced.
    public const string AnonymousCodeCheck = nameof(AnonymousCodeCheck);

    public static IServiceCollection AddRateLimitPolicies(this IServiceCollection services)
    {
        return services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(AnonymousCodeCheck, context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? string.Empty,
                _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(15)
                }));
        });
    }
}
