using System.Linq.Expressions;
using Envirotrax.App.Server.Data.Models.Contractors;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.DbContexts
{
    public class ContractorDbContext : TenantDbContext
    {
        private readonly ITenantProvidersService _tenantProvider;

        public ContractorDbContext(
            DbContextOptions<ContractorDbContext> options,
            ILogger<ContractorDbContext> logger,
            ITenantProvidersService tenantProvider)
            : base(options, logger, tenantProvider)
        {
            _tenantProvider = tenantProvider;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IContractorModel).IsAssignableFrom(entity.ClrType))
                {
                    Expression<Func<IContractorModel, bool>> expression = model => model.ContractorId == _tenantProvider.ContractorId;
                    var lambdaExpression = ConvertFilterExpression(expression, entity.ClrType);

                    modelBuilder.Entity(entity.ClrType).HasQueryFilter("ContractorFilter", lambdaExpression);
                }
            }
        }
    }
}