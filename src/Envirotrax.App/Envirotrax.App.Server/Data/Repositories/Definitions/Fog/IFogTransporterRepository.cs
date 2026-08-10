using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Fog
{
    public interface IFogTransporterRepository : IRepository<Professional>
    {
        Task<IEnumerable<Professional>> SearchAsync(string? registrationNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken);
    }
}
