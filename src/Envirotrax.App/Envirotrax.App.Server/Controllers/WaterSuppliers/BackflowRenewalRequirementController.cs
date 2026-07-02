using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.WaterSuppliers;

[Route("api/backflow-renewal-requirements")]
public class BackflowRenewalRequirementController : WaterSupplierCrudController<BackflowRenewalRequirementDto>
{
    public BackflowRenewalRequirementController(IBackflowRenewalRequirementService service)
        : base(service)
    {
    }
}
