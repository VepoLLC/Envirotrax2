using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Implementations.Sites;
using Envirotrax.App.Server.Domain.Services.Implementations.Users;
using Envirotrax.App.Server.Domain.Services.Implementations;
using Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;
using Envirotrax.Common.Configuration;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Implementations.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions;

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
        services.AddTransient<ISiteService, SiteService>();
        services.AddTransient<ILookupService, LookupService>();

        services.AddTransient<IProfessionalService, ProfessionalService>();
        services.AddTransient<IProfessionalUserService, ProfessionalUserService>();
        services.AddTransient<IProfessionalSupplierService, ProfessionalSupplierService>();

        return services;
    }
}