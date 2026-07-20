
using Envirotrax.LegacyDataMigration.Data;
using Envirotrax.LegacyDataMigration.Data.Users;
using Envirotrax.LegacyDataMigration.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var legacyV1DatabasConnection = "Your V1 connection string here.";
var newV2DatabaseConnection = "Your V2 connectiong string here.";

var services = new ServiceCollection();

services.AddTransient(_ => new LegacyDbService(legacyV1DatabasConnection));

services
    .AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(newV2DatabaseConnection);

        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    });

services
    .AddIdentityCore<AppUser>()
    .AddRoles<IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>();

services.AddLogging(builder => builder.AddConsole());

services.AddTransient<UserService>();

var provider = services.BuildServiceProvider();

var userService = provider.GetRequiredService<UserService>();
await userService.MigrateAsync();