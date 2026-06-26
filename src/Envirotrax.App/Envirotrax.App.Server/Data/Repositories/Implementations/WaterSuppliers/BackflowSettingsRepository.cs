using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class BackflowSettingsRepository : Repository<BackflowSettings>, IBackflowSettingsRepository
{
    public BackflowSettingsRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<BackflowTestingSettingsDto?> GetTestingSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        return await Entity
            .IgnoreQueryFilters()
            .Where(s => s.WaterSupplierId == waterSupplierId)
            .Select(s => new BackflowTestingSettingsDto
            {
                ShowRainSensor = s.ShowRainSensor,
                ShowOSSF = s.ShowOSSF,
                ShowPermitNumber = s.ShowPermitNumber
            })
            .SingleOrDefaultAsync(cancellationToken);
    }
}
