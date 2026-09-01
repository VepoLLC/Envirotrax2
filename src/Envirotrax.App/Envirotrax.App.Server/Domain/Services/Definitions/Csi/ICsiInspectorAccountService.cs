
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectorAccountService
{
    Task<IPagedData<CsiInspectorAccountDto>> SearchForAdminAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken);

    Task<CsiInspectorAccountDetailsDto?> GetDetailsForAdminAsync(int professionalId, int? userId, CancellationToken cancellationToken);

    Task<CsiInspectorAccountDetailsDto?> UpdateDetailsForAdminAsync(int professionalId, CsiInspectorAccountDetailsDto details, CancellationToken cancellationToken);
}
