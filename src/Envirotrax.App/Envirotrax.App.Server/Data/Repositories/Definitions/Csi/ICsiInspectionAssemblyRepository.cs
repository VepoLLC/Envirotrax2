using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionAssemblyRepository : IRepository<CsiInspectionVisuallyIdentifiedAssembly>
{
    Task<List<CsiInspectionVisuallyIdentifiedAssembly>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
}
