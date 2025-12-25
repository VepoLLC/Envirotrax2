
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Repositories.Implementations;

public class ContractorUserRepository : IContractorUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ContractorUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ContractorUser?> GetAsync(int contractorId, int userId)
    {
        var contractorUser = await _dbContext
            .ContractorUsers
            .SingleOrDefaultAsync(s => s.ContractorId == contractorId && s.UserId == userId);

        if (contractorUser == null)
        {
            var parentId = await _dbContext
                    .Contractors
                    .Where(t => t.Id == contractorId)
                    .Select(t => t.ParentId)
                    .SingleOrDefaultAsync();

            if (parentId.HasValue)
            {
                contractorUser = await GetAsync(parentId.Value, userId);
            }
        }

        return contractorUser;
    }
}