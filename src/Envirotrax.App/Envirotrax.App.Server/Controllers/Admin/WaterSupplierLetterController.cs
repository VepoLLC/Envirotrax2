using Envirotrax.App.Server.Domain.Services.Definitions.Letters;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Admin;

[Route("api/admin/water-suppliers/{waterSupplierId}/letters")]
public class WaterSupplierLetterController : AdminBaseController
{
    private readonly ILetterGeneratorService _letterGeneratorService;

    public WaterSupplierLetterController(ILetterGeneratorService letterGeneratorService)
    {
        _letterGeneratorService = letterGeneratorService;
    }

    [HttpPost]
    public async Task<IActionResult> GenerateAsync(int waterSupplierId)
    {
        var letters = await _letterGeneratorService.GenerateAsync(waterSupplierId);

        if (letters == null)
        {
            return NotFound();
        }

        return Ok(letters);
    }
}
