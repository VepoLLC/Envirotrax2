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
}
