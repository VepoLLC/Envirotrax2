using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.LegacyDataMigration.Data.Csi;

public class PendingCsiInspectionImage
{
    [AppPrimaryKey(false)]
    public int LegacyRecordId { get; set; }

    public int WaterSupplierId { get; set; }

    public int InspectionId { get; set; }

    public int ProfessionalId { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string LegacyFilePath { get; set; } = string.Empty;

    public bool IsStoredInDatabase { get; set; }
}
