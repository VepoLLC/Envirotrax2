using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi
{
    public interface ICsiInspectorRepository : IRepository<Professional>
    {
        Task<IEnumerable<Professional>> SearchAsync(string? inspectorLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken);
    }
}
