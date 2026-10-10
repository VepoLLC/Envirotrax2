using Envirotrax.App.Server.Configuration;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.Services.Definitions.Sites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Envirotrax.App.Server.Controllers.Sites;

[AllowAnonymous]
[Route("api/renewal-opt-in")]
public class RenewalOptInController : EnvirotraxBaseController
{
    private readonly IRenewalOptInService _renewalOptInService;

    public RenewalOptInController(IRenewalOptInService renewalOptInService)
    {
        _renewalOptInService = renewalOptInService;
    }

    [HttpGet("sites/{siteId}")]
    public async Task<IActionResult> GetSiteAsync(int siteId, CancellationToken cancellationToken)
    {
        var site = await _renewalOptInService.GetSiteAsync(siteId, cancellationToken);

        return Ok(site);
    }

    [HttpPost("sites/{siteId}")]
    [EnableRateLimiting(RateLimitPolicies.AnonymousCodeCheck)]
    public async Task<IActionResult> SaveAsync(int siteId, [FromBody] RenewalOptInRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _renewalOptInService.SaveAsync(siteId, request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("verify-email")]
    [EnableRateLimiting(RateLimitPolicies.AnonymousCodeCheck)]
    public async Task<IActionResult> VerifyEmailAsync([FromBody] RenewalOptInTokenDto request, CancellationToken cancellationToken)
    {
        var result = await _renewalOptInService.VerifyEmailAsync(request, cancellationToken);

        return Ok(result);
    }

    [HttpPost("unsubscribe")]
    [EnableRateLimiting(RateLimitPolicies.AnonymousCodeCheck)]
    public async Task<IActionResult> UnsubscribeAsync([FromBody] RenewalOptInTokenDto request, CancellationToken cancellationToken)
    {
        var result = await _renewalOptInService.UnsubscribeAsync(request, cancellationToken);

        return Ok(result);
    }
}
