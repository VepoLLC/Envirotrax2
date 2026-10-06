using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi;

// The rows are an IProfessionalModel, so in the professional context every query and ExecuteUpdate/ExecuteDelete
// below touches only the logged-in professional's rows.
public class CsiInspectionAssemblyRepository : Repository<CsiInspectionVisuallyIdentifiedAssembly>, ICsiInspectionAssemblyRepository
{
    public CsiInspectionAssemblyRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override IQueryable<CsiInspectionVisuallyIdentifiedAssembly> GetListQuery()
    {
        return base.GetListQuery()
            .Include(assembly => assembly.Test);
    }

    public async Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .Where(assembly => assembly.InspectionId == inspectionId)
            .OrderBy(assembly => assembly.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        return await base.GetListQuery()
            .CountAsync(assembly => assembly.InspectionId == inspectionId, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<CsiInspectionVisuallyIdentifiedAssembly> assemblies, CancellationToken cancellationToken)
    {
        Entity.AddRange(assemblies);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    // ExecuteUpdateAsync bypasses SaveChanges, so ISharedProfessionalModel's ownership check never runs
    // for it — the ProfessionalId check below is this method's only protection against touching another
    // professional's assemblies.
    public Task UpdateVisuallyIdentifiedAsync(int inspectionId, int professionalId, IReadOnlyCollection<int> visuallyIdentifiedIds, CancellationToken cancellationToken)
    {
        return Entity
            .Where(assembly => assembly.InspectionId == inspectionId && assembly.ProfessionalId == professionalId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(assembly => assembly.VisuallyIdentified, assembly => visuallyIdentifiedIds.Contains(assembly.Id)), cancellationToken);
    }

    // See UpdateVisuallyIdentifiedAsync: ExecuteUpdateAsync needs its own ProfessionalId check.
    public Task MarkPaidAsync(int inspectionId, int professionalId, string transactionId, CancellationToken cancellationToken)
    {
        return Entity
            .Where(assembly => assembly.InspectionId == inspectionId && assembly.ProfessionalId == professionalId && (assembly.TransactionId == null || assembly.TransactionId == ""))
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(assembly => assembly.TransactionId, transactionId), cancellationToken);
    }

    // V1 csi_inspection_submit_worker: a removed row takes its test with it only when this inspection added
    // that test and it is unpaid, so the site's real test history is never touched.
    public async Task DeleteFromInspectionAsync(int inspectionId, IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        var assemblies = await Entity
            .Include(assembly => assembly.Test)
            .Where(assembly => assembly.InspectionId == inspectionId && ids.Contains(assembly.Id))
            .ToListAsync(cancellationToken);

        foreach (var assembly in assemblies.Where(assembly => assembly.AddedOnInspection && assembly.Test != null))
        {
            if (string.IsNullOrEmpty(assembly.Test!.TransactionId))
            {
                DbContext.BackflowTests.Remove(assembly.Test);
            }
        }

        Entity.RemoveRange(assemblies);

        await DbContext.SaveChangesAsync(cancellationToken);
    }

    // V1 checkout delete of an unpaid inspection: its rows go, the tests it added stay. See
    // UpdateVisuallyIdentifiedAsync: ExecuteDeleteAsync needs its own ProfessionalId check.
    public Task DeleteByInspectionAsync(int inspectionId, int professionalId, CancellationToken cancellationToken)
    {
        return Entity
            .Where(assembly => assembly.InspectionId == inspectionId && assembly.ProfessionalId == professionalId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
