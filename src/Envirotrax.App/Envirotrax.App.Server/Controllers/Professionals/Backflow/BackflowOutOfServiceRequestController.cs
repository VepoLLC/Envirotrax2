using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;
using Envirotrax.App.Server.Filters;
using Envirotrax.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.App.Server.Controllers.Professionals.Backflow;

[Route("api/professionals/backflow/out-of-service-requests")]
[HasFeature(FeatureType.BackflowTesting)]
[Authorize(Roles = RoleDefinitions.Professionals.BackflowTester)]
public class BackflowOutOfServiceRequestController : ProfessionalProtectedController
{
    private readonly IBackflowOutOfServiceRequestService _service;

    public BackflowOutOfServiceRequestController(IBackflowOutOfServiceRequestService service)
    {
        _service = service;
    }

    [HttpGet("replacement-candidates")]
    public async Task<IActionResult> GetReplacementCandidatesAsync([FromQuery] int testId, CancellationToken cancellationToken)
    {
        var result = await _service.GetReplacementCandidatesAsync(testId, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitAsync([FromBody] BackflowOutOfServiceRequestDto dto, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var result = await _service.SubmitAsync(dto, cancellationToken);
        return Ok(result);
    }
}
