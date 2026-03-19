
using DeveloperPartners.SortingFiltering;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalUserRepository : Repository<ProfessionalUser>, IProfessionalUserRepository
{
    public ProfessionalUserRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
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
}