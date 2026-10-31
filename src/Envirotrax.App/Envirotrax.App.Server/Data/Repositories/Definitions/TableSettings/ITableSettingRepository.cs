using Envirotrax.App.Server.Data.Models.TableSettings;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.TableSettings;

public interface ITableSettingRepository : IRepository<TableSetting>
{
    Task<IEnumerable<TableSetting>> GetAllByUserAsync(int userId, TableSettingType tableType, CancellationToken cancellationToken);
    Task<bool> ExistsForUserAsync(int id, int userId, CancellationToken cancellationToken);
}
