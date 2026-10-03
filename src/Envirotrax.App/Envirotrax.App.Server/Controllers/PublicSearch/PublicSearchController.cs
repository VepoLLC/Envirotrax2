
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
}
