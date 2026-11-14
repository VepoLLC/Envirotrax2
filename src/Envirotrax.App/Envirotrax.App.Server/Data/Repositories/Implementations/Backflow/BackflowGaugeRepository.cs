
using System.Linq.Expressions;
using DeveloperPartners.SortingFiltering;
using DeveloperPartners.SortingFiltering.EntityFrameworkCore;
using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Backflow;

public class BackflowGaugeRepository : Repository<BackflowGauge>, IBackflowGaugeRepository
{
    private readonly IAuthService _authService;

    public BackflowGaugeRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    protected override IQueryable<BackflowGauge> GetListQuery()
    {
        return IncludeAssignedUser(OnlyOwnGaugesForTesterWithoutAdminRights(base.GetListQuery()));
    }

    protected override IQueryable<BackflowGauge> GetDetailsQuery()
    {
        return IncludeAssignedUser(OnlyOwnGaugesForTesterWithoutAdminRights(base.GetDetailsQuery()));
    }

    public override async Task<BackflowGauge?> GetTrackedForUpdateAsync(int id, CancellationToken cancellationToken)
    {
        return await OnlyOwnGaugesForTesterWithoutAdminRights(Entity).SingleOrDefaultAsync(gauge => gauge.Id == id, cancellationToken);
    }

    private static IQueryable<BackflowGauge> IncludeAssignedUser(IQueryable<BackflowGauge> query)
    {
        return query
            .Include(gauge => gauge.ProfessionalUser)
            .ThenInclude(user => user!.User);
    }

    private IQueryable<BackflowGauge> OnlyOwnGaugesForTesterWithoutAdminRights(IQueryable<BackflowGauge> query)
    {
        var isTesterWithoutAdminRights = DbContext is ProfessionalDbContext
            && !_authService.HasAnyRole(RoleDefinitions.Professionals.Admin);

        return query.WhereIf(isTesterWithoutAdminRights, gauge => gauge.ProfessionalUserId == _authService.ActingAsUserId);
    }

    private static Expression<Func<BackflowGauge, bool>> IsCompanyOrUserGauge(int userId)
    {
        return gauge => gauge.ProfessionalUserId == null || gauge.ProfessionalUserId == userId;
    }

    public Task<int> CountCompanyAndUserGaugesAsync(int userId, CancellationToken cancellationToken)
    {
        return Entity.CountAsync(IsCompanyOrUserGauge(userId), cancellationToken);
    }

    public async Task<IEnumerable<BackflowGauge>> GetCompanyAndUserGaugesAsync(int userId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowGauge.Id)] = SortOperator.Asc;
        }

        var paginated = await IncludeAssignedUser(Entity.AsNoTracking())
            .Where(IsCompanyOrUserGauge(userId))
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<BackflowGauge?> GetCompanyOrUserGaugeAsync(int gaugeId, int userId, CancellationToken cancellationToken)
    {
        return Entity
            .AsNoTracking()
            .Where(IsCompanyOrUserGauge(userId))
            .SingleOrDefaultAsync(gauge => gauge.Id == gaugeId, cancellationToken);
    }

    public async Task<IEnumerable<BackflowGauge>> DeleteAssignedToUserAsync(int professionalId, int userId, CancellationToken cancellationToken)
    {
        var gauges = await Entity
            .Where(gauge => gauge.ProfessionalId == professionalId && gauge.ProfessionalUserId == userId)
            .ToListAsync(cancellationToken);

        Entity.RemoveRange(gauges);
        await DbContext.SaveChangesAsync(cancellationToken);

        return gauges;
    }

    public async Task<IEnumerable<BackflowGauge>> GetAllByProfessionalAsync(int professionalId, PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var paginated = await IncludeAssignedUser(DbContext.BackflowGauges.AsNoTracking())
            .Where(g => g.ProfessionalId == professionalId)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<BackflowGauge?> UpdateGaugeAsync(BackflowGauge model)
    {
        var gauge = await GetTrackedForUpdateAsync(model.Id, default);

        if (gauge == null)
        {
            return null;
        }

        gauge.Manufacturer = model.Manufacturer;
        gauge.Model = model.Model;
        gauge.SerialNumber = model.SerialNumber;
        gauge.LastCalibrationDate = model.LastCalibrationDate;
        gauge.IsPortable = model.IsPortable;
        gauge.FilePath = model.FilePath;

        await SaveChangesAsync(logData: true);

        return gauge;
    }

    private IQueryable<BackflowGauge> ScopedToWaterSupplier()
    {
        return DbContext.BackflowGauges
            .AsNoTracking()
            .Where(g => DbContext.ProfessionalWaterSuppliers.Any(pws =>
                pws.ProfessionalId == g.ProfessionalId && pws.HasBackflowTesting));
    }

    public async Task<IEnumerable<BackflowGauge>> GetUnverifiedByWaterSupplierAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        var baseQuery = ScopedToWaterSupplier()
            .Where(g => g.LastCalibrationDate == null && !g.IsManaged)
            .Include(g => g.Professional)
            .Include(g => g.CreatedBy);

        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowGauge.Id)] = SortOperator.Asc;
        }

        var paginated = await baseQuery
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnverifiedCountByWaterSupplierAsync(CancellationToken cancellationToken)
    {
        return await ScopedToWaterSupplier()
            .CountAsync(g => g.LastCalibrationDate == null && !g.IsManaged, cancellationToken);
    }

    public async Task<IEnumerable<BackflowGauge>> GetUnverifiedForAdminAsync(PageInfo pageInfo, Query query, CancellationToken cancellationToken)
    {
        if (query.Sort.IsNullOrEmpty())
        {
            query.Sort[nameof(BackflowGauge.Id)] = SortOperator.Asc;
        }

        var paginated = await DbContext.BackflowGauges
            .AsNoTracking()
            .Where(gauge => gauge.LastCalibrationDate == null)
            .Where(query.Filter)
            .OrderBy(query.Sort)
            .PaginateAsync(pageInfo, cancellationToken);

        return await paginated.ToListAsync(cancellationToken);
    }

    public Task<int> CountUnverifiedForAdminAsync(CancellationToken cancellationToken)
    {
        return DbContext.BackflowGauges.CountAsync(gauge => gauge.LastCalibrationDate == null, cancellationToken);
    }

    public async Task<IEnumerable<ProfessionalUser>> GetProfessionalUsersAsync(IEnumerable<int> professionalIds, CancellationToken cancellationToken)
    {
        return await DbContext.ProfessionalUsers
            .AsNoTracking()
            .Include(pu => pu.User)
            .Where(pu => professionalIds.Contains(pu.ProfessionalId))
            .ToListAsync(cancellationToken);
    }
}
