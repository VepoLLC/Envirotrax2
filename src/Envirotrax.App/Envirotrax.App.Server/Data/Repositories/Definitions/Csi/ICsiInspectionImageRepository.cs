using Envirotrax.App.Server.Data.Models.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionImageRepository : IRepository<CsiInspectionImage>
{
    Task<List<CsiInspectionImage>> GetByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
    Task<int> GetCountByInspectionAsync(int inspectionId, CancellationToken cancellationToken);
}
