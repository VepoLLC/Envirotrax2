
using Envirotrax.Common.Domain.Services.Implementations;
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.Common.Configuration;
using Envirotrax.Admin.Server.Domain.Services.Definitions;
using Envirotrax.Admin.Server.Domain.Services.Implementations;
using Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.Admin.Server.Domain.Services.Implementations.WaterSuppliers;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Sites;
using Envirotrax.Admin.Server.Domain.Services.Implementations.Sites;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Lookup;
using Envirotrax.Admin.Server.Domain.Services.Implementations.Lookup;
using Envirotrax.Admin.Server.Domain.Services.Definitions.GoogleMaps;
using Envirotrax.Admin.Server.Domain.Services.Implementations.GoogleMaps;

namespace Envirotrax.Admin.Server.Domain.Configuration;

public static class ServiceRegistration
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthService();
        services.AddSingleton<IQueryHelperService, QueryHelperService>();
        services.AddInternalApi<EnvirotraxApiOptions>(configuration.GetSection("EnvirotraxApi"));
        services.AddTransient<IEnvirotraxApiClient, EnvirotraxApiClient>();

        // Register services here:
        services.AddTransient<IWaterSupplierService, WaterSupplierService>();
        services.AddTransient<ISiteService, SiteService>();
        services.AddTransient<ILookupService, LookupService>();
        services.AddTransient<IGoogleMapsService, GoogleMapsService>();

        return services;
    }
}