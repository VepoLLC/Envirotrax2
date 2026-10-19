
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowTesterService
{
    Task<IPagedData<ProfessionalDto>> SearchAsync(BackflowTesterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);

    Task<ProfessionalDto?> GetAsync(int professionalId, CancellationToken cancellationToken);

    Task<ProfessionalDto?> UpdateAsync(int professionalId, ProfessionalDto professional, CancellationToken cancellationToken);
}
