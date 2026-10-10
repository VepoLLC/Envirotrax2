using Envirotrax.App.Server.Data.Models.Sites;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Sites;

/// <summary>
/// Access for the anonymous renewal opt-in pages, where the site owner is signed out and no tenant
/// is set. Rows are looked up by their own ids instead of being scoped by a water supplier.
/// </summary>
public interface IRenewalOptInRepository
{
    Task<Site?> GetSiteAsync(int siteId, CancellationToken cancellationToken);

    Task<Site?> GetTrackedSiteAsync(int siteId, CancellationToken cancellationToken);

    Task<RenewalEmailVerification?> GetTrackedVerificationAsync(int verificationId, CancellationToken cancellationToken);

    void AddVerifications(IEnumerable<RenewalEmailVerification> verifications);

    Task SaveAsync(CancellationToken cancellationToken);
}
