namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;

public class RecordLogSearchDto
{
    public string? TableName { get; set; }

    public int? WaterSupplierId { get; set; }

    public int? RecordId { get; set; }

    public string? UserEmail { get; set; }

    public int? LogType { get; set; }

    public IDictionary<string, string> ToParameters()
    {
        return new Dictionary<string, string>
        {
            [nameof(TableName)] = TableName ?? string.Empty,
            [nameof(WaterSupplierId)] = WaterSupplierId?.ToString() ?? string.Empty,
            [nameof(RecordId)] = RecordId?.ToString() ?? string.Empty,
            [nameof(UserEmail)] = UserEmail ?? string.Empty,
            [nameof(LogType)] = LogType?.ToString() ?? string.Empty
        };
    }
}
