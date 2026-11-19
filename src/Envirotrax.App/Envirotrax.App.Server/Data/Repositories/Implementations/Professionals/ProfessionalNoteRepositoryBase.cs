using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Data.Models;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

// Shared by the company comment log (ProfessionalNotes) and the user comment log (ProfessionalUserNotes),
// which follow the same water supplier rules.
public abstract class ProfessionalNoteRepositoryBase<TNote> : Repository<TNote>
    where TNote : TenantModel<WaterSupplier>
{
    private readonly IAuthService _authService;

    // The Admin portal calls in with the AdminInternal scope; a super user signed in to the App carries the role.
    private bool IsAdmin => _authService.HasScope(ScopeDefinitions.AdminInternal)
        || _authService.HasAnyRole(RoleDefinitions.SuperUser);

    protected ProfessionalNoteRepositoryBase(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    // IgnoreQueryFilters: as in V1, a parent supplier reads, edits and deletes the notes its child and grandchild
    // suppliers wrote, and the tenant filter would drop those. Scoping is restored explicitly from the supplier the
    // user belongs to, which is the parent when a parent's user works inside a child supplier, plus the supplier
    // being worked in. Admins reach the notes of every water supplier.
    protected override IQueryable<TNote> GetDetailsQuery()
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

    // An admin note keeps the Vepo LLC supplier the service set, as in V1 where only admins saw it; the tenant
    // stamp would move it to the supplier a super user is working in.
    public override async Task<TNote> AddAsync(TNote note)
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
    public override async Task<TNote?> UpdateAsync(TNote note)
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

    public override async Task<TNote?> DeleteAsync(int id)
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
