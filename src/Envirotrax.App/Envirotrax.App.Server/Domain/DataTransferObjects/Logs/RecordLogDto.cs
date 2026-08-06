using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Logs;

public class RecordLogDto : IDto
{
    public int Id { get; set; }

    public DateTime LogDate { get; set; }

    public RecordLogType LogType { get; set; }

    public string? TableName { get; set; }

    public int RecordId { get; set; }

    public string? Description { get; set; }

    public AppUserDto? User { get; set; }
}
