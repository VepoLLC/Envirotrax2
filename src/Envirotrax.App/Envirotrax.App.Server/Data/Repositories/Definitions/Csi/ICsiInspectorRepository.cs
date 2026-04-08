using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi
{
    public interface ICsiInspectorRepository : IRepository<Professional>
    {
        Task<Professional?> GetWithStateAsync(int id, CancellationToken cancellationToken);
    }
}
