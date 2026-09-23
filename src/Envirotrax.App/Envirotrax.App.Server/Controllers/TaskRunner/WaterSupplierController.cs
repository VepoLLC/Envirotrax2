using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.TaskRunner;

[Route("api/task-runner/water-suppliers")]
public class WaterSupplierController : TaskRunnerBaseContoller
{
    private readonly IWaterSupplierService _waterSupplierService;

    public WaterSupplierController(IWaterSupplierService waterSupplierService)
    {
        _waterSupplierService = waterSupplierService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSupplierIdsAsync([FromQuery] bool hasBackflowTests, CancellationToken cancellationToken)
    {
        var ids = await _waterSupplierService.GetSupplierIdsAsync(hasBackflowTests, cancellationToken);
        return Ok(ids);
    }
}
