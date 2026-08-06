
namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Logs;

public class RecordLogDto
{
    public int Id { get; set; }

    public DateTime LogDate { get; set; }

    public int LogType { get; set; }

    public string? TableName { get; set; }

    public int RecordId { get; set; }

    public string? Description { get; set; }

    public RecordLogUserDto? User { get; set; }
}

public class RecordLogUserDto
{
    public int Id { get; set; }

    public string? Email { get; set; }
}
