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

    public async Task ReplaceFromAsync(int targetWaterSupplierId, int sourceWaterSupplierId, CancellationToken cancellationToken)
    {
        var sourceRequirements = await GetAllByWaterSupplierIdAsync(sourceWaterSupplierId, cancellationToken);

        var currentRequirements = await Entity
            .Where(requirement => requirement.WaterSupplierId == targetWaterSupplierId)
            .ToListAsync(cancellationToken);

        var copies = sourceRequirements.Select(requirement => new BackflowRenewalRequirement
        {
            WaterSupplierId = targetWaterSupplierId,
            PropertyType = requirement.PropertyType,
            DeviceType = requirement.DeviceType,
            HazardType = requirement.HazardType,
            HasSiteOssf = requirement.HasSiteOssf,
            AuxWaterSupply = requirement.AuxWaterSupply,
            RenewalYears = requirement.RenewalYears
        });

        Entity.RemoveRange(currentRequirements);
        Entity.AddRange(copies);

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
