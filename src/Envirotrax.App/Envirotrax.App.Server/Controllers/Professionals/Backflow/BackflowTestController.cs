using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Backflow;

[Route("api/professionals/backflow/tests")]
[HasFeature(FeatureType.BackflowTesting)]
[Authorize(Roles = RoleDefinitions.Professionals.BackflowTester)]
public class BackflowTestController : ProfessionalProtectedController
{
    private readonly IBackflowTestService _backflowTestService;

    public BackflowTestController(IBackflowTestService backflowTestService)
    {
        _backflowTestService = backflowTestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PageInfo pageInfo, [FromQuery] Query query, CancellationToken cancellationToken)
    {
        var result = await _backflowTestService.GetAllAsync(pageInfo, query, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _backflowTestService.GetAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAsync(
        [FromForm] BackflowTestDto dto,
        [FromForm] IFormFile? assemblyImage,
        [FromForm] IFormFile? serialNumberImage,
        [FromForm] IFormFile? bypassAssemblyImage,
        [FromForm] IFormFile? bypassSerialNumberImage,
        [FromForm] IFormFile? airGapImage,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }
        dto.Id = 0;

        await using var assemblyStream = assemblyImage?.OpenReadStream();
        await using var serialStream = serialNumberImage?.OpenReadStream();
        await using var bypassAssemblyStream = bypassAssemblyImage?.OpenReadStream();
        await using var bypassSerialStream = bypassSerialNumberImage?.OpenReadStream();
        await using var airGapStream = airGapImage?.OpenReadStream();

        var result = await _backflowTestService.SubmitWithImagesAsync(
            dto,
            assemblyStream, assemblyImage?.FileName,
            serialStream, serialNumberImage?.FileName,
            bypassAssemblyStream, bypassAssemblyImage?.FileName,
            bypassSerialStream, bypassSerialNumberImage?.FileName,
            airGapStream, airGapImage?.FileName,
            cancellationToken);

        return Ok(result);
    }
}
