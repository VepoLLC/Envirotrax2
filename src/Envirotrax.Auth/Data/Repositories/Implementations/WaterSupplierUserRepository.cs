
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Repositories.Implementations;

public class WaterSupplierUserRepository : IWaterSupplierUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public WaterSupplierUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<WaterSupplierUser?> GetAsync(int supplierId, int userId)
    {
        var supplierUser = await _dbContext
            .WaterSupplierUsers
            .SingleOrDefaultAsync(s => s.WaterSupplierId == supplierId && s.UserId == userId);

        if (supplierUser == null)
        {
            var parentId = await _dbContext
                    .WaterSuppliers
                    .Where(t => t.Id == supplierId)
                    .Select(t => t.ParentId)
                    .SingleOrDefaultAsync();

            if (parentId.HasValue)
            {
                supplierUser = await GetAsync(parentId.Value, userId);
            }
        }

        return supplierUser;
    }
}