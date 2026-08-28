using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Logs;

public class RecordLog : ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public long Id { get; set; }

    public RecordLogType LogType { get; set; }

    [StringLength(100)]
    public string TableName { get; set; } = string.Empty;

    public int RecordId { get; set; }

    public string? Description { get; set; }

    public int? WaterSupplierId { get; set; }
    public WaterSupplier? WaterSupplier { get; set; }

    public int? ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    [StringLength(50)]
    public string? IpAddress { get; set; }

    [StringLength(100)]
    public string? SessionId { get; set; }

    // ICreateAuditableModel<AppUser>
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class RecordLogConfiguration : IEntityTypeConfiguration<RecordLog>
{
    public void Configure(EntityTypeBuilder<RecordLog> builder)
    {
        builder.HasIndex(l => new { l.TableName, l.RecordId, l.WaterSupplierId, l.ProfessionalId });
    }
}
