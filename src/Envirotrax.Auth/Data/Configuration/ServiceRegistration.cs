
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Auth.Data.Repositories.Implementations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException());

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
            .AddDefaultTokenProviders();

        services.AddTransient<IWaterSupplierUserRepository, WaterSupplierUserRepository>();
        services.AddTransient<IContractorUserRepository, ContractorUserRepository>();

        return services;
    }
}