using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Implementations.Lookup;
using Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Data.Services.Implementations;
using Envirotrax.Common.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Configuration;

public static class ServiceRegistration
{
    private static void AddDbContext<TContext>(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        where TContext : DbContext
    {
        services.AddDbContext<TContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException());

            if (environment.IsDevelopment())
            {
                options.EnableDetailedErrors();
                options.EnableSensitiveDataLogging();
                options.LogTo(Console.WriteLine, LogLevel.Information);
            }
        });
    }

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        AddDbContext<TenantDbContext>(services, configuration, environment);
        AddDbContext<ContractorDbContext>(services, configuration, environment);
        services.AddScoped<IDbContextSelector, DbContextSelector>();

        services.AddTenantProvider();
        services.Configure<AdminUserOptions>(configuration.GetSection("AdminUser"));
        services.AddHostedService<SeedDataService>();

        services.AddTransient<IWaterSupplierRepository, WaterSupplierRepository>();
        services.AddTransient<LookupRepository>();
        return services;
    }
}