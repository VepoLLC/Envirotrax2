
using Envirotrax.Auth.Data.Models;

namespace Envirotrax.Auth.Data.Repositories.Defintions;

public interface IWaterSupplierUserRepository
{
    Task<WaterSupplierUser?> GetAsync(int supplierId, int userId);
}