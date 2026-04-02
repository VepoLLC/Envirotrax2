using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi
{
    public class CsiInspectorRepository : Repository<Professional>, ICsiInspectorRepository
    {
        public CsiInspectorRepository(IDbContextSelector dbContextSelector) 
            : base(dbContextSelector)
        {
        }
    }
}
