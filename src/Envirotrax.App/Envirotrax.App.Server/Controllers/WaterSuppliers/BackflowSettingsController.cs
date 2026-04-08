using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/backflow-settings")]
public class BackflowSettingsController : WaterSupplierCrudController<BackflowSettingsDto>
{
    public BackflowSettingsController(IBackflowSettingsService service)
        : base(service)
    {
    }
}
