using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Users;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;
using Envirotrax.App.Server.Data.Repositories.Implementations.Csi;
using Envirotrax.App.Server.Data.Repositories.Implementations.Fog;
using Envirotrax.App.Server.Data.Repositories.Implementations.GisAreas;
using Envirotrax.App.Server.Data.Repositories.Implementations.Sites;
using Envirotrax.App.Server.Data.Repositories.Implementations.Users;
using Envirotrax.App.Server.Data.Repositories.Implementations.Lookup;
using Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Data.Services.Implementations;
using Envirotrax.Common.Configuration;
using Microsoft.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;
using Envirotrax.App.Server.Data.Repositories.Implementations.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

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
        AddDbContext<ProfessionalDbContext>(services, configuration, environment);
        services.AddScoped<IDbContextSelector, DbContextSelector>();

        services.AddTenantProvider();
        services.Configure<AdminUserOptions>(configuration.GetSection("AdminUser"));
        services.AddHostedService<SeedDataService>();

        services.AddTransient<IWaterSupplierRepository, WaterSupplierRepository>();
        services.AddTransient<IGeneralSettingsRepository, GeneralSettingsRepository>();
        services.AddTransient<ISiteRepository, SiteRepository>();
        services.AddTransient<ICsiInspectionRepository, CsiInspectionRepository>();
        services.AddTransient<ICsiInspectorRepository, CsiInspectorRepository>();
        services.AddTransient<IFogInspectorRepository, FogInspectorRepository>();
        services.AddTransient<IBackflowTesterRepository, BackflowTesterRepository>();
        services.AddTransient<IBackflowTestRepository, BackflowTestRepository>();
        services.AddTransient<LookupRepository>();

        services.AddTransient<ICsiSettingsRepository, CsiSettingsRepository>();
        services.AddTransient<IBackflowSettingsRepository, BackflowSettingsRepository>();
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IRoleRepository, RoleRepository>();
        services.AddTransient<IRolePermissionRepository, RolePermissionRepository>();
        services.AddTransient<IUserRoleRepository, UserReoleRepository>();

        services.AddTransient<IProfessionalRepository, ProfessionalRepository>();
        services.AddTransient<IProfessionalUserRepository, ProfessionalUserRepository>();
        services.AddTransient<IProfessionalSupplierRepository, ProfessionalSupplierRepository>();
        services.AddTransient<IProfessionalUserLicenseRepository, ProfessionalUserLicenseRepository>();
        services.AddTransient<IProfessionalLicenseTypeRepository, ProfessionalLicenseTypeRepository>();
        services.AddTransient<IProfessionalInsuranceRepository, ProfessionalInsuranceRepository>();
        services.AddTransient<IBackflowGaugeRepository, BackflowGaugeRepository>();

        services.AddTransient<IGisAreaRepository, GisAreaRepository>();
        services.AddTransient<IGisAreaCoordinateRepository, GisAreaCoordinateRepository>();

        return services;
    }
}