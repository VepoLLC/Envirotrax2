
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.SeedData;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Services.Implementations;

public class SeedDataService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;

    private WaterSupplier? _defaultTenant;

    public SeedDataService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<TenantDbContext>();
            await dbContext.Database.MigrateAsync();

            dbContext.SkipSaveSecurityProperties = true;

            await AddTenantsAsync(dbContext);
            await AddStatesAsync(dbContext);
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

    private async Task AddStatesAsync(TenantDbContext dbContext)
    {
        if (!await dbContext.States.AnyAsync())
        {
            dbContext.States.AddRange(StateSeedData.States);
            await dbContext.SaveChangesAsync();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}