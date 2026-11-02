
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Fog;

public interface IFogTransporterService
{
    Task<IPagedData<ProfessionalDto>> SearchAsync(FogTransporterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalDto?> GetAsync(int professionalId, CancellationToken cancellationToken);

    Task<ProfessionalDto?> UpdateAsync(int professionalId, ProfessionalDto professional, CancellationToken cancellationToken);
}
