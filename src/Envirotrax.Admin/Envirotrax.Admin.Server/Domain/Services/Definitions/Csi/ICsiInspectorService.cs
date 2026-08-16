
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Csi;

public interface ICsiInspectorService
{
    Task<IPagedData<ProfessionalDto>> SearchAsync(PageInfo pageInfo, Query query, string? inspectorLicenseNumber, string? insurancePolicyNumber, string? userEmail, string? contactName, CancellationToken cancellationToken);
}
