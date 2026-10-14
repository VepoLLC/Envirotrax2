using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Notifications;

public class NotificationDigestSettingDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto WaterSupplier { get; set; } = null!;
}
