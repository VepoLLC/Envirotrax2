using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog
{
    public interface IFogTransporterRepository : IRepository<Professional>
    {
        Task<IEnumerable<Professional>> SearchAsync(string? registrationNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken);

        /// <summary>
        /// Searches FOG transportation companies. The row is the Professional (the company) - V2 has no master/sub
        /// professional accounts, so person-level criteria are applied as EXISTS sub-queries over the company's
        /// FOG transporter users. Same shape as <c>FogInspectorRepository.SearchAsync</c>.
        /// </summary>
        Task<IEnumerable<Professional>> SearchForAdminAsync(FogTransporterSearchDto criteria, PageInfo pageInfo, Query query, CancellationToken cancellationToken);
    }
}
