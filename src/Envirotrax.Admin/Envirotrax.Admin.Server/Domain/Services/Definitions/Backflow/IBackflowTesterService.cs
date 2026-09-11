
using DeveloperPartners.SortingFiltering;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.Admin.Server.Domain.Services.Definitions.Backflow;

public interface IBackflowTesterService
{
    Task<IPagedData<ProfessionalDto>> SearchAsync(PageInfo pageInfo, Query query, IDictionary<string, string> criteria, CancellationToken cancellationToken);
}
