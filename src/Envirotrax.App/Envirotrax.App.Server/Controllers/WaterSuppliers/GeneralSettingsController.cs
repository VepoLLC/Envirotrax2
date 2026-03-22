
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/general-settings")]
[PermissionResource(PermissionType.WaterSuppliers)]
public class GeneralSettingsController : WaterSupplierCrudController<GeneralSettingsDto>
{
    public GeneralSettingsController(IGeneralSettingsService service)
        : base(service)
    {
    }
}
