using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

public interface IBackflowSettingsService : IService<BackflowSettings, BackflowSettingsDto>
{
    Task<BackflowTestingSettingsDto> GetTestingSettingsAsync(int waterSupplierId, CancellationToken cancellationToken);

    Task<BackflowTestingSettingsDto> GetTestingSettingsByWaterSupplierAsync(int waterSupplierId, CancellationToken cancellationToken);

    Task<BackflowSettingsDto> AddOrUpdateAsync(int waterSupplierId, BackflowSettingsDto settings);
}
