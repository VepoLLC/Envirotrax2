using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Repositories.Implementations;

public class UserImpersonationRequestRepository : IUserImpersonationRequestRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserImpersonationRequestRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(int sourceUserId, int targetUserId)
    {
        var waterSupplierId = await _dbContext
            .WaterSuppliers
            .Where(supplier => supplier.Domain == WaterSupplier.EnvirotraxAdminDomain)
            .Select(supplier => supplier.Id)
            .SingleAsync();

        _dbContext.UserImpersonationRequests.Add(new UserImpersonationRequest
        {
            WaterSupplierId = waterSupplierId,
            SourceUserId = sourceUserId,
            TargetUserId = targetUserId,
            CreatedTime = DateTime.UtcNow
        });

        await _dbContext.SaveChangesAsync();
    }
}
