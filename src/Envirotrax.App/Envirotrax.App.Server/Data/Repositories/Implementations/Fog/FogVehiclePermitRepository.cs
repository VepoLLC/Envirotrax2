using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogVehiclePermitRepository : Repository<FogVehiclePermit>, IFogVehiclePermitRepository
{
    public FogVehiclePermitRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<FogVehicle>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogVehicle.Id)] = SortOperator.Asc;
        }

        var paginated = await GetSearchQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<FogVehicle?> GetSearchResultByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken)
    {
        return GetSearchQuery()
            .SingleOrDefaultAsync(vehicle => vehicle.Id == vehicleId, cancellationToken);
    }

    public Task<bool> HasVehicleInScopeAsync(int vehicleId, CancellationToken cancellationToken)
    {
        return GetSearchQuery()
            .AnyAsync(vehicle => vehicle.Id == vehicleId, cancellationToken);
    }

    public async Task<FogVehiclePermit> SetPermitAsync(FogVehiclePermit permit, CancellationToken cancellationToken)
    {
        var existing = await Entity
            .SingleOrDefaultAsync(p => p.VehicleId == permit.VehicleId, cancellationToken);

        if (existing == null)
        {
            Entity.Add(permit);
            await DbContext.SaveChangesAsync(cancellationToken);

            return permit;
        }

        existing.PermitNumber = permit.PermitNumber;
        existing.InspectionDueDate = permit.InspectionDueDate;
        existing.IsActive = permit.IsActive;

        await DbContext.SaveChangesAsync(cancellationToken);

        return existing;
    }

    private IQueryable<int> GetRegisteredTransporterIdsQuery()
    {
        return DbContext.ProfessionalWaterSuppliers
            .Where(pws => pws.HasFogTransportation && !pws.IsBanned)
            .Select(pws => pws.ProfessionalId);
    }

    private IQueryable<FogVehicle> GetSearchQuery()
    {
        var registeredTransporterIds = GetRegisteredTransporterIdsQuery();

        return DbContext.FogVehicles
            .AsNoTracking()
            .Include(vehicle => vehicle.Professional!)
                .ThenInclude(professional => professional.State)
            .Include(vehicle => vehicle.Permit)
            .Where(vehicle => vehicle.DeletedTime == null
                && vehicle.Professional!.DeletedTime == null
                && registeredTransporterIds.Contains(vehicle.ProfessionalId));
    }
}
