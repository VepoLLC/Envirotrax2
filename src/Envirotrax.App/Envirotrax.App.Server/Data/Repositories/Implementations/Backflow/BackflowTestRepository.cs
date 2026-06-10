using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowTestRepository : Repository<BackflowTest>, IBackflowTestRepository
{
    public BackflowTestRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<BackflowTest> GetListQuery()
    {
        return base.GetListQuery()
            .Include(bt => bt.WaterSupplier)
            .Include(bt => bt.Site)
            .Include(bt => bt.Bpat)
            .Include(bt => bt.BpatState)
            .Include(bt => bt.PropertyState)
            .Include(bt => bt.MailingState);
    }

    protected override IQueryable<BackflowTest> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(bt => bt.WaterSupplier)
            .Include(bt => bt.Site)
            .Include(bt => bt.Bpat)
            .Include(bt => bt.BpatState)
            .Include(bt => bt.PropertyState)
            .Include(bt => bt.MailingState);
    }

    public override Task<IEnumerable<BackflowTest>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowTest.Id)] = SortOperator.Asc;
        }
        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    // Image paths are owned by the dedicated image flow (UpdateImagePathAsync),
    // so a regular update must never overwrite them.
    protected override void UpdateEntity(BackflowTest model)
    {
        base.UpdateEntity(model);

        var entry = DbContext.Entry(model);
        entry.Property(m => m.AssemblyImagePath).IsModified = false;
        entry.Property(m => m.SerialNumberImagePath).IsModified = false;
        entry.Property(m => m.BypassAssemblyImagePath).IsModified = false;
        entry.Property(m => m.BypassSerialNumberImagePath).IsModified = false;
        entry.Property(m => m.AirGapImagePath).IsModified = false;
    }

    public async Task<BackflowTest> UpdateImagePathAsync(BackflowTest model, string imagePathPropertyName)
    {
        DbContext.Attach(model);
        DbContext.Entry(model).Property(imagePathPropertyName).IsModified = true;

        await DbContext.SaveChangesAsync();

        return model;
    }
}
