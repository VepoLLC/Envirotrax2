
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/water-suppliers")]
public class WaterSupplierController : AdminBaseController
{
    private readonly IWaterSupplierService _waterSupplierService;
    private readonly IUserService _userService;

    public WaterSupplierController(
        IWaterSupplierService waterSupplierService,
        IUserService userService)
    {
        _waterSupplierService = waterSupplierService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var suppliers = await _waterSupplierService.GetAllAsync(pageInfo, query, cancellationToken);
        return Ok(suppliers);
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetDetailsAsync(int id, CancellationToken cancellationToken)
    {
        var details = await _waterSupplierService.GetDetailsAsync(id, cancellationToken);

        if (details == null)
        {
            return NotFound();
        }

        return Ok(details);
    }

    [HttpGet("{id}/user-accounts")]
    public async Task<IActionResult> GetUserAccountsAsync(int id, CancellationToken cancellationToken)
    {
        var accounts = await _userService.GetAccountsForWaterSupplierAsync(id, cancellationToken);

        return Ok(accounts);
    }

    [HttpPut("{id}/details")]
    public async Task<IActionResult> UpdateDetailsAsync(int id, WaterSupplierDetailsDto details)
    {
        var updated = await _waterSupplierService.UpdateDetailsAsync(id, details);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }
}
