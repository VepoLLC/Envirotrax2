using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalNoteRepository : Repository<ProfessionalNote>, IProfessionalNoteRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public ProfessionalNoteRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    // IgnoreQueryFilters: as in V1, a parent supplier also reads the notes its child suppliers wrote, and the
    // tenant filter would drop those. Scoping is restored explicitly to the current supplier and its children.
    public async Task<IEnumerable<ProfessionalNote>> GetAllByProfessionalAsync(int professionalId, ProfessionalType professionalType, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var waterSupplierId = _tenantProvider.WaterSupplierId;

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(ProfessionalNote.CreatedTime)] = SortOperator.Desc;
        }

        var paginated = await Entity
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Include(note => note.CreatedBy)
            .Where(note => note.ProfessionalId == professionalId
                && note.ProfessionalType == professionalType
                && (note.WaterSupplierId == waterSupplierId || note.WaterSupplier!.ParentId == waterSupplierId))
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<ProfessionalNote?> GetForProfessionalAsync(int professionalId, ProfessionalType professionalType, int noteId, CancellationToken cancellationToken)
    {
        return Entity
            .AsNoTracking()
            .SingleOrDefaultAsync(note => note.Id == noteId
                && note.ProfessionalId == professionalId
                && note.ProfessionalType == professionalType, cancellationToken);
    }
}
