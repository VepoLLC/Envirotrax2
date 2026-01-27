
using Envirotrax.Auth.Data.Models;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.Auth.Data.Repositories.Implementations;

public class ProfessionalUserRepository : IProfessionalUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ProfessionalUserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProfessionalUser?> GetAsync(int professionalId, int userId)
    {
        var professionalUser = await _dbContext
            .ProfessionalUsers
            .SingleOrDefaultAsync(s => s.ProfessionalId == professionalId && s.UserId == userId);

        if (professionalUser == null)
        {
            var parentId = await _dbContext
                    .Professionals
                    .Where(t => t.Id == professionalId)
                    .Select(t => t.ParentId)
                    .SingleOrDefaultAsync();

            if (parentId.HasValue)
            {
                professionalUser = await GetAsync(parentId.Value, userId);
            }
        }

        return professionalUser;
    }
}