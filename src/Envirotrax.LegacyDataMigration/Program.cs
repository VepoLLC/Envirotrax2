
using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Users;
using Envirotrax.LegacyDataMigration.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

var newV2DatabaseConnection = @"Server=(localdb)\mssqllocaldb;Database=Envirotrax2Dev;Trusted_Connection=True;MultipleActiveResultSets=true";

// Where V1 file attachments are downloaded from, and the Azure Storage account they are uploaded to.
// The storage account is REAL and SHARED - there is no local stand-in for it - and DefaultAzureCredential
// signs in as you, so your Azure login needs write access to it. See the README before running.
var legacyFileServerAddress = "https://iofiles.envirotrax.com";
var azureStorageAccountName = "envirotrax2dev";
var azureStorageContainerName = "default";

var services = new ServiceCollection();

void ConfigureDbContext(DbContextOptionsBuilder options)
{
    options.UseSqlServer(newV2DatabaseConnection, sqlServerOptions => sqlServerOptions.CommandTimeout(120));

    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine, LogLevel.Information);
}

services.AddDbContext<AppIdentityDbContext>(options => ConfigureDbContext(options));
services.AddDbContext<AppDbContext>(options => ConfigureDbContext(options));

services.AddSingleton<ITenantProvidersService, MigrationTenantProvidersService>();

services
    .AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppIdentityDbContext>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Map(
        Serilog.Core.Constants.SourceContextPropertyName,
        "General",
        (sourceContext, writeTo) => writeTo.File($"Logs/{sourceContext.Split('.').Last()}.log"))
    .CreateLogger();

services.AddLogging(builder => builder.AddSerilog(Log.Logger, dispose: true));

services.AddSingleton(new LegacyFileServerService(legacyFileServerAddress));
services.AddSingleton(new BlobStorageService(azureStorageAccountName, azureStorageContainerName));

services.AddTransient<UserService>();
services.AddTransient<WaterSupplierService>();
services.AddTransient<WaterSupplierUserService>();
services.AddTransient<ProfessionalService>();
services.AddTransient<ProfessionalWaterSupplierService>();
services.AddTransient<ProfessionalLicenseService>();
services.AddTransient<ProfessionalInsuranceService>();
services.AddTransient<GisAreaService>();
services.AddTransient<SiteService>();
services.AddTransient<SiteLogService>();
services.AddTransient<RenewalEmailVerificationService>();
services.AddTransient<BackflowGaugeService>();
services.AddTransient<BackflowTestService>();
services.AddTransient<CsiInspectionService>();
services.AddTransient<FogInspectionService>();
services.AddTransient<FogDisposalSiteService>();
services.AddTransient<FogVehicleService>();
services.AddTransient<FogTripTicketService>();
services.AddTransient<NotificationSettingService>();
services.AddTransient<NotificationService>();
services.AddTransient<ApiAccountService>();
services.AddTransient<SiteScheduleService>();

var provider = services.BuildServiceProvider();

/***********************************************************************************************************************************************/
/* Migration code starts from here. The code above was for configruation. Everything below this line is the main business logic of this app.   */
/***********************************************************************************************************************************************/
var userService = provider.GetRequiredService<UserService>();
await userService.MigrateAsync();

var waterSupplierService = provider.GetRequiredService<WaterSupplierService>();
await waterSupplierService.MigrateAsync();

var supplierUserService = provider.GetRequiredService<WaterSupplierUserService>();
await supplierUserService.MigrateAsync();

var professionalService = provider.GetRequiredService<ProfessionalService>();
await professionalService.MigrateAsync();

var professionalWaterSupplierService = provider.GetRequiredService<ProfessionalWaterSupplierService>();
await professionalWaterSupplierService.MigrateAsync();

var professionalLicenseService = provider.GetRequiredService<ProfessionalLicenseService>();
await professionalLicenseService.MigrateAsync();

var professionalInsuranceService = provider.GetRequiredService<ProfessionalInsuranceService>();
await professionalInsuranceService.MigrateAsync();

var gisAreaService = provider.GetRequiredService<GisAreaService>();
await gisAreaService.MigrateAsync();

var siteService = provider.GetRequiredService<SiteService>();
await siteService.MigrateAsync();

// After the sites: a pending verification belongs to a site, and its address is added to that site
// once it is confirmed.
var renewalEmailVerificationService = provider.GetRequiredService<RenewalEmailVerificationService>();
await renewalEmailVerificationService.MigrateAsync();

var siteLogService = provider.GetRequiredService<SiteLogService>();
await siteLogService.MigrateAsync();

// After the professionals: a gauge hangs off the company, and it is resolved through the staging table
// that the professional migration builds.
var backflowGaugeService = provider.GetRequiredService<BackflowGaugeService>();
await backflowGaugeService.MigrateAsync();

// After the sites and the professionals, because a test is recorded against a site by a tester who
// belongs to one of them, and before the notifications, whose RecordId points at a test by its V1 id.
var backflowTestService = provider.GetRequiredService<BackflowTestService>();
await backflowTestService.MigrateAsync();

var csiInspectionService = provider.GetRequiredService<CsiInspectionService>();
await csiInspectionService.MigrateAsync();

// After the sites and the professionals, because an inspection is recorded against a site by an
// inspector who belongs to one of them. Its photos and signature are moved into Azure Storage here too.
var fogInspectionService = provider.GetRequiredService<FogInspectionService>();
await fogInspectionService.MigrateAsync();

// A global master list that depends on nothing else, but it has to land before the transporter
// registrations and trip tickets, which find a disposal site by its V1 id through LegacyRecordId.
var fogDisposalSiteService = provider.GetRequiredService<FogDisposalSiteService>();
await fogDisposalSiteService.MigrateAsync();

// After the professionals and the water suppliers: a vehicle belongs to a transporter's company, found
// through the staging table the professional migration builds, and each permit is issued by a supplier.
// Before the trip tickets, which find a vehicle by its V1 id through LegacyRecordId.
var fogVehicleService = provider.GetRequiredService<FogVehicleService>();
await fogVehicleService.MigrateAsync();

// After the sites, the professionals, the disposal sites and the vehicles: a ticket records a pickup at a
// site by a transporter, in one of their vehicles, delivered to a disposal site, and it finds each of them
// by its V1 id through LegacyRecordId. Its signatures are moved into Azure Storage here too.
var fogTripTicketService = provider.GetRequiredService<FogTripTicketService>();
await fogTripTicketService.MigrateAsync();

// After the supplier users: a notification setting is delivered to one of them, and V2 requires
// that recipient, so a setting whose user is missing cannot be migrated at all.
var notificationSettingService = provider.GetRequiredService<NotificationSettingService>();
await notificationSettingService.MigrateAsync();

// Last, because a notification points at the record that raised it through RecordId. Every row V1
// can produce is a backflow one, so that pointer is always a backflow test id. It is carried over
// unresolved: V2 row ids equal the V1 ids, so it lines up once the backflow test migration lands.
var notificationService = provider.GetRequiredService<NotificationService>();
await notificationService.MigrateAsync();

var apiAccountService = provider.GetRequiredService<ApiAccountService>();
await apiAccountService.MigrateAsync();

var siteScheduleService = provider.GetRequiredService<SiteScheduleService>();
await siteScheduleService.MigrateAsync();