
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Envirotrax.Auth.Areas.OpenIdConnect.Services;
using Envirotrax.Auth.Areas.OpenIdConnect.Services.Definitions;
using Envirotrax.Auth.Data;
using Envirotrax.Auth.Domain.Services.Implementations;
using Envirotrax.Common.Configuration;
using Quartz;

namespace Envirotrax.Auth.Domain.Configuration
{
    public static class ServiceRegistration
    {
        private static IServiceCollection AddOpenIdConnect(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.Configure<OpenIddictOptions>(configuration.GetSection("OpenIddict"));

            // OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
            // (like pruning orphaned authorizations/tokens from the database) at regular intervals.
            services.AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            services.AddOpenIddict()

                // Register the OpenIddict core components.
                .AddCore(options =>
                {
                    options.UseQuartz();

                    // Configure OpenIddict to use the EF Core stores/models.
                    options.UseEntityFrameworkCore()
                        .UseDbContext<ApplicationDbContext>();
                })

                // Register the OpenIddict server components.
                .AddServer(options =>
                {
                    options
                        .AllowAuthorizationCodeFlow()
                        .AllowRefreshTokenFlow()
                        .AllowClientCredentialsFlow();

                    options
                        .SetTokenEndpointUris("/oauth/connect/token")
                        .SetAuthorizationEndpointUris("/oauth/connect/authorize")
                        .SetUserInfoEndpointUris("/oauth/connect/userinfo")
                        .SetEndSessionEndpointUris("oauth/connect/logout");

                    // Encryption and signing of tokens
                    options
                        .AddEphemeralEncryptionKey()
                        .AddEphemeralSigningKey();

                    // Register scopes (permissions)
                    options.RegisterScopes("envirotrax_app");

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    var aspNetOptions = options.UseAspNetCore();

                    aspNetOptions
                        .EnableTokenEndpointPassthrough()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableUserInfoEndpointPassthrough()
                        .EnableEndSessionEndpointPassthrough();

                    options.DisableAccessTokenEncryption();

                    if (environment.IsDevelopment())
                    {
                        options.AddDevelopmentSigningCertificate();
                    }
                    else
                    {
                        // IMPORTANT:
                        // The certificate must not be checked in to source control. 
                        // For example, If we host the App in Azure, it can be stored in App Service
                        string certThumbprint = configuration["OpenIddict:CertThumbprint"] ?? throw new InvalidOperationException("We need certificate thumbprint from Azure App Service");

                        var filePath = $"/var/ssl/private/{certThumbprint}.p12";

                        if (!File.Exists(filePath))
                        {
                            throw new Exception($"Certificate with thumbprint {certThumbprint} was not found");
                        }

                        var bytes = File.ReadAllBytes(filePath);
                        var cert = X509CertificateLoader.LoadCertificate(bytes);

                        options
                            .AddEncryptionCertificate(cert)
                            .AddSigningCertificate(cert);
                    }
                })

                // Register the OpenIddict validation components.
                .AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();

                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            AddOpenIdConnect(services, configuration, environment);

            services.AddEmailService(configuration.GetSection("Email"), options =>
            {
                options.Assembly = Assembly.GetExecutingAssembly();
                options.Namespace = "Envirotrax.Auth";
            });

            services.Configure<AdminUserOptions>(configuration.GetSection("AdminUser"));
            services.AddHostedService<SeedDataService>();

            services.AddTransient<IAuthService, AuthService>();

            return services;
        }
    }
}