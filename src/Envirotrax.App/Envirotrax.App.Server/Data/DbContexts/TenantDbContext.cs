using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.DbContexts;
using Envirotrax.Common.Data.Models;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.DbContexts;

public class TenantDbContext : TenantDbContextBase<WaterSupplier, AppUser>
{
    public DbSet<WaterSupplierProfessional> WaterSupplierProfessionals { get; set; }
    public DbSet<WaterSupplierUser> WaterSupplierUsers { get; set; }
    public DbSet<GeneralSettings> GeneralSettings { get; set; }

    public DbSet<Professional> Professionals { get; set; }
    public DbSet<ProfessionalUser> ProfessionalUsers { get; set; }
    public DbSet<State> States { get; set; }
    public DbSet<Site> Sites { get; set; }

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
}