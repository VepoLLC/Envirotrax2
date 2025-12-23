
using Envirotrax.Auth.Data.Models;

namespace Envirotrax.Auth.Data.Repositories.Defintions;

public interface IContractorUserRepository
{
    Task<ContractorUser?> GetAsync(int contractorId, int userId);
}