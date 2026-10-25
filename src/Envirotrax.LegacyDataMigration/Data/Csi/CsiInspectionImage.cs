using System.ComponentModel.DataAnnotations;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.LegacyDataMigration.Data.Csi;

public class CsiInspectionImage : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int InspectionId { get; set; }

    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int ProfessionalId { get; set; }

    public int? LegacyRecordId { get; set; }
}
