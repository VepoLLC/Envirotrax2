
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalRepository : Repository<Professional>, IProfessionalRepository
{
    private static readonly TimeSpan BalanceLockDuration = TimeSpan.FromSeconds(90);

    private ITenantProvidersService _tenantProvider;

    public ProfessionalRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void UpdateEntity(Professional model)
    {
        base.UpdateEntity(model);

        // We are not going to update HasWiseGuys from API. If needed, it will only be updated from the database.
        DbContext.Entry(model).Property(p => p.HasWiseGuys).IsModified = false;
        DbContext.Entry(model).Property(p => p.AccountBalance).IsModified = false;
        DbContext.Entry(model).Property(p => p.BalanceLockedUntil).IsModified = false;
    }

    public override async Task<Professional?> UpdateAsync(Professional model)
    {
        await base.UpdateAsync(model);
        await DbContext.Entry(model).ReloadAsync();

        return model;
    }

    public async Task<IEnumerable<Professional>> GetAllMyAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await DbContext
            .ProfessionalUsers
            .IgnoreQueryFilters()
            .Where(proUser => proUser.UserId == _tenantProvider.UserId)
            .Select(proUser => proUser.Professional!)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Professional>> GetSubAccountsAsync(CancellationToken cancellationToken)
    {
        return await DbContext.Set<Professional>()
            .IgnoreQueryFilters()
            .Where(p => p.ParentId == _tenantProvider.ProfessionalId && p.DeletedTime == null)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IAsyncDisposable?> TryAcquireBalanceLockAsync(int professionalId, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var lockedUntil = now.Add(BalanceLockDuration);

        var acquiredCount = await DbContext.Professionals
            .Where(professional => professional.Id == professionalId
                && (professional.BalanceLockedUntil == null || professional.BalanceLockedUntil < now))
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(professional => professional.BalanceLockedUntil, lockedUntil), cancellationToken);

        if (acquiredCount == 0)
        {
            return null;
        }

        return new BalanceLock(this, professionalId, lockedUntil);
    }

    public async Task<bool> TryDebitBalanceAsync(int professionalId, decimal amount, CancellationToken cancellationToken)
    {
        var updatedCount = await DbContext.Professionals
            .Where(professional => professional.Id == professionalId && professional.AccountBalance >= amount)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(professional => professional.AccountBalance, professional => professional.AccountBalance - amount), cancellationToken);

        return updatedCount == 1;
    }

    public async Task CreditBalanceAsync(int professionalId, decimal amount, CancellationToken cancellationToken)
    {
        var updatedCount = await DbContext.Professionals
            .Where(professional => professional.Id == professionalId)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(professional => professional.AccountBalance, professional => professional.AccountBalance + amount), cancellationToken);

        if (updatedCount != 1)
        {
            throw new InvalidOperationException($"Professional {professionalId} not found.");
        }
    }

    private async Task ReleaseBalanceLockAsync(int professionalId, DateTime lockedUntil)
    {
        await DbContext.Professionals
            .Where(professional => professional.Id == professionalId && professional.BalanceLockedUntil == lockedUntil)
            .ExecuteUpdateAsync(setter => setter
                .SetProperty(professional => professional.BalanceLockedUntil, (DateTime?)null));
    }

    private class BalanceLock : IAsyncDisposable
    {
        private readonly ProfessionalRepository _repository;
        private readonly int _professionalId;
        private readonly DateTime _lockedUntil;

        public BalanceLock(ProfessionalRepository repository, int professionalId, DateTime lockedUntil)
        {
            _repository = repository;
            _professionalId = professionalId;
            _lockedUntil = lockedUntil;
        }

        public async ValueTask DisposeAsync()
        {
            await _repository.ReleaseBalanceLockAsync(_professionalId, _lockedUntil);
        }
    }
}