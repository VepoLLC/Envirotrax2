
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Csi;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectorService
{
    Task<IPagedData<CsiInspectorAccountDto>> SearchAsync(PageInfo pageInfo, Query query, string? licenseNumber, string? insuranceNumber, CancellationToken cancellationToken);
}
