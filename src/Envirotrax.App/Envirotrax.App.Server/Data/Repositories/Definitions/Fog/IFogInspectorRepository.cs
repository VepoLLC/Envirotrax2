using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog
{
    public interface IFogInspectorRepository : IRepository<Professional>
    {
        /// <summary>
        /// Searches FOG inspection companies. The row is the Professional (the company) - V2 has no master/sub
        /// professional accounts, so person-level criteria are applied as EXISTS sub-queries over the company's
        /// FOG inspector users rather than by rooting the search on ProfessionalUser. Same shape as
        /// <c>BackflowTesterRepository.SearchAsync</c>.
        /// </summary>
        Task<IEnumerable<Professional>> SearchAsync(FogInspectorSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    }
}
