
using Envirotrax.Common.Data.DbContexts;
using Envirotrax.Common.Data.Services.Definitions;
using Envirotrax.LegacyDataMigration.Data.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace Envirotrax.LegacyDataMigration.Data;

public class AppDbContext : TenantDbContextBase<WaterSupplier>
{
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<WaterSupplierUser> WaterSupplierUsers { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger, ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    protected override void SetupGlobalFiltering(ModelBuilder builder, IMutableEntityType entity)
    {
        // No filtering - this tool processes every water supplier at once.
    }

    protected override void SetSecurityProperties()
    {
        // WaterSupplierId is set explicitly by the migration logic for every row.
    }

    protected override void SetSecurityProperties(object entity)
    {
    }
}
