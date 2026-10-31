using Envirotrax.App.Server.Data.Models.TableSettings;
using Envirotrax.App.Server.Data.Repositories.Definitions.TableSettings;
using Envirotrax.App.Server.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.TableSettings;

public class TableSettingRepository : Repository<TableSetting>, ITableSettingRepository
{
    public TableSettingRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task<IEnumerable<TableSetting>> GetAllByUserAsync(int userId, TableSettingType tableType, CancellationToken cancellationToken)
    {
        return await GetListQuery()
            .Where(setting => setting.UserId == userId && setting.TableType == tableType)
            .OrderBy(setting => setting.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsForUserAsync(int id, int userId, CancellationToken cancellationToken)
    {
        return Entity.AnyAsync(setting => setting.Id == id && setting.UserId == userId, cancellationToken);
    }
}
