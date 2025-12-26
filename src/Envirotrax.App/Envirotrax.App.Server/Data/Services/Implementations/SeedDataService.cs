
using Envirotrax.App.Server.Data.Configuration;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Envirotrax.App.Server.Data.Services.Implementations;

public class SeedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AdminUserOptions _adminUserOptions;

    private WaterSupplier? _defaultTenant;

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
            await AddUsersAsync(dbContext);
        }
    }

    private async Task AddTenantsAsync(TenantDbContext dbContext)
    {
        _defaultTenant = await dbContext.WaterSuppliers.SingleOrDefaultAsync(tenant => tenant.Domain == "vepollc");

        if (_defaultTenant == null)
        {
            _defaultTenant = new WaterSupplier
            {
                Name = "Vepo LLC",
                Domain = "vepollc"
            };

            dbContext.WaterSuppliers.Add(_defaultTenant);
            await dbContext.SaveChangesAsync();
        }
    }

    private async Task AddUsersAsync(TenantDbContext dbContext)
    {
        if (await dbContext.WaterSupplierUsers.AnyAsync())
        {
            var normalizedEmail = _adminUserOptions.EmailAddress.ToUpperInvariant();

            var user = await dbContext.AspNetUsers.SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail)
                ?? throw new InvalidOperationException("Error when seeding data. No user with such email address");

            dbContext.WaterSupplierUsers.Add(new()
            {
                WaterSupplierId = _defaultTenant!.Id,
                UserId = user.Id
            });

            await dbContext.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}