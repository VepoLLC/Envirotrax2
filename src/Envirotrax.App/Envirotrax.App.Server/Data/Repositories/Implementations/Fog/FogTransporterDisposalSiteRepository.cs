using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogTransporterDisposalSiteRepository : Repository<FogTransporterDisposalSite>, IFogTransporterDisposalSiteRepository
{
    public FogTransporterDisposalSiteRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<FogTransporterDisposalSite> GetListQuery()
    {
        return base.GetListQuery().Include(d => d.DisposalSite);
    }

    public override Task<IEnumerable<FogTransporterDisposalSite>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogTransporterDisposalSite.Id)] = SortOperator.Asc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    public async Task<IEnumerable<FogDisposalSiteCandidate>> GetAvailableAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            // V1 sorts the disposal-site master list by County.
            query.Sort[nameof(FogDisposalSiteCandidate.County)] = SortOperator.Asc;
        }

        // Master list of disposal facilities. There is no automatic soft-delete query filter,
        // so exclude soft-deleted rows explicitly (replaces V1's "Active = 1"). Registration state
        // comes from correlated subqueries against this professional's registrations (Entity is
        // auto-scoped by ProfessionalDbContext) so each site yields exactly ONE row.
        var candidates = from site in DbContext.FogDisposalSites.AsNoTracking()
                         where site.DeletedTime == null
                         select new FogDisposalSiteCandidate
                         {
                             Id = site.Id,
                             Name = site.Name,
                             RegistrationNumber = site.RegistrationNumber,
                             County = site.County,
                             PhysicalType = site.PhysicalType,
                             IsActive = Entity.Any(r => r.DisposalSiteId == site.Id && r.IsActive)
                         };

        var paginated = await candidates
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<FogDisposalSiteCandidate> SetRegistrationAsync(int disposalSiteId, bool isActive, CancellationToken cancellationToken)
    {
        // Entity is scoped to the current professional by ProfessionalDbContext's global filter.
        var existing = await Entity
            .Where(r => r.DisposalSiteId == disposalSiteId)
            .ToListAsync(cancellationToken);

        if (isActive)
        {
            // Idempotent: only create when there is no active registration yet.
            // ProfessionalId is set automatically on save by ProfessionalDbContext.
            if (!existing.Any(r => r.IsActive))
            {
                Entity.Add(new FogTransporterDisposalSite
                {
                    DisposalSiteId = disposalSiteId,
                    IsActive = true
                });
                await DbContext.SaveChangesAsync(cancellationToken);
            }
        }
        else if (existing.Count > 0)
        {
            // Unselect: remove every registration row for this site (cleans up duplicates).
            DbContext.RemoveRange(existing);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        var site = await DbContext.FogDisposalSites
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == disposalSiteId, cancellationToken);

        var hasActiveRegistration = await Entity
            .AnyAsync(r => r.DisposalSiteId == disposalSiteId && r.IsActive, cancellationToken);

        return new FogDisposalSiteCandidate
        {
            Id = disposalSiteId,
            Name = site?.Name ?? string.Empty,
            RegistrationNumber = site?.RegistrationNumber ?? string.Empty,
            County = site?.County ?? string.Empty,
            PhysicalType = site?.PhysicalType ?? PhysicalType.Other,
            IsActive = hasActiveRegistration
        };
    }
}
