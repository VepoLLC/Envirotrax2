using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class BackflowRenewalRequirementRepository : Repository<BackflowRenewalRequirement>, IBackflowRenewalRequirementRepository
{
    public BackflowRenewalRequirementRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}
