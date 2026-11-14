
namespace Envirotrax.Auth.Data.Repositories.Defintions;

public interface IUserImpersonationRequestRepository
{
    Task AddAsync(int sourceUserId, int targetUserId);
}
