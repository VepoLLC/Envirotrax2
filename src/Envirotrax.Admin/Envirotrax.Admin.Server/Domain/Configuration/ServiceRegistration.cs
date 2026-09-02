
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
using Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;
using Envirotrax.Admin.Server.Domain.Services.Implementations.Csi;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Implementations.Backflow;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;
using Envirotrax.Admin.Server.Domain.Services.Implementations.Fog;

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
                services.AddTransient<IWaterSupplierUserService, WaterSupplierUserService>();
                services.AddTransient<ISiteService, SiteService>();
                services.AddTransient<ILookupService, LookupService>();
                services.AddTransient<IGoogleMapsService, GoogleMapsService>();
                services.AddTransient<ICsiInspectionService, CsiInspectionService>();
                services.AddTransient<ICsiInspectorService, CsiInspectorService>();
                services.AddTransient<ICsiInspectorUserService, CsiInspectorUserService>();
                services.AddTransient<ICsiInspectorLicenseService, CsiInspectorLicenseService>();
                services.AddTransient<ICsiInspectorInsuranceService, CsiInspectorInsuranceService>();
                services.AddTransient<IBackflowTestService, BackflowTestService>();
                services.AddTransient<IFogInspectionService, FogInspectionService>();

                return services;
        }
}