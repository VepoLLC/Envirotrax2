
using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.LegacyDataMigration.Data.Sites;

// Only the columns the attachment migration reads and writes. The full model lives in
// Envirotrax.App.Server; rows are inserted by Scripts/SiteLogs/02_SiteLogs.sql, never by EF, so the
// columns that script alone fills in are deliberately absent here.
public class SiteLog : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int SiteId { get; set; }

    [MaxLength(500)]
    public string? FileAttachmentPath { get; set; }

    public bool SkipFile { get; set; }

    // Points back to Vepo.dbo.CsiBackflowSiteLog.ID. It names the blob, because it survives a wipe and
    // rebuild of the V2 database while the identity value in Id does not.
    public int? LegacyRecordId { get; set; }

    // Where the file sits underneath the legacy file server root, until it is copied into Azure Storage.
    [MaxLength(500)]
    public string? LegacyFilePath { get; set; }
}
