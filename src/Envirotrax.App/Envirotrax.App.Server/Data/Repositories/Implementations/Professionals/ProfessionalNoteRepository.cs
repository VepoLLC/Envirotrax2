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

public class ProfessionalNoteRepository : Repository<ProfessionalNote>, IProfessionalNoteRepository
{
    private readonly IAuthService _authService;

    // The Admin portal calls in with the AdminInternal scope; a super user signed in to the App carries the role.
    private bool IsAdmin => _authService.HasScope(ScopeDefinitions.AdminInternal)
        || _authService.HasAnyRole(RoleDefinitions.SuperUser);

    public ProfessionalNoteRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    // IgnoreQueryFilters: as in V1, a parent supplier reads, edits and deletes the notes its child and grandchild
    // suppliers wrote, and the tenant filter would drop those. Scoping is restored explicitly from the supplier the
    // user belongs to, which is the parent when a parent's user works inside a child supplier, plus the supplier
    // being worked in. Admins reach the notes of every water supplier.
    protected override IQueryable<ProfessionalNote> GetDetailsQuery()
    {
        var waterSupplierId = _authService.WaterSupplierId;
        var accountWaterSupplierId = _authService.ParentWaterSupplierId > 0 ? _authService.ParentWaterSupplierId : waterSupplierId;

        return Entity
            .IgnoreQueryFilters()
            .AsNoTracking()
            .WhereIf(!IsAdmin, note => note.WaterSupplierId == waterSupplierId
                || note.WaterSupplierId == accountWaterSupplierId
                || note.WaterSupplier!.ParentId == accountWaterSupplierId
                || note.WaterSupplier!.Parent!.ParentId == accountWaterSupplierId);
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

    // An admin note keeps the Vepo LLC supplier the service set, as in V1 where only admins saw it; the tenant
    // stamp would move it to the supplier a super user is working in.
    public override async Task<ProfessionalNote> AddAsync(ProfessionalNote note)
    {
        DbContext.SkipSaveSecurityProperties = IsAdmin;

        try
        {
            return await base.AddAsync(note);
        }
        finally
        {
            DbContext.SkipSaveSecurityProperties = false;
        }
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
