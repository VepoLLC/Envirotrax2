
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Backflow;
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

    public async Task<UpdateResult<BackflowGauge>> UpdateGaugeAsync(BackflowGauge model)
    {
        var result = new UpdateResult<BackflowGauge>();

        var gauge = await GetTrackedForUpdateAsync(model.Id, default);

        if (gauge == null)
        {
            return result;
        }

        gauge.Manufacturer = model.Manufacturer;
        gauge.Model = model.Model;
        gauge.SerialNumber = model.SerialNumber;
        gauge.LastCalibrationDate = model.LastCalibrationDate;
        gauge.IsPortable = model.IsPortable;
        gauge.FilePath = model.FilePath;

        result.Changes = BuildChangeDescription(gauge);

        await DbContext.SaveChangesAsync();

        result.Model = gauge;

        return result;
    }
}
