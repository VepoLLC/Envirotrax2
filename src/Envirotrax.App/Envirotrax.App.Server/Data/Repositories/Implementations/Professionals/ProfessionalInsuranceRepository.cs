
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalInsuranceRepository : Repository<ProfessionalInsurance>, IProfessionalInsuranceRepository
{
    public ProfessionalInsuranceRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override void UpdateEntity(ProfessionalInsurance model)
    {
        base.UpdateEntity(model);

        // You cannot update the file path.
        DbContext.Entry(model).Property(i => i.FilePath).IsModified = false;
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

    public async Task<IEnumerable<ProfessionalInsurance>> GetUnverifiedByWaterSupplierAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var baseQuery = DbContext.Set<ProfessionalInsurance>()
            .AsNoTracking()
            .Where(i => DbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == i.ProfessionalId))
            .Where(i => i.ExpirationDate == null)
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

    public async Task<Dictionary<int, ProfessionalType?>> GetProfessionalTypesAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
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

            if (registration.HasBackflowTesting)
            {
                return ProfessionalType.Bpat;
            }

            if (registration.HasCsiInpection)
            {
                return ProfessionalType.CsiInspector;
            }

            if (registration.HasFogTransportation)
            {
                return ProfessionalType.FogTransporter;
            }

            if (registration.HasFogInspection)
            {
                return ProfessionalType.FogInspector;
            }

            return (ProfessionalType?)null;
        });
    }

    public async Task<IEnumerable<ProfessionalUser>> GetProfessionalUsersAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        return await DbContext.ProfessionalUsers
            .AsNoTracking()
            .Where(pu => professionalIds.Contains(pu.ProfessionalId))
            .ToListAsync(cancellationToken);
    }
}