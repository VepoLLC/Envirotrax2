

using Envirotrax.Admin.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.WaterSuppliers;

[Route("api/water-suppliers/{waterSupplierId}/users")]
public class WaterSupplierUserController : AdminBaseController
{
    private readonly IWaterSupplierUserService _waterSupplierUserService;

    public WaterSupplierUserController(IWaterSupplierUserService waterSupplierUserService)
    {
        _waterSupplierUserService = waterSupplierUserService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var users = await _waterSupplierUserService.GetAllAsync(waterSupplierId, cancellationToken);

        return Ok(users);
    }
}
