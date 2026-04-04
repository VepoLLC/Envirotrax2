using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.DbContexts;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using System.Reflection;

namespace Envirotrax.App.Server.Data.DbContexts;

public class TenantDbContext : TenantDbContextBase<WaterSupplier, AppUser>
{
    public DbSet<GeneralSettings> GeneralSettings { get; set; }
    public DbSet<CsiSettings> CsiSettings { get; set; }

    public DbSet<WaterSupplierUser> WaterSupplierUsers { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<Professional> Professionals { get; set; }
    public DbSet<ProfessionalUser> ProfessionalUsers { get; set; }
    public DbSet<ProfessionalWaterSupplier> ProfessionalWaterSuppliers { get; set; }
    public DbSet<ProfessionalLicenseType> ProfessionalLicenseTypes { get; set; }
    public DbSet<ProfessionalUserLicense> ProfessionalUserLicenses { get; set; }
    public DbSet<ProfessionalInsurance> ProfessionalInsurances { get; set; }

    public DbSet<State> States { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<CsiInspection> CsiInspections { get; set; }

    protected TenantDbContext(
        DbContextOptions options,
        ILogger<TenantDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        ILogger<TenantDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}