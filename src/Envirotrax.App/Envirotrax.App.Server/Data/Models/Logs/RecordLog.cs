using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Logs;

[Table("RecordLogs")]
public class RecordLog : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public DateTime LogDate { get; set; }

    public RecordLogType LogType { get; set; }

    [StringLength(100)]
    public string TableName { get; set; } = string.Empty;

    public int RecordId { get; set; }

    public string? Description { get; set; }

    public int? UserId { get; set; }
    public AppUser? User { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    [StringLength(100)]
    public string? SessionId { get; set; }
}
