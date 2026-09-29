using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog;

public interface IFogInspectionRepository : IRepository<FogInspection>
{
    Task<IEnumerable<FogInspection>> SearchForProfessionalAsync(
        PageInfo pageInfo, Query query,
        bool latestOnly, CancellationToken cancellationToken);

    Task<FogInspection?> UpdateForProfessionalAsync(
        FogInspection model,
        int professionalId,
        string? newExteriorImagePath,
        string? newInteriorImagePath,
        string? newSignatureImagePath);

    Task<FogInspection?> UpdateForAdminAsync(int id, FogInspectionAdminUpdateRequest request);

    Task<FogInspection?> UpdateImagePathAsync(int id, string imagePathPropertyName, string newPath);

    Task<int> CountBySiteAsync(int siteId, CancellationToken cancellationToken);
}
