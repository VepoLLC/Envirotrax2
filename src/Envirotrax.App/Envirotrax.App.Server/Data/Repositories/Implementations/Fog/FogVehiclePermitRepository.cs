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

    public async Task<IEnumerable<FogVehiclePermitSearchResult>> SearchAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(FogVehiclePermitSearchResult.VehicleId)] = SortOperator.Asc;
        }

        var paginated = await GetSearchQuery()
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<FogVehiclePermitSearchResult?> GetSearchResultByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken)
    {
        return GetSearchQuery()
            .SingleOrDefaultAsync(r => r.VehicleId == vehicleId, cancellationToken);
    }

    public Task<bool> HasVehicleInScopeAsync(int vehicleId, CancellationToken cancellationToken)
    {
        return GetSearchQuery()
            .AnyAsync(r => r.VehicleId == vehicleId, cancellationToken);
    }

    public async Task<FogVehiclePermit> SetPermitAsync(FogVehiclePermit permit, CancellationToken cancellationToken)
    {
        var existing = await Entity
            .SingleOrDefaultAsync(p => p.VehicleId == permit.VehicleId && p.DeletedTime == null, cancellationToken);

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

    private IQueryable<FogVehiclePermitSearchResult> GetSearchQuery()
    {
        var registeredTransporterIds = GetRegisteredTransporterIdsQuery();

        var query = from vehicle in DbContext.FogVehicles
                    join transporter in DbContext.Professionals on vehicle.ProfessionalId equals transporter.Id
                    join permitRow in DbContext.FogVehiclePermits.Where(p => p.DeletedTime == null)
                        on vehicle.Id equals permitRow.VehicleId into permitJoin
                    from permit in permitJoin.DefaultIfEmpty()
                    where vehicle.DeletedTime == null
                        && transporter.DeletedTime == null
                        && (registeredTransporterIds.Contains(vehicle.ProfessionalId) || permit != null)
                    select new FogVehiclePermitSearchResult
                    {
                        VehicleId = vehicle.Id,

                        TransporterId = transporter.Id,
                        TransporterCompanyName = transporter.Name,
                        TransporterAddress = transporter.Address,
                        TransporterCity = transporter.City,
                        TransporterState = transporter.State != null ? transporter.State.Code : null,
                        TransporterZip = transporter.ZipCode,
                        TransporterPhoneNumber = transporter.PhoneNumber,
                        TransporterFaxNumber = transporter.FaxNumber,
                        TransporterEmailAddress = transporter.CompanyEmail,

                        LicensePlateNumber = vehicle.LicensePlateNumber,
                        Manufacturer = vehicle.Manufacturer,
                        ManufacturedYear = vehicle.ManufacturedYear,
                        Capacity = vehicle.Capacity,
                        CapacityType = vehicle.CapacityType,
                        StickerNumber = vehicle.StickerNumber,

                        HasPermit = permit != null,
                        PermitNumber = permit != null ? permit.PermitNumber : null,
                        InspectionDueDate = permit != null ? permit.InspectionDueDate : null,
                        IsActive = permit != null ? (bool?)permit.IsActive : null
                    };

        return query;
    }
}
