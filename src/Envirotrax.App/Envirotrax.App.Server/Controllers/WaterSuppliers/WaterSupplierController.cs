
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/water-suppliers")]
public class WaterSupplierController : CrudController<WaterSupplierDto>
{
    public WaterSupplierController(IWaterSupplierService service)
        : base(service)
    {
    }
}