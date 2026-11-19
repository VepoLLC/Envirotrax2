
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Azure.Identity;
using Envirotrax.Auth.Areas.OpenIdConnect.Services;
using Envirotrax.Auth.Data;
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Domain.Services.Definitions;
using Envirotrax.Auth.Domain.Services.Implementations;
using Envirotrax.Common.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
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
                    var issuer = configuration["Sites:EnvirotraxAuth:Url"] ?? throw new InvalidOperationException("Auth site URL is required (Sites:EnvirotraxAuth:Url)");

                    options.SetIssuer(new Uri(issuer, UriKind.Absolute));

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
                    options.RegisterScopes(
                        "openid",
                        "profile",
                        "offline_access",
                        "envirotrax_app",
                        "envirotrax_app_internal",
                        "task_runner",
                        "envirotrax_admin",
                        "envirotrax_admin_internal");

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
                        // On a Windows VM, import the certificate into the Local Machine > Personal store
                        // and set OpenIddict:CertThumbprint in configuration to its thumbprint.
                        string certThumbprint = configuration["OpenIddict:CertThumbprint"] ?? throw new InvalidOperationException("Certificate thumbprint is required (OpenIddict:CertThumbprint)");

                        using var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                        store.Open(OpenFlags.ReadOnly);

                        var matches = store.Certificates.Find(X509FindType.FindByThumbprint, certThumbprint, validOnly: false);

                        if (matches.Count == 0)
                        {
                            throw new Exception($"Certificate with thumbprint '{certThumbprint}' was not found in LocalMachine\\My store");
                        }

                        var cert = matches[0];

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

        private static IServiceCollection AddDataProtection(IServiceCollection services, IConfiguration configuration)
        {
            // Persist the key ring to blob storage so every instance reads and writes the same keys,
            // and wrap those keys with a Key Vault key so they are never stored in plain text.
            // The blob container must already exist. The app identity needs "Storage Blob Data Contributor"
            // on the container and the "Key Vault Crypto Service Encryption User" role (wrap/unwrap) on the key.
            var blobUri = configuration["DataProtection:BlobUri"] ?? throw new InvalidOperationException("Data protection blob URI is required (DataProtection:BlobUri)");
            var keyUri = configuration["DataProtection:KeyUri"] ?? throw new InvalidOperationException("Data protection Key Vault key URI is required (DataProtection:KeyUri)");
            var credential = new DefaultAzureCredential();

            services
                .AddDataProtection()
                // All instances behind the load balancer must share the same application name,
                // otherwise they isolate their payloads from each other even with a shared key ring.
                .SetApplicationName("Envirotrax.Auth")
                .PersistKeysToAzureBlobStorage(new Uri(blobUri), credential)
                .ProtectKeysWithAzureKeyVault(new Uri(keyUri), credential);

            return services;
        }

        public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            AddDataProtection(services, configuration);
            AddOpenIdConnect(services, configuration, environment);

            services.AddScoped<IUserClaimsPrincipalFactory<AppUser>, AppUserClaimsPrincipalFactory>();

            services.AddEmailService(configuration.GetSection("Email"), configuration["Sites:EnvirotraxAuth:Url"], options =>
            {
                options.Assembly = Assembly.GetExecutingAssembly();
                options.Namespace = "Envirotrax.Auth";
            });

            services.AddSmsService(configuration.GetSection("Sms"));

            services.Configure<AdminUserOptions>(configuration.GetSection("AdminUser"));
            services.Configure<SiteOptions>(configuration.GetSection("Sites:EnvirotraxAuth"));
            services.AddHostedService<SeedDataService>();

            services.AddTransient<IAuthService, AuthService>();
            services.AddHttpContextAccessor();
            services.AddTransient<IUserInvitationService, UserInvitationService>();

            return services;
        }
    }
}