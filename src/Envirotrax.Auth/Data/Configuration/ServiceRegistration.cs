
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Data.Repositories.Implementations;
using Envirotrax.Auth.Domain.Security;
using Envirotrax.Common.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException(),
                sqlOptions => sqlOptions.EnableRetryOnFailure());

            // Register the entity sets needed by OpenIddict
            options.UseOpenIddict();

            if (environment.IsDevelopment())
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });

        services.AddDatabaseDeveloperPageExceptionFilter();

        services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            // Overrides the built-in "Email" 2FA token provider to extend code validity to 15
            // minutes; the "Phone" (SMS) provider registered above is left untouched.
            .AddTokenProvider<EmailTwoFactorTokenProvider<AppUser>>("Email");

        services.AddTransient<IWaterSupplierUserRepository, WaterSupplierUserRepository>();
        services.AddTransient<IProfessionalUserRepository, ProfessionalUserRepository>();
        services.AddTransient<IUserInvitationReppsitory, UserInvitationReppsitory>();
        services.AddTransient<IFeatureRepository, FeatureRepository>();

        return services;
    }
}