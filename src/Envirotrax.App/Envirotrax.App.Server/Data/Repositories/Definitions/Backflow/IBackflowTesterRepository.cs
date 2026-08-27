using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Backflow
{
    public interface IBackflowTesterRepository : IRepository<Professional>
    {
        Task<IEnumerable<Professional>> SearchAsync(string? bpatLicenseNumber, string? fireLicenseNumber, string? insurancePolicyNumber, PageInfo pageInfo, CancellationToken cancellationToken);
    }
}
