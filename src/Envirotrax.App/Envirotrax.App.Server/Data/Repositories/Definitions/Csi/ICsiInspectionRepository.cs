using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Csi;

public interface ICsiInspectionRepository : IRepository<CsiInspection>
{
    Task<CsiInspection?> GetForProfessionalAsync(int id, int professionalId, CancellationToken cancellationToken);
    Task<IEnumerable<CsiInspection>> SearchForProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CsiInspectionProfessionalSearchRequest request, CancellationToken cancellationToken);
}
