

using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

public class AdminDbContext : TenantDbContext
{
    protected AdminDbContext(
        DbContextOptions options,
        ILogger<AdminDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    public AdminDbContext(
        DbContextOptions<AdminDbContext> options,
        ILogger<AdminDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    protected override void SetSecurityProperties()
    {
        // Set no automatic tenant IDs. Admins can edit records from all tenants.
    }

    protected override void SetupGlobalFiltering(ModelBuilder builder, IMutableEntityType entity)
    {
        // Do no filtering for admins. They can see everything.
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}