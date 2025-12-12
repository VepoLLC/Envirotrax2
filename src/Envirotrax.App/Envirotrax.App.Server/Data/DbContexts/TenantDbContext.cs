
using Envirotrax.App.Server.Data.Models.Contractors;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.DbContexts;
using Envirotrax.Common.Data.Models;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.DbContexts;

public class TenantDbContext : TenantDbContextBase<WaterSupplier>
{
    public DbSet<WaterSupplierContractor> WaterSupplierContractors { get; set; }
    public DbSet<WaterSupplierUser> WaterSupplierUsers { get; set; }

    public DbSet<Contractor> Contractors { get; set; }
    public DbSet<ContractorUser> ContractorUsers { get; set; }

    protected TenantDbContext(
        DbContextOptions options,
        ILogger<TenantDbContextBase<WaterSupplier, AspNetUserBase>> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        ILogger<TenantDbContextBase<WaterSupplier, AspNetUserBase>> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }
}