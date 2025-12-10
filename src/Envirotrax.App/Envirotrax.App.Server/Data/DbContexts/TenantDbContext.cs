
using Envirotrax.App.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.DbContexts;
using Envirotrax.Common.Data.Models;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.DbContexts;

public class TenantDbContext : TenantDbContextBase<WaterSupplier>
{
    public TenantDbContext(
        DbContextOptions options,
        ILogger<TenantDbContextBase<WaterSupplier, AspNetUserBase>> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }
}