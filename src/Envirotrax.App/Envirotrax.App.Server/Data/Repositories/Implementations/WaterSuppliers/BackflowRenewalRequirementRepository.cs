using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class BackflowRenewalRequirementRepository : Repository<BackflowRenewalRequirement>, IBackflowRenewalRequirementRepository
{
    public BackflowRenewalRequirementRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<BackflowRenewalRequirement>> GetAllByWaterSupplierIdAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<BackflowRenewalRequirement>()
            .IgnoreQueryFilters()
            .Where(r => r.WaterSupplierId == waterSupplierId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
