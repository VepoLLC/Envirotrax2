
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class WaterSupplierRepository : Repository<WaterSupplier>
{
    public WaterSupplierRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }
}