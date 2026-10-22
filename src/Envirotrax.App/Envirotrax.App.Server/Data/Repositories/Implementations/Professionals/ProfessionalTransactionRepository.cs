using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalTransactionRepository : Repository<ProfessionalTransaction>, IProfessionalTransactionRepository
{
    private readonly IAuthService _authService;

    public ProfessionalTransactionRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    protected override IQueryable<ProfessionalTransaction> GetListQuery()
    {
        return ScopeToCurrentUser(base.GetListQuery())
            .Include(transaction => transaction.User)
            .ThenInclude(user => user!.User);
    }

    public async Task<ProfessionalTransaction?> GetMyByTransactionIdAsync(string transactionId, CancellationToken cancellationToken)
    {
        return await ScopeToCurrentUser(Entity.AsNoTracking())
            .SingleOrDefaultAsync(transaction => transaction.TransactionId == transactionId, cancellationToken);
    }

    private IQueryable<ProfessionalTransaction> ScopeToCurrentUser(IQueryable<ProfessionalTransaction> query)
    {
        var isAdmin = _authService.HasAnyRole(RoleDefinitions.Professionals.Admin);

        return query.WhereIf(!isAdmin, transaction => transaction.UserId == _authService.UserId);
    }

    public async Task<ProfessionalTransaction?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken)
    {
        return await Entity
            .AsNoTracking()
            .SingleOrDefaultAsync(transaction => transaction.TransactionId == transactionId, cancellationToken);
    }
}
