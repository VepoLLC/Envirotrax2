using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> SubmitAsync([FromBody] BackflowTestDto dto)
    {
        dto.Id = 0;
        var result = await _backflowTestService.AddAsync(dto);
        return Ok(result);
    }
}
