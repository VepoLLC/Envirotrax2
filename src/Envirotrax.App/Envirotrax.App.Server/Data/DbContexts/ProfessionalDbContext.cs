using System.Linq.Expressions;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;
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
            if (typeof(IProfessionalModel).IsAssignableFrom(entity.ClrType))
            {
                Expression<Func<IProfessionalModel, bool>> expression = model => model.ProfessionalId == _tenantProvider.ProfessionalId;
                var lambdaExpression = ConvertFilterExpression(expression, entity.ClrType);

                builder.Entity(entity.ClrType).HasQueryFilter(lambdaExpression);
            }
        }

        protected override void SetSecurityProperties()
        {
            base.SetSecurityProperties();

            if (!SkipSaveSecurityProperties)
            {
                var professionalId = _tenantProvider.ProfessionalId;

                foreach (var entry in ChangeTracker.Entries<IProfessionalModel>())
                {
                    var professionalProperty = entry.Property(e => e.ProfessionalId);
                    professionalProperty.CurrentValue = professionalId;
                }
            }
        }

        protected override void SetSecurityProperties(object entity)
        {
            base.SetSecurityProperties(entity);

            if (!SkipSaveSecurityProperties)
            {
                if (entity is IProfessionalModel professionalModel)
                {
                    professionalModel.ProfessionalId = _tenantProvider.ProfessionalId;
                }
            }
        }
    }
}