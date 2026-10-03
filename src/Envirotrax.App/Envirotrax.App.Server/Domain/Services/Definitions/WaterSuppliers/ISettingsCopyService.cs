using Envirotrax.App.Server.Data.Models.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

public interface ISettingsCopyService
{
    Task<bool> CanCopyFromParentAsync(CancellationToken cancellationToken);

    Task CopyFromParentAsync(SettingsSection section, CancellationToken cancellationToken);
}
