
namespace Envirotrax.TaskRunner.Domain.DataTransferObjects;

public class NotificationSettingDto
{
    public int Id { get; set; }

    public WaterSupplierDto WaterSupplier { get; set; } = null!;

    public DateTimeOffset AsOfTime { get; set; }
}
