using Envirotrax.App.Server.Data.Models.TableSettings;
using Envirotrax.App.Server.Data.Repositories.Definitions.TableSettings;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.TableSettings;

public class TableSettingRepository : Repository<TableSetting>, ITableSettingRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public TableSettingRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    // Table settings belong to the logged-in user, so the reads never see another user's rows. That includes the
    // base DeleteAsync, which loads the row through GetDetailsQuery and returns null when it isn't found.
    protected override IQueryable<TableSetting> GetListQuery()
    {
        return base.GetListQuery().Where(setting => setting.UserId == _tenantProvider.UserId);
    }

    protected override IQueryable<TableSetting> GetDetailsQuery()
    {
        return base.GetDetailsQuery().Where(setting => setting.UserId == _tenantProvider.UserId);
    }

    public async Task<IEnumerable<TableSetting>> GetAllByTableTypeAsync(TableSettingType tableType, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .Where(setting => setting.TableType == tableType)
            .OrderBy(setting => setting.Name)
            .ToListAsync(cancellationToken);
    }
}
