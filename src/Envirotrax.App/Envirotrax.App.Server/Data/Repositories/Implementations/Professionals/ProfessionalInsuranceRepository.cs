
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalInsuranceRepository : Repository<ProfessionalInsurance>, IProfessionalInsuranceRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public ProfessionalInsuranceRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void UpdateEntity(ProfessionalInsurance model)
    {
        base.UpdateEntity(model);

        // You cannot update the file path.
        DbContext.Entry(model).Property(i => i.FilePath).IsModified = false;

        // Only the V1 migration sets IsManaged (V1's Manage button), so a save must not reset it.
        DbContext.Entry(model).Property(i => i.IsManaged).IsModified = false;
    }

    public async Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => i.ProfessionalId == professionalId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForProfessionalAsync(int professionalId, int id, CancellationToken cancellationToken)
    {
        return Entity.AnyAsync(i => i.Id == id && i.ProfessionalId == professionalId, cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalInsurance>> GetAllByProfessionalIdsAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        return await DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => professionalIds.Contains(i.ProfessionalId))
            .ToListAsync(cancellationToken);
    }

    public async Task<ProfessionalInsurance?> GetCurrentForProfessionalAsync(int professionalId, CancellationToken cancellationToken)
    {
        return await DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => i.ProfessionalId == professionalId)
            .OrderByDescending(i => i.ExpirationDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<ProfessionalInsurance?> GetForProfessionalAsync(int professionalId, int insuranceId, CancellationToken cancellationToken)
    {
        return DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == insuranceId && i.ProfessionalId == professionalId, cancellationToken);
    }

    // V1 rule: a policy waits for verification until it gets an expiration date, unless V1's Manage button dismissed it.
    // A water supplier sees the policies of professionals registered with it; an admin sees every policy
    // (AdminDbContext has no tenant filters, so the registration check would otherwise mean "with any supplier").
    private IQueryable<ProfessionalInsurance> GetUnverifiedQuery()
    {
        return DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => i.ExpirationDate == null && !i.IsManaged)
            .WhereIf(!_tenantProvider.HasScope(ScopeDefinitions.AdminInternal), i => DbContext.ProfessionalWaterSuppliers.Any(pws =>
                pws.ProfessionalId == i.ProfessionalId));
    }

    public async Task<IEnumerable<ProfessionalInsurance>> GetUnverifiedAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var baseQuery = GetUnverifiedQuery()
            .Include(i => i.Professional)
            .Include(i => i.CreatedBy);

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(ProfessionalInsurance.Id)] = SortOperator.Asc;
        }

        var paginated = await baseQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<int> CountUnverifiedAsync(CancellationToken cancellationToken)
    {
        return GetUnverifiedQuery().CountAsync(cancellationToken);
    }

    public async Task<Dictionary<int, ProfessionalType?>> GetProfessionalTypesAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        // An admin works across all suppliers, so the company's own services decide. The registrations can't be used
        // there: without tenant filters a company registered with several suppliers has one row per supplier.
        if (_tenantProvider.HasScope(ScopeDefinitions.AdminInternal))
        {
            var professionals = await DbContext.Professionals
                .AsNoTracking()
                .Where(p => professionalIds.Contains(p.Id))
                .Select(p => new { p.Id, p.HasBackflowTesting, p.HasCsiInspection, p.HasFogTransportation, p.HasFogInspection })
                .ToListAsync(cancellationToken);

            return professionals.ToDictionary(p => p.Id, p =>
                GetProfessionalType(p.HasBackflowTesting, p.HasCsiInspection, p.HasFogTransportation, p.HasFogInspection));
        }

        var registrations = await DbContext.ProfessionalWaterSuppliers
            .AsNoTracking()
            .Where(pws => professionalIds.Contains(pws.ProfessionalId))
            .ToDictionaryAsync(pws => pws.ProfessionalId, pws => pws, cancellationToken);

        return professionalIds.Distinct().ToDictionary(id => id, id =>
        {
            if (!registrations.TryGetValue(id, out var registration))
            {
                return (ProfessionalType?)null;
            }

            return GetProfessionalType(registration.HasBackflowTesting, registration.HasCsiInpection,
                registration.HasFogTransportation, registration.HasFogInspection);
        });
    }

    private static ProfessionalType? GetProfessionalType(bool hasBackflowTesting, bool hasCsiInspection, bool hasFogTransportation, bool hasFogInspection)
    {
        if (hasBackflowTesting)
        {
            return ProfessionalType.Bpat;
        }

        if (hasCsiInspection)
        {
            return ProfessionalType.CsiInspector;
        }

        if (hasFogTransportation)
        {
            return ProfessionalType.FogTransporter;
        }

        if (hasFogInspection)
        {
            return ProfessionalType.FogInspector;
        }

        return null;
    }

    public async Task<IEnumerable<ProfessionalUser>> GetProfessionalUsersAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        return await DbContext.ProfessionalUsers
            .AsNoTracking()
            .Where(pu => professionalIds.Contains(pu.ProfessionalId))
            .ToListAsync(cancellationToken);
    }
}