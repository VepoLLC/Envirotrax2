
using Envirotrax.App.Server.Data.Configuration;
using Envirotrax.App.Server.Domain.Configuration;
using OpenIddict.Validation.AspNetCore;

namespace Envirotrax.App.Server.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        services
            .AddOpenIddict()
            .AddValidation(options =>
            {
                var authOptions = new AuthServerOptions();
                configuration.GetSection("AuthServer").Bind(authOptions);

                options.SetIssuer(authOptions.Issuer ?? throw new InvalidOperationException("Issuer not set in settings."));

                if (!string.IsNullOrEmpty(authOptions.Audiences))
                {
                    var audiences = authOptions.Audiences.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    options.AddAudiences(audiences);
                }

                // Register the System.Net.Http integration.
                options.UseSystemNetHttp();

                // Register the ASP.NET Core host.
                options.UseAspNetCore();
            });

        return services
            .AddDataServices(configuration, environment)
            .AddDomainServices(configuration, environment);
    }
}