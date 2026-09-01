
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

services.AddTransient<UserService>();
services.AddTransient<WaterSupplierService>();
services.AddTransient<WaterSupplierUserService>();
services.AddTransient<SiteService>();

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

var siteService = provider.GetRequiredService<SiteService>();
await siteService.MigrateAsync();