
using Envirotrax.Common.Domain.Services.Implementations;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.Common.Configuration;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Implementations;
using Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Admin.Server.Domain.Services.Implementations.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IQueryHelperService, QueryHelperService>();
        services.AddInternalApi<EnvirotraxApiOptions>(configuration.GetSection("EnvirotraxApi"));
        services.AddTransient<IEnvirotraxApiClient, EnvirotraxApiClient>();

        // Register services here:
        services.AddTransient<IWaterSupplierService, WaterSupplierService>();

        return services;
    }
}