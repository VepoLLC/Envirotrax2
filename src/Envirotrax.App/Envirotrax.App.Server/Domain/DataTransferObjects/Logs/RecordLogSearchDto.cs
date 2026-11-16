using Envirotrax.App.Server.Data.Models.Logs;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Logs;

public class RecordLogSearchDto
{
    public string? TableName { get; set; }

    public int? WaterSupplierId { get; set; }

    public int? RecordId { get; set; }

    public string? UserEmail { get; set; }

    public RecordLogType? LogType { get; set; }
}
