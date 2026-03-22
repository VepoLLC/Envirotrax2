using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/csi-settings")]
public class CsiSettingsController : WaterSupplierCrudController<CsiSettingsDto>
{
    public CsiSettingsController(ICsiSettingsService service)
        : base(service)
    {
    }
}
