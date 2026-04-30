using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Implementations.Backflow;
using Envirotrax.App.Server.Domain.Services.Implementations.Csi;
using Envirotrax.App.Server.Domain.Services.Implementations.Fog;
using Envirotrax.App.Server.Domain.Services.Implementations.GisAreas;
using Envirotrax.App.Server.Domain.Services.Implementations.Sites;
using Envirotrax.App.Server.Domain.Services.Implementations.Users;
using Envirotrax.App.Server.Domain.Services.Implementations;
using Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;
using Envirotrax.Common.Configuration;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Implementations.Professionals;
using Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Implementations.Helpers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

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
        services.AddTransient<ITimeZoneHelperService, TimeZoneHelperService>();

        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        services.AddTransient<IFileStorageService, FileStorageService>();

        services.AddTransient<IWaterSupplierService, WaterSupplierService>();
        services.AddTransient<IGeneralSettingsService, GeneralSettingsService>();
        services.AddTransient<ISiteService, SiteService>();
        services.AddTransient<ICsiInspectionService, CsiInspectionService>();
        services.AddTransient<ICsiInspectorService, CsiInspectorService>();
        services.AddTransient<IFogInspectorService, FogInspectorService>();
        services.AddTransient<IFogInspectionService, FogInspectionService>();
        services.AddTransient<IBackflowTesterService, BackflowTesterService>();
        services.AddTransient<IBackflowTestService, BackflowTestService>();
        services.AddTransient<ILookupService, LookupService>();

        services.AddTransient<ICsiSettingsService, CsiSettingsService>();
        services.AddTransient<IBackflowSettingsService, BackflowSettingsService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserRoleService, UserRoleService>();
        services.AddTransient<IRolePermissionService, RolePermissionService>();
        services.AddTransient<IRoleService, RoleService>();

        services.AddTransient<IProfessionalService, ProfessionalService>();
        services.AddTransient<IProfessionalUserService, ProfessionalUserService>();
        services.AddTransient<IProfessionalSupplierService, ProfessionalSupplierService>();
        services.AddTransient<IProfessionalUserLicenseService, ProfessionalUserLicenseService>();
        services.AddTransient<IProfessionalLicenseTypeService, ProfessionalLicenseTypeService>();
        services.AddTransient<IProfessionalInsuranceService, ProfessionalInsuranceService>();

        services.AddTransient<IGisAreaService, GisAreaService>();
        services.AddTransient<IGisAreaCoordinateService, GisAreaCoordinateService>();

        return services;
    }
}