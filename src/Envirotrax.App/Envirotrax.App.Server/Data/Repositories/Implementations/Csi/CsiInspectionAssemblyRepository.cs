using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Csi;

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

    public async Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetBySubmissionAsync(string submissionId, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .Where(assembly => assembly.SubmissionId == submissionId)
            .OrderBy(assembly => assembly.Id)
            .ToListAsync(cancellationToken);
    }

    // Only an unpaid row of this submission can be deleted. Its test goes with it only when this
    // submission created that test, so the site's real test history is never touched.
    public async Task<bool> DeleteForSubmissionAsync(int id, string submissionId, CancellationToken cancellationToken)
    {
        var assembly = await Entity
            .Include(assembly => assembly.Test)
            .SingleOrDefaultAsync(assembly => assembly.Id == id && assembly.SubmissionId == submissionId && (assembly.TransactionId == null || assembly.TransactionId == ""), cancellationToken);

        if (assembly == null)
        {
            return false;
        }

        Entity.Remove(assembly);

        if (assembly.Test != null && assembly.Test.SubmissionId == submissionId && string.IsNullOrEmpty(assembly.Test.TransactionId))
        {
            DbContext.BackflowTests.Remove(assembly.Test);
        }

        await DbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    public Task UpdateVisuallyIdentifiedAsync(string submissionId, IReadOnlyCollection<int> visuallyIdentifiedIds, CancellationToken cancellationToken)
    {
        return Entity
            .Where(assembly => assembly.SubmissionId == submissionId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(assembly => assembly.VisuallyIdentified, assembly => visuallyIdentifiedIds.Contains(assembly.Id)), cancellationToken);
    }

    public Task LinkToInspectionAsync(int inspectionId, string submissionId, CancellationToken cancellationToken)
    {
        return Entity
            .Where(assembly => assembly.SubmissionId == submissionId && assembly.InspectionId == null)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(assembly => assembly.InspectionId, inspectionId), cancellationToken);
    }

    public Task MarkPaidAsync(int inspectionId, string submissionId, string transactionId, CancellationToken cancellationToken)
    {
        return Entity
            .Where(assembly => assembly.InspectionId == inspectionId && assembly.SubmissionId == submissionId && (assembly.TransactionId == null || assembly.TransactionId == ""))
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(assembly => assembly.TransactionId, transactionId), cancellationToken);
    }

    public Task DeleteByInspectionAsync(int inspectionId, CancellationToken cancellationToken)
    {
        return Entity
            .Where(assembly => assembly.InspectionId == inspectionId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
