
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;

public interface IProfessionalUserLicenseRepository : IRepository<ProfessionalUserLicense>
{
    Task<IEnumerable<ProfessionalUserLicense>> GetAllAsync(int userId, PageInfo pageInfo, Query query);
}
