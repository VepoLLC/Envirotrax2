using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.Services.Definitions.Users;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/water-suppliers/{waterSupplierId}/users")]
public class WaterSupplierUserController : AdminBaseController
{
    private readonly IUserService _userService;

    public WaterSupplierUserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync(int waterSupplierId, [FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var users = await _userService.GetAllForWaterSupplierAsync(waterSupplierId, pageInfo, query, cancellationToken);

        return Ok(users);
    }
}
