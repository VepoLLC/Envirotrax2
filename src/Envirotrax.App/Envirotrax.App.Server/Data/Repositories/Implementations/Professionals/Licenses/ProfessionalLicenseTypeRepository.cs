
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals.Licenses;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Models.Professionals.Licenses;

public class ProfessionalLicenseTypeRepository : IProfessionalLicenseTypeRepository
{
    private readonly TenantDbContext _dbContext;

    public ProfessionalLicenseTypeRepository(IDbContextSelector dbContextSelector)
    {
        _dbContext = dbContextSelector.Current;
    }

    public async Task<IEnumerable<ProfessionalLicenseType>> GetAllAsync(Query query, CancellationToken cancellationToken)
    {
        return await _dbContext
            .ProfessionalLicenseTypes
            .Where(query.Filter)
            .ToListAsync(cancellationToken);
    }
}