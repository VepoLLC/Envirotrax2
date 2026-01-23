
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/general-settings")]
public class GeneralSettingsController : CrudController<GeneralSettingsDto>
{
    public GeneralSettingsController(IGeneralSettingsService service)
        : base(service)
    {
    }
}
