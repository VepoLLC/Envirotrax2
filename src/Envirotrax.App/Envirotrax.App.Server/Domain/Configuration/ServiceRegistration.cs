using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Fog;
using Envirotrax.App.Server.Domain.Services.Definitions.GisAreas;
using Envirotrax.App.Server.Domain.Services.Definitions.Logs;
using Envirotrax.App.Server.Domain.Services.Definitions.Notifications;
using Envirotrax.App.Server.Domain.Services.Implementations.Logs;
using Envirotrax.App.Server.Domain.Services.Implementations.Notifications;
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
using Envirotrax.Common.Domain.Services.Defintions;
using Envirotrax.Common.Domain.Services.Implementations;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Implementations.Professionals;
using Envirotrax.App.Server.Domain.Services.Implementations.Professionals.Licenses;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.Helpers;
using Envirotrax.App.Server.Domain.Services.Implementations.Helpers;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;
using Envirotrax.App.Server.Domain.Services.Implementations.Payments;

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
        services.AddTransient<ISiteLogService, SiteLogService>();
        services.AddTransient<ICsiInspectionService, CsiInspectionService>();
        services.AddTransient<ICsiInspectionImageService, CsiInspectionImageService>();
        services.AddTransient<ICsiInspectionAssemblyService, CsiInspectionAssemblyService>();
        services.AddTransient<IRecordLogService, RecordLogService>();
        services.AddTransient<ICsiInspectorService, CsiInspectorService>();
        services.AddTransient<ICsiInspectorAccountService, CsiInspectorAccountService>();
        services.AddTransient<ICsiSystemReportService, CsiSystemReportService>();
        services.AddTransient<IFogInspectorService, FogInspectorService>();
        services.AddTransient<IFogTransporterService, FogTransporterService>();
        services.AddTransient<IFogInspectionService, FogInspectionService>();
        services.AddTransient<IBackflowTesterService, BackflowTesterService>();
        services.AddTransient<IBackflowTestService, BackflowTestService>();
        services.AddTransient<IBackflowTestReportService, BackflowTestReportService>();
        services.AddTransient<IBackflowComplianceReportService, BackflowComplianceReportService>();
        services.AddTransient<IBackflowComplianceSnapshotService, BackflowComplianceSnapshotService>();
        services.AddTransient<IBackflowNewRemovedReportService, BackflowNewRemovedReportService>();
        services.AddTransient<IBackflowGaugeService, BackflowGaugeService>();
        services.AddTransient<IBackflowOutOfServiceRequestService, BackflowOutOfServiceRequestService>();
        services.AddTransient<IFogVehicleService, FogVehicleService>();
        services.AddTransient<IFogVehiclePermitService, FogVehiclePermitService>();
        services.AddTransient<IFogTransporterDisposalSiteService, FogTransporterDisposalSiteService>();
        services.AddTransient<IFogDisposalSiteService, FogDisposalSiteService>();
        services.AddTransient<IFogTripTicketService, FogTripTicketService>();
        services.AddTransient<IFogSettingsService, FogSettingsService>();
        services.AddTransient<IFogSystemReportService, FogSystemReportService>();
        services.AddTransient<ILookupService, LookupService>();

        services.AddTransient<ICsiSettingsService, CsiSettingsService>();
        services.AddTransient<IBackflowSettingsService, BackflowSettingsService>();
        services.AddTransient<IBackflowRenewalRequirementService, BackflowRenewalRequirementService>();
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IUserRoleService, UserRoleService>();
        services.AddTransient<IRolePermissionService, RolePermissionService>();
        services.AddTransient<IRoleService, RoleService>();

        services.AddTransient<IProfessionalService, ProfessionalService>();
        services.AddTransient<IRegisteredProfessionalService, RegisteredProfessionalService>();
        services.AddTransient<IProfessionalUserService, ProfessionalUserService>();
        services.AddTransient<IProfessionalSupplierService, ProfessionalSupplierService>();
        services.AddTransient<IProfessionalUserLicenseService, ProfessionalUserLicenseService>();
        services.AddTransient<IProfessionalLicenseTypeService, ProfessionalLicenseTypeService>();
        services.AddTransient<IProfessionalInsuranceService, ProfessionalInsuranceService>();

        services.AddTransient<IGisAreaService, GisAreaService>();
        services.AddTransient<IGisAreaCoordinateService, GisAreaCoordinateService>();

        services.AddTransient<INotificationSettingService, NotificationSettingService>();

        services.AddTransient<IWaterSupplierDashboardService, WaterSupplierDashboardService>();

        services.Configure<GeocodingOptions>(configuration.GetSection("Geocoding"));
        services.AddHttpClient<IGeocodingService, GeocodingService>();

        services.Configure<AuthorizeNetOptions>(configuration.GetSection("AuthorizeNet"));
        services.AddHttpClient<IAuthorizeNetPaymentService, AuthorizeNetPaymentService>();

        services.AddHtmlTemplateService(opts =>
        {
            opts.Assembly = typeof(ServiceRegistration).Assembly;
            opts.Namespace = "Envirotrax.App.Server";
        });
        services.AddPdfTemplateService(environment.IsDevelopment(), configuration.GetSection("PdfTemplate"));

        services.AddTransient<ICsvHelperService, CsvHelperService>();

        return services;
    }
}