using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Backflow
{
    public interface IBackflowTesterService : IService<Professional, ProfessionalDto>
    {
        Task<IPagedData<ProfessionalDto>> SearchAsync(BackflowTesterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    }
}
