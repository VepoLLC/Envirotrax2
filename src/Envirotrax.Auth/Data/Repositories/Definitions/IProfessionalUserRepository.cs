
using Envirotrax.Auth.Data.Models;

namespace Envirotrax.Auth.Data.Repositories.Defintions;

public interface IProfessionalUserRepository
{
    Task<ProfessionalUser?> GetAsync(int professionalId, int userId);
}