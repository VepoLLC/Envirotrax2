using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalNoteRepository : ProfessionalNoteRepositoryBase<ProfessionalNote>, IProfessionalNoteRepository
{
    public ProfessionalNoteRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector, authService)
    {
    }

    public async Task<IEnumerable<ProfessionalNote>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(ProfessionalNote.CreatedTime)] = SortOperator.Desc;
        }

        var paginated = await GetDetailsQuery()
            .Include(note => note.CreatedBy)
            .Where(note => note.ProfessionalId == professionalId && note.ProfessionalType == professionalType)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<ProfessionalNote?> GetForProfessionalAsync(int professionalId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken)
    {
        return GetDetailsQuery()
            .SingleOrDefaultAsync(note => note.Id == noteId
                && note.ProfessionalId == professionalId
                && note.ProfessionalType == professionalType, cancellationToken);
    }
}
