using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow
{
    public interface IBackflowTesterRepository : IRepository<Professional>
    {
        /// <summary>
        /// Searches backflow-testing companies. The row is the Professional (the company) - V2 has no master/sub
        /// professional accounts, so person-level criteria are applied as EXISTS sub-queries over the company's
        /// backflow-tester users rather than by rooting the search on ProfessionalUser. Same shape as
        /// <c>CsiInspectorRepository.SearchAsync</c>.
        /// </summary>
        Task<IEnumerable<Professional>> SearchAsync(BackflowTesterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    }
}
