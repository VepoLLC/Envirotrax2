
using System.Runtime.CompilerServices;
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalUserRepository : Repository<ProfessionalUser>, IProfessionalUserRepository
{
    private readonly IAuthService _authService;

    public ProfessionalUserRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    protected override IQueryable<ProfessionalUser> GetDetailsQuery()
    {
        return base.GetDetailsQuery()
            .Include(proUser => proUser.User);
    }

    protected override IQueryable<ProfessionalUser> GetListQuery()
    {
        return base.GetListQuery()
            .Include(proUser => proUser.User);
    }

    public override Task<IEnumerable<ProfessionalUser>> GetAllAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(ProfessionalUser.UserId)] = SortOperator.Asc;
        }

        return base.GetAllAsync(pageInfo, query, cancellationToken);
    }

    protected override void UpdateEntity(ProfessionalUser model)
    {
        base.UpdateEntity(model);

        var isAdmin = _authService.HasAnyRole(RoleDefinitions.Professionals.Admin);
        var entry = Entity.Entry(model);

        entry.Property(u => u.IsAdmin).IsModified = isAdmin;
        entry.Property(u => u.IsBackflowTester).IsModified = isAdmin;
        entry.Property(u => u.IsCsiInspector).IsModified = isAdmin;
        entry.Property(u => u.IsFogInspector).IsModified = isAdmin;
        entry.Property(u => u.IsFogTransporter).IsModified = isAdmin;
        entry.Property(u => u.IsWiseGuy).IsModified = isAdmin;
    }

    public async Task<ProfessionalUser?> UpdateNonSensitiveDataAsync(ProfessionalUser user)
    {
        user.ProfessionalId = _authService.ProfessionalId;
        var existing = await DbContext.ProfessionalUsers.SingleOrDefaultAsync(u => u.ProfessionalId == user.ProfessionalId && u.UserId == user.UserId);

        if (existing != null)
        {
            existing.ContactName = user.ContactName;
            existing.JobTitle = user.JobTitle;

            await DbContext.SaveChangesAsync();
        }

        return existing;
    }
}