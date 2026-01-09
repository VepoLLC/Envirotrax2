using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.States;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.States
{
    public class StateRepository : Repository<State>, IStateRepository
    {

        private readonly ITenantProvidersService _tenantProvider;

        public StateRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
            : base(dbContextSelector)
        {
            _tenantProvider = tenantProvider;
        }

        protected override IQueryable<State> GetListQuery()
        {
            return base.GetListQuery()
                .AsNoTracking();
        }
    }
}
