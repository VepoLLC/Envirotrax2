using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Envirotrax.App.Server.Data.DbContexts
{
    public class ProfessionalDbContext : TenantDbContext
    {
        private readonly ITenantProvidersService _tenantProvider;

        public ProfessionalDbContext(
            DbContextOptions<ProfessionalDbContext> options,
            ILogger<ProfessionalDbContext> logger,
            ITenantProvidersService tenantProvider)
            : base(options, logger, tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        protected override void SetupGlobalFiltering(ModelBuilder builder, IMutableEntityType entity)
        {
            // ISharedProfessionalModel rows are visible to every professional, so they get no query
            // filter here; SetSecurityProperties still restricts who can write to them.
            if (typeof(ISharedProfessionalModel).IsAssignableFrom(entity.ClrType))
            {
                return;
            }

            if (typeof(IProfessionalModel).IsAssignableFrom(entity.ClrType))
            {
                Expression<Func<IProfessionalModel, bool>> expression = model => model.ProfessionalId == _tenantProvider.ProfessionalId;
                var lambdaExpression = ConvertFilterExpression(expression, entity.ClrType);

                builder.Entity(entity.ClrType).HasQueryFilter(lambdaExpression);
            }
        }

        protected override void SetSecurityProperties()
        {
            if (!SkipSaveSecurityProperties)
            {
                var professionalId = _tenantProvider.ProfessionalId;

                foreach (var entry in ChangeTracker.Entries<IProfessionalModel>())
                {
                    if (entry.Entity is ISharedProfessionalModel)
                    {
                        SetSharedProfessionalSecurityProperties(entry, professionalId);
                        continue;
                    }

                    var professionalProperty = entry.Property(e => e.ProfessionalId);
                    professionalProperty.CurrentValue = professionalId;
                }
            }
        }

        // A shared row's ProfessionalId is its owner, not just its tenant, so new rows get stamped the
        // same as any IProfessionalModel, but an existing row can only be changed or deleted by the
        // professional recorded as its owner — otherwise this would let anyone editing a shared row
        // silently reassign it to themselves.
        private static void SetSharedProfessionalSecurityProperties(EntityEntry<IProfessionalModel> entry, int professionalId)
        {
            var professionalProperty = entry.Property(e => e.ProfessionalId);

            if (entry.State == EntityState.Added)
            {
                professionalProperty.CurrentValue = professionalId;

                return;
            }

            if (entry.State is EntityState.Modified or EntityState.Deleted && professionalProperty.OriginalValue != professionalId)
            {
                throw new ValidationException("Access denied.");
            }
        }

        protected override void SetSecurityProperties(object entity)
        {
            if (!SkipSaveSecurityProperties)
            {
                // Ownership for a shared row is enforced in the ChangeTracker-based overload above,
                // once the entity's real state (Added vs Modified/Deleted) is known; Attach/Entry run
                // before that, so stamping here would just overwrite the real owner with whoever attached it.
                if (entity is ISharedProfessionalModel)
                {
                    return;
                }

                if (entity is IProfessionalModel professionalModel)
                {
                    professionalModel.ProfessionalId = _tenantProvider.ProfessionalId;
                }
            }
        }
    }
}