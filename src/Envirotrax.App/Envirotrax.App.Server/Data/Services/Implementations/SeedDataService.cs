
using Envirotrax.App.Server.Data.Configuration;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Models.WaterSuppliers.Features;
using Envirotrax.App.Server.Data.SeedData;
using Envirotrax.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Envirotrax.App.Server.Data.Services.Implementations;

public class SeedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AdminUserOptions _adminUserOptions;

    private const string AdminRoleName = "Admin";

    private WaterSupplier? _defaultTenant;
    private IDictionary<string, State>? _states;

    public SeedDataService(IServiceProvider serviceProvider, IOptions<AdminUserOptions> adminUserOptions)
    {
        _serviceProvider = serviceProvider;
        _adminUserOptions = adminUserOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await dbContext.Database.MigrateAsync();

            dbContext.SkipSaveSecurityProperties = true;

            await AddTenantsAsync(dbContext);
            await AddFeaturesAsync(dbContext);
            await AddStatesAsync(dbContext);

            await AddPermissionsAsync(dbContext);
            await AddRolesAsync(dbContext);
            await AddUsersAsync(dbContext);

            await AddLicenseTypesAsync(dbContext);
        }
    }

    private async Task AddTenantsAsync(TenantDbContext dbContext)
    {
        _defaultTenant = await dbContext.WaterSuppliers.SingleOrDefaultAsync(tenant => tenant.Domain == WaterSupplier.EnvirotraxAdminDomain);

        if (_defaultTenant == null)
        {
            _defaultTenant = new WaterSupplier
            {
                Name = "Vepo LLC",
                Domain = WaterSupplier.EnvirotraxAdminDomain
            };

            dbContext.WaterSuppliers.Add(_defaultTenant);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task AddUsersAsync(TenantDbContext dbContext)
    {
        if (!await dbContext.WaterSupplierUsers.IgnoreQueryFilters().AnyAsync())
        {
            var normalizedEmail = _adminUserOptions.EmailAddress.ToUpperInvariant();

            var user = await dbContext.AspNetUsers.SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail)
                ?? throw new InvalidOperationException("Error when seeding data. No user with such email address");

            dbContext.WaterSupplierUsers.Add(new()
            {
                WaterSupplierId = _defaultTenant!.Id,
                UserId = user.Id,
                EmailAddress = _adminUserOptions.EmailAddress
            });

            await dbContext.SaveChangesAsync();

            var adminRoleId = await dbContext
                .Roles
                .IgnoreQueryFilters()
                .Where(r => r.Name == AdminRoleName)
                .Select(r => r.Id)
                .SingleAsync();

            dbContext.UserRoles.Add(new()
            {
                WaterSupplierId = _defaultTenant.Id,
                RoleId = adminRoleId,
                UserId = user.Id
            });

            await dbContext.SaveChangesAsync();
        }
    }

    private async Task AddStatesAsync(TenantDbContext dbContext)
    {
        if (!await dbContext.States.AnyAsync())
        {
            dbContext.States.AddRange(StateSeedData.States);
            await dbContext.SaveChangesAsync();
        }

        var states = await dbContext.States.ToListAsync();
        _states = states.ToDictionary(s => s.Code);
    }

    private async Task AddPermissionsAsync(TenantDbContext dbContext)
    {
        if (!await dbContext.Permissions.AnyAsync())
        {
            dbContext.Permissions.AddRange(PermissionSeedData.Permissions);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task AddRolesAsync(TenantDbContext dbContext)
    {
        if (!await dbContext.Roles.IgnoreQueryFilters().AnyAsync())
        {
            var adminRole = new Role
            {
                WaterSupplierId = _defaultTenant!.Id,
                Name = AdminRoleName
            };

            dbContext.Roles.Add(adminRole);
            await dbContext.SaveChangesAsync();

            var rolePermissions = Enum.GetValues<PermissionType>().Select(permissionId => new RolePermission
            {
                WaterSupplierId = _defaultTenant!.Id,
                RoleId = adminRole.Id,
                PermissionId = permissionId,
                CanView = true,
                CanCreate = true,
                CanEdit = true,
                CanDelete = true
            });

            dbContext.RolePermissions.AddRange(rolePermissions);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task AddLicenseTypesAsync(TenantDbContext dbContext)
    {
        if (!await dbContext.ProfessionalLicenseTypes.AnyAsync())
        {
            dbContext.ProfessionalLicenseTypes.AddRange(ProfessionalLicenseTypeSeedData.GetTypes(_states!));
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task AddFeaturesAsync(TenantDbContext dbContext)
    {
        if (!await dbContext.Features.AnyAsync())
        {
            dbContext.Features.AddRange(FeatureSeedData.Features);

            var waterSupplierFeatures = FeatureSeedData
                .Features
                .Select(feature => new WaterSupplierFeature
                {
                    WaterSupplierId = _defaultTenant!.Id,
                    FeatureId = feature.Id
                });

            dbContext.WaterSupplierFeatures.AddRange(waterSupplierFeatures);
            await dbContext.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}