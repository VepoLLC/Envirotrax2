using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.DbContexts;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using System.Reflection;
using Envirotrax.App.Server.Data.Models.WaterSuppliers.Features;
using Envirotrax.App.Server.Data.Models.GisAreas;

namespace Envirotrax.App.Server.Data.DbContexts;

public class TenantDbContext : TenantDbContextBase<WaterSupplier, AppUser>
{
    public DbSet<GeneralSettings> GeneralSettings { get; set; }
    public DbSet<CsiSettings> CsiSettings { get; set; }
    public DbSet<BackflowSettings> BackflowSettings { get; set; }
    public DbSet<BackflowRenewalRequirement> BackflowRenewalRequirements { get; set; }

    public DbSet<WaterSupplierUser> WaterSupplierUsers { get; set; }
    public DbSet<Feature> Features { get; set; }
    public DbSet<WaterSupplierFeature> WaterSupplierFeatures { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }

    public DbSet<GisArea> GisAreas { get; set; }
    public DbSet<GisAreaCoordinate> GisAreaCoordinates { get; set; }

    public DbSet<Professional> Professionals { get; set; }
    public DbSet<ProfessionalUser> ProfessionalUsers { get; set; }
    public DbSet<ProfessionalWaterSupplier> ProfessionalWaterSuppliers { get; set; }
    public DbSet<ProfessionalLicenseType> ProfessionalLicenseTypes { get; set; }
    public DbSet<ProfessionalUserLicense> ProfessionalUserLicenses { get; set; }
    public DbSet<ProfessionalInsurance> ProfessionalInsurances { get; set; }
    public DbSet<BackflowGauge> BackflowGauges { get; set; }
    public DbSet<FogTripTicket> FogTripTickets { get; set; }
    public DbSet<FogVehicle> FogVehicles { get; set; }
    public DbSet<FogVehiclePermit> FogVehiclePermits { get; set; }
    public DbSet<FogTransporterDisposalSite> FogTransporterDisposalSites { get; set; }
    public DbSet<FogDisposalSite> FogDisposalSites { get; set; }

    public DbSet<State> States { get; set; }
    public DbSet<Site> Sites { get; set; }
    public DbSet<CsiInspection> CsiInspections { get; set; }
    public DbSet<CsiInspectionImage> CsiInspectionImages { get; set; }
    public DbSet<CsiInspectionVisuallyIdentifiedAssembly> CsiInspectionVisuallyIdentifiedAssemblies { get; set; }
    public DbSet<RecordLog> RecordLogs { get; set; }
    public DbSet<FogInspection> FogInspections { get; set; }
    public DbSet<BackflowTest> BackflowTests { get; set; }
    public DbSet<BackflowOutOfServiceRequest> BackflowOutOfServiceRequests { get; set; }
    public DbSet<BackflowComplianceSnapshot> BackflowComplianceSnapshots { get; set; }
    public DbSet<SiteLog> SiteLogs { get; set; }

    // Site is not an ITenantModel, so the base class does not filter or stamp it. The tenant
    // provider is held here so this context can apply that behavior to Site itself.
    private readonly ITenantProvidersService _tenantProvider;

    protected TenantDbContext(
        DbContextOptions options,
        ILogger<TenantDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public TenantDbContext(
        DbContextOptions<TenantDbContext> options,
        ILogger<TenantDbContext> logger,
        ITenantProvidersService tenantProvider)
        : base(options, logger, tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Applies the WaterSupplier tenant filter to Site, which the base class skips
    /// because Site does not implement ITenantModel.
    ///
    /// This MUST stay inside SetupGlobalFiltering: AdminDbContext and ProfessionalDbContext
    /// suppress tenant filtering by overriding this method without calling base. Moving it to
    /// OnModelCreating would silently apply the Site filter to those contexts too.
    /// </summary>
    protected override void SetupGlobalFiltering(ModelBuilder builder, IMutableEntityType entity)
    {
        base.SetupGlobalFiltering(builder, entity);

        if (entity.ClrType == typeof(Site))
        {
            builder.Entity<Site>()
                .HasQueryFilter(site => site.WaterSupplierId == _tenantProvider.WaterSupplierId);
        }
    }

    /// <summary>
    /// Stamps the tenant onto tracked Sites before saving, mirroring what the base class does for
    /// ITenantModel entities. Without this the client-supplied WaterSupplierId would be persisted.
    /// </summary>
    protected override void SetSecurityProperties()
    {
        base.SetSecurityProperties();

        if (!SkipSaveSecurityProperties)
        {
            foreach (var entry in ChangeTracker.Entries<Site>())
            {
                var tenantIdProperty = entry.Property(site => site.WaterSupplierId);
                tenantIdProperty.CurrentValue = VerifySaveTenantId(tenantIdProperty.CurrentValue);
            }
        }
    }

    /// <summary>
    /// Stamps the tenant onto a Site as soon as it is attached or entered, mirroring the base
    /// class behavior for ITenantModel entities.
    /// </summary>
    protected override void SetSecurityProperties(object entity)
    {
        base.SetSecurityProperties(entity);

        if (!SkipSaveSecurityProperties && entity is Site site)
        {
            site.WaterSupplierId = VerifySaveTenantId(_tenantProvider.WaterSupplierId);
        }
    }
}