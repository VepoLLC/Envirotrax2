using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectionService : IService<CsiInspection, CsiInspectionDto>
{
    Task<CsiInspectionDto?> GetForProfessionalAsync(int id, CancellationToken cancellationToken);
    Task<CsiInspectionDto> SubmitAsync(CsiInspectionDto request, CancellationToken cancellationToken);
    Task<IPagedData<CsiInspectionDto>> SearchForProfessionalAsync(PageInfo pageInfo, Query query, CsiInspectionProfessionalSearchRequest request, CancellationToken cancellationToken);
}
