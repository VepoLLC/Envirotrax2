
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;
using Envirotrax.App.Server.Domain.Services.Definitions.PublicSearch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.PublicSearch;

[AllowAnonymous]
[Route("api/public-search")]
public class PublicSearchController : EnvirotraxBaseController
{
    private readonly IPublicSearchService _publicSearchService;

    public PublicSearchController(IPublicSearchService publicSearchService)
    {
        _publicSearchService = publicSearchService;
    }

    [HttpGet("water-suppliers")]
    public async Task<IActionResult> GetWaterSuppliersAsync(
        [FromQuery] string? domain,
        CancellationToken cancellationToken)
    {
        var suppliers = await _publicSearchService.GetWaterSuppliersAsync(domain, cancellationToken);

        return Ok(suppliers);
    }

    [HttpGet("backflow-tests")]
    public async Task<IActionResult> SearchBackflowTestsAsync(
        [FromQuery] PublicSearchCriteriaDto criteria,
        [FromQuery] PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var results = await _publicSearchService.SearchBackflowTestsAsync(criteria, pageInfo, cancellationToken);

        return Ok(results);
    }

    [HttpGet("csi-inspections")]
    public async Task<IActionResult> SearchCsiInspectionsAsync(
        [FromQuery] PublicSearchCriteriaDto criteria,
        [FromQuery] PageInfo pageInfo,
        CancellationToken cancellationToken)
    {
        var results = await _publicSearchService.SearchCsiInspectionsAsync(criteria, pageInfo, cancellationToken);

        return Ok(results);
    }

    [HttpGet("backflow-tests/{id}")]
    public async Task<IActionResult> GetBackflowTestAsync(int id, CancellationToken cancellationToken)
    {
        var test = await _publicSearchService.GetBackflowTestAsync(id, cancellationToken);

        return Ok(test);
    }

    [HttpGet("backflow-tests/{id}/pdf")]
    public async Task<IActionResult> GetBackflowTestPdfAsync(int id, CancellationToken cancellationToken)
    {
        var pdf = await _publicSearchService.GetBackflowTestPdfAsync(id, cancellationToken);

        if (pdf == null)
        {
            return NoContent();
        }

        return File(pdf, "application/pdf");
    }

    [HttpGet("csi-inspections/{id}")]
    public async Task<IActionResult> GetCsiInspectionAsync(int id, CancellationToken cancellationToken)
    {
        var inspection = await _publicSearchService.GetCsiInspectionAsync(id, cancellationToken);

        return Ok(inspection);
    }

    [HttpGet("csi-inspections/{id}/assemblies")]
    public async Task<IActionResult> GetCsiInspectionAssembliesAsync(int id, CancellationToken cancellationToken)
    {
        var assemblies = await _publicSearchService.GetCsiInspectionAssembliesAsync(id, cancellationToken);

        return Ok(assemblies);
    }

    [HttpGet("csi-inspections/{id}/images")]
    public async Task<IActionResult> GetCsiInspectionImagesAsync(int id, CancellationToken cancellationToken)
    {
        var images = await _publicSearchService.GetCsiInspectionImagesAsync(id, cancellationToken);

        return Ok(images);
    }

    [HttpGet("csi-inspections/{id}/pdf")]
    public async Task<IActionResult> GetCsiInspectionPdfAsync(int id, CancellationToken cancellationToken)
    {
        var pdf = await _publicSearchService.GetCsiInspectionPdfAsync(id, cancellationToken);

        if (pdf == null)
        {
            return NoContent();
        }

        return File(pdf, "application/pdf");
    }
}
