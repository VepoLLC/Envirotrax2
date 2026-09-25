
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowGaugeRepository : Repository<BackflowGauge>, IBackflowGaugeRepository
{
    public BackflowGaugeRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<BackflowGauge>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await DbContext.BackflowGauges
            .AsNoTracking()
            .Where(g => g.ProfessionalId == professionalId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<BackflowGauge?> UpdateGaugeAsync(BackflowGauge model)
    {
        var gauge = await GetTrackedForUpdateAsync(model.Id, default);

        if (gauge == null)
        {
            return null;
        }

        gauge.Manufacturer = model.Manufacturer;
        gauge.Model = model.Model;
        gauge.SerialNumber = model.SerialNumber;
        gauge.LastCalibrationDate = model.LastCalibrationDate;
        gauge.IsPortable = model.IsPortable;
        gauge.FilePath = model.FilePath;

        await SaveChangesAsync(logData: true);

        return gauge;
    }

    private IQueryable<BackflowGauge> ScopedToWaterSupplier()
    {
        return DbContext.BackflowGauges
            .AsNoTracking()
            .Where(g => DbContext.ProfessionalWaterSuppliers.Any(pws =>
                pws.ProfessionalId == g.ProfessionalId && pws.HasBackflowTesting));
    }

    public async Task<IEnumerable<BackflowGauge>> GetUnverifiedByWaterSupplierAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var baseQuery = ScopedToWaterSupplier()
            .Where(g => g.LastCalibrationDate == null && !g.IsManaged)
            .Include(g => g.Professional)
            .Include(g => g.CreatedBy);

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowGauge.Id)] = SortOperator.Asc;
        }

        var paginated = await baseQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnverifiedCountByWaterSupplierAsync(CancellationToken cancellationToken)
    {
        return await ScopedToWaterSupplier()
            .CountAsync(g => g.LastCalibrationDate == null && !g.IsManaged, cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalUser>> GetProfessionalUsersAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        return await DbContext.ProfessionalUsers
            .AsNoTracking()
            .Include(pu => pu.User)
            .Where(pu => professionalIds.Contains(pu.ProfessionalId))
            .ToListAsync(cancellationToken);
    }
}
