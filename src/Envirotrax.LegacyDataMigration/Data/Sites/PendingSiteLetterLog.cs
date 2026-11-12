using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.LegacyDataMigration.Data.Sites;

public class PendingSiteLetterLog
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int WaterSupplierId { get; set; }

    public int SiteId { get; set; }

    public DateTime ProcessDate { get; set; }

    [MaxLength(255)]
    public string FileName { get; set; } = string.Empty;

    public int Page { get; set; }

    [MaxLength(500)]
    public string LegacyFilePath { get; set; } = string.Empty;

    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;
}
