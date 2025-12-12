
using Envirotrax.App.Server.Data.DbContexts;
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

        services.AddTenantProvider();

        return services;
    }
}