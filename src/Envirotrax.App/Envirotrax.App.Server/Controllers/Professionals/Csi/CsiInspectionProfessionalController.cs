using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Csi;

[Route("api/professionals/csi/inspections")]
[HasFeature(FeatureType.CsiInspection)]
[Authorize(Roles = $"{RoleDefinitions.Professionals.Admin},{RoleDefinitions.Professionals.CsiInspector}")]
public class CsiInspectionProfessionalController : ProfessionalProtectedController
{
    private readonly ICsiInspectionService _inspectionService;
    private readonly ICsiCheckoutService _checkoutService;
    private readonly ICsiInspectionAssemblyService _assemblyService;

    public CsiInspectionProfessionalController(
        ICsiInspectionService inspectionService,
        ICsiCheckoutService checkoutService,
        ICsiInspectionAssemblyService assemblyService)
    {
        _inspectionService = inspectionService;
        _checkoutService = checkoutService;
        _assemblyService = assemblyService;
    }

    [HttpGet("insurance-check")]
    public async Task<IActionResult> GetInsuranceCheckAsync([FromQuery] int waterSupplierId, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.GetInsuranceCheckAsync(waterSupplierId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.GetForProfessionalAsync(id, cancellationToken);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpGet("{id}/pdf")]
    public async Task<IActionResult> GetPdfAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _inspectionService.GetForProfessionalAsync(id, cancellationToken);
        if (inspection == null)
        {
            return NotFound();
        }

        var pdf = await _inspectionService.GeneratePdfForProfessionalAsync(inspection);
        return File(pdf, "application/pdf");
    }

    // The inspection's saved "Assemblies at This Location" rows, for the read-only view. The form uses
    // CsiInspectionAssemblyProfessionalController instead, which also lists the site's unlisted tests.
    [HttpGet("{id}/assemblies")]
    public async Task<IActionResult> GetAssembliesAsync(int id, CancellationToken cancellationToken)
    {
        var assemblies = await _assemblyService.GetByInspectionAsync(id, cancellationToken);
        return Ok(assemblies);
    }

    [HttpGet]
    public async Task<IActionResult> GetProfessionalInspectionsAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, [FromQuery] bool latestOnly, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.SearchForProfessionalAsync(pageInfo, query, latestOnly, cancellationToken);
        return Ok(result);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitAsync([FromBody] CreateCsiInspectionDto request, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.SubmitAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] CreateCsiInspectionDto request, CancellationToken cancellationToken)
    {
        var result = await _inspectionService.UpdateForProfessionalAsync(id, request, cancellationToken);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await _inspectionService.DeleteAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> CheckoutAsync([FromBody] ProfessionalCheckoutRequestDto request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var receipt = await _checkoutService.CheckoutAsync(request, cancellationToken);
        return Ok(receipt);
    }
}
