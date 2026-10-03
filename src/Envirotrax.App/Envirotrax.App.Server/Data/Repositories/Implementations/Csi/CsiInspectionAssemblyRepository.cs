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

    // Makes the inspection's rows match `assemblies` in one save: rows with an Id keep their snapshot
    // and only take the new VisuallyIdentified flag, rows without one are inserted (together with a
    // new Test when one is attached), and saved rows missing from the list are removed. A removed
    // row's test goes with it only when this submission created it and it is still unpaid — the V1
    // rule, which leaves the site's real test history alone.
    public async Task SaveForInspectionAsync(
        int inspectionId,
        string? submissionId,
        IReadOnlyCollection<CsiInspectionVisuallyIdentifiedAssembly> assemblies,
        CancellationToken cancellationToken)
    {
        var saved = await Entity
            .Include(assembly => assembly.Test)
            .Where(assembly => assembly.InspectionId == inspectionId)
            .ToListAsync(cancellationToken);

        var keptById = assemblies
            .Where(assembly => assembly.Id > 0)
            .ToDictionary(assembly => assembly.Id);

        foreach (var assembly in saved)
        {
            if (keptById.TryGetValue(assembly.Id, out var kept))
            {
                assembly.VisuallyIdentified = kept.VisuallyIdentified;
                continue;
            }

            Entity.Remove(assembly);

            var createdBySubmission = assembly.Test != null
                && !string.IsNullOrEmpty(submissionId)
                && assembly.Test.SubmissionId == submissionId
                && string.IsNullOrEmpty(assembly.Test.TransactionId);

            if (createdBySubmission)
            {
                DbContext.BackflowTests.Remove(assembly.Test!);
            }
        }

        Entity.AddRange(assemblies.Where(assembly => assembly.Id == 0));

        await DbContext.SaveChangesAsync(cancellationToken);
    }
}
