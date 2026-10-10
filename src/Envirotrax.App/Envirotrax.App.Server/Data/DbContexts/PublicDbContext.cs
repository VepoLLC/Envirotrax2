
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Envirotrax.App.Server.Data.DbContexts;

/// <summary>
/// Context for anonymous, cross-tenant pages such as the public Registered Professionals
/// directory and the renewal opt-in pages. There is no ambient tenant on those requests — the
/// caller is signed out, or is a professional whose own company must not scope what the directory
/// shows — so every tenant filter is switched off here and scoping comes from the explicit water
/// supplier or record id the query is given.
///
/// This exists so those queries do not have to repeat <c>IgnoreQueryFilters()</c> on every call,
/// which is easy to forget and silently returns the wrong rows when missed.
///
/// Tenant ids are never stamped here, so anything added through this context must set its
/// WaterSupplierId explicitly.
/// </summary>
public class PublicDbContext : TenantDbContext
{
    protected PublicDbContext(
        DbContextOptions options,
        ILogger<PublicDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    public PublicDbContext(
        DbContextOptions<PublicDbContext> options,
        ILogger<PublicDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
    }

    protected override void SetupGlobalFiltering(ModelBuilder builder, IMutableEntityType entity)
    {
        // No tenant filtering. Public pages read across every water supplier and scope themselves
        // by the water supplier id they are given.
    }

    protected override void SetSecurityProperties()
    {
        // No ambient tenant to stamp. Writes set their tenant ids explicitly.
    }

    protected override void SetSecurityProperties(object entity)
    {
        // No ambient tenant to stamp. Writes set their tenant ids explicitly.
    }
}
