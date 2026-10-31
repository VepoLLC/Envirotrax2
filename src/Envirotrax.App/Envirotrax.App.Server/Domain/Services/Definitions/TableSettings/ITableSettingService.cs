using Envirotrax.App.Server.Data.Models.TableSettings;
using Envirotrax.App.Server.Domain.DataTransferObjects.TableSettings;

namespace Envirotrax.App.Server.Domain.Services.Definitions.TableSettings;

public interface ITableSettingService : IService<TableSettingDto>
{
    Task<IEnumerable<TableSettingDto>> GetAllMyAsync(TableSettingType tableType, CancellationToken cancellationToken);
    Task<bool> CanAddMyAsync(CancellationToken cancellationToken);
    Task<TableSettingDto> AddMyAsync(TableSettingDto dto, CancellationToken cancellationToken);
    Task<TableSettingDto?> UpdateMyAsync(int id, TableSettingDto dto, CancellationToken cancellationToken);
    Task<TableSettingDto?> DeleteMyAsync(int id, CancellationToken cancellationToken);
}
