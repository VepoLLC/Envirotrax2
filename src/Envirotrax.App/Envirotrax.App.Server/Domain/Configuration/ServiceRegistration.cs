using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Implementations.Users;
using Envirotrax.App.Server.Domain.Services.Implementations.Lookup;
using Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;
using Envirotrax.Common.Configuration;

namespace Envirotrax.App.Server.Domain.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
    {
        services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(ServiceRegistration).Assembly);
            config.LicenseKey = configuration["AutoMapper:LicenseKey"];
        });

        services.AddInternalApi<AuthApiOptions>(configuration.GetSection("AuthApi"));

        services.AddTransient<IWaterSupplierService, WaterSupplierService>();
        services.AddTransient<IGeneralSettingsService, GeneralSettingsService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<LookupService>();

        return services;
    }
}