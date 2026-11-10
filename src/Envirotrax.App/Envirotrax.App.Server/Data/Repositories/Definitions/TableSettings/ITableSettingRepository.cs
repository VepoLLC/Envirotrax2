using Envirotrax.App.Server.Data.Models.TableSettings;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.TableSettings;

public interface ITableSettingRepository : IRepository<TableSetting>
{
    Task<IEnumerable<TableSetting>> GetAllByTableTypeAsync(TableSettingType tableType, CancellationToken cancellationToken);
}
