using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Sites;

[Table("SiteLogs")]
public class SiteLog : TenantModel<WaterSupplier>, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int SiteId { get; set; }
    public Site? Site { get; set; }

    public SiteLogType LogType { get; set; }

    [Column(TypeName = "text")]
    public string? NoteText { get; set; }

    public DateTime? ReviewDate { get; set; }

    public int? AssemblyId { get; set; }
    public BackflowTest? Assembly { get; set; }

    [MaxLength(255)]
    public string? FileAttachmentName { get; set; }

    [MaxLength(500)]
    public string? FileAttachmentPath { get; set; }

    // When true, skip Azure SAS URL generation (e.g. V1-migrated records with files on old shared drive)
    public bool SkipFile { get; set; }

    // ICreateAuditableModel<AppUser>
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
