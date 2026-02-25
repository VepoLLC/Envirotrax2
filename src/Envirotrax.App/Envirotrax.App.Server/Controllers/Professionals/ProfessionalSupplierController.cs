
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Controllers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

[Route("api/professionals/water-suppliers")]
public class ProfessionalSupplierContoller : CrudController<ProfessionalWaterSupplierDto>
{
    private readonly IProfessionalSupplierService _proSupplierService;

    public ProfessionalSupplierContoller(IProfessionalSupplierService service) : base(service)
    {
        _proSupplierService = service;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAllAvailableAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var suppliers = await _proSupplierService.GetAllAvailableSuppliersAsync(pageInfo, query, cancellationToken);
        return Ok(suppliers);
    }
}