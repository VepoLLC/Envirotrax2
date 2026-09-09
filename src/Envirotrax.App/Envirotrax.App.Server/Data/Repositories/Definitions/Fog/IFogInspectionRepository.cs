using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogInspectionRepository : IRepository<FogInspection>
{
    Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query,
        bool latestOnly, CancellationToken cancellationToken);

    Task<UpdateResult<FogInspection>> UpdateForProfessionalAsync(
        FogInspection model,
        int professionalId,
        string? newExteriorImagePath,
        string? newInteriorImagePath,
        string? newSignatureImagePath);
}
