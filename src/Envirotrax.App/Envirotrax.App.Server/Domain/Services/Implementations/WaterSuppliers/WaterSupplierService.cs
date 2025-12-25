
using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class WaterSupplierService : Service<WaterSupplier, WaterSupplierDto>, IWaterSupplierService
{
    public WaterSupplierService(IMapper mapper, IWaterSupplierRepository repository)
        : base(mapper, repository)
    {
    }
}