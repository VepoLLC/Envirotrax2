using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Sites;

public interface IRenewalOptInService
{
    Task<RenewalOptInSiteDto> GetSiteAsync(int siteId, CancellationToken cancellationToken);

    Task<RenewalOptInResultDto> SaveAsync(int siteId, RenewalOptInRequestDto request, CancellationToken cancellationToken);

    Task<RenewalOptInResultDto> VerifyEmailAsync(RenewalOptInTokenDto request, CancellationToken cancellationToken);

    Task<RenewalOptInResultDto> UnsubscribeAsync(RenewalOptInTokenDto request, CancellationToken cancellationToken);
}
