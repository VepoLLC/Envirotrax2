using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public class RenewalOptInRepository : Repository<Site, int, PublicDbContext>, IRenewalOptInRepository
{
    public RenewalOptInRepository(PublicDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<Site?> GetSiteAsync(int siteId, CancellationToken cancellationToken)
    {
        return await DbContext.Sites
            .AsNoTracking()
            .FirstOrDefaultAsync(site => site.Id == siteId && site.DeletedTime == null, cancellationToken);
    }

    public async Task<Site?> GetTrackedSiteAsync(int siteId, CancellationToken cancellationToken)
    {
        return await DbContext.Sites
            .FirstOrDefaultAsync(site => site.Id == siteId && site.DeletedTime == null, cancellationToken);
    }

    public async Task<RenewalEmailVerification?> GetTrackedVerificationAsync(int verificationId, CancellationToken cancellationToken)
    {
        return await DbContext.RenewalEmailVerifications
            .Include(verification => verification.Site)
            .FirstOrDefaultAsync(verification => verification.Id == verificationId, cancellationToken);
    }

    public void AddVerifications(IEnumerable<RenewalEmailVerification> verifications)
    {
        DbContext.RenewalEmailVerifications.AddRange(verifications);
    }

    public async Task SaveAsync(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(logData: true, cancellationToken);
    }
}
