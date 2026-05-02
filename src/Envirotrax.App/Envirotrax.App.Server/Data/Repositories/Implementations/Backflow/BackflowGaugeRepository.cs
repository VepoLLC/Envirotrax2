
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowGaugeRepository : Repository<BackflowGauge>, IBackflowGaugeRepository
{
    public BackflowGaugeRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}
