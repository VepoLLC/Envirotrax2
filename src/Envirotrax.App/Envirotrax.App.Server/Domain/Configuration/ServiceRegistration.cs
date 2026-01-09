
using Envirotrax.App.Server.Domain.Services.Definitions.States;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Implementations.States;
using Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

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

        services.AddTransient<IWaterSupplierService, WaterSupplierService>();
        services.AddTransient<IStateService, StateService>();

        return services;
    }
}