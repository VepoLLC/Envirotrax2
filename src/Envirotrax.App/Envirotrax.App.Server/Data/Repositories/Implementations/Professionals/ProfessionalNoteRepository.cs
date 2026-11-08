using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
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

    // IgnoreQueryFilters: as in V1, a parent supplier reads, edits and deletes the notes its child and grandchild
    // suppliers wrote, and the tenant filter would drop those. Scoping is restored explicitly to the current supplier
    // and its descendants. Admins reach the notes of every water supplier.
    protected override IQueryable<ProfessionalNote> GetDetailsQuery()
    {
        var waterSupplierId = _tenantProvider.WaterSupplierId;
        var isAdmin = _tenantProvider.HasScope(ScopeDefinitions.AdminInternal);

        return Entity
            .IgnoreQueryFilters()
            .AsNoTracking()
            .WhereIf(!isAdmin, note => note.WaterSupplierId == waterSupplierId
                || note.WaterSupplier!.ParentId == waterSupplierId
                || note.WaterSupplier!.Parent!.ParentId == waterSupplierId);
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

    // The tenant stamp would move a child supplier's note to the parent editing it, and WaterSupplierId is part
    // of the key. The note was loaded through GetDetailsQuery, so its WaterSupplierId is already the right one.
    public override async Task<ProfessionalNote?> UpdateAsync(ProfessionalNote note)
    {
        DbContext.SkipSaveSecurityProperties = true;

        try
        {
            return await base.UpdateAsync(note);
        }
        finally
        {
            DbContext.SkipSaveSecurityProperties = false;
        }
    }

    public override async Task<ProfessionalNote?> DeleteAsync(int id)
    {
        DbContext.SkipSaveSecurityProperties = true;

        try
        {
            return await base.DeleteAsync(id);
        }
        finally
        {
            DbContext.SkipSaveSecurityProperties = false;
        }
    }
}
