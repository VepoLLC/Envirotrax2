using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Csi;

[Table("CsiInspectionImages")]
public class CsiInspectionImage : TenantModel<WaterSupplier>, IProfessionalModel
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int InspectionId { get; set; }
    public CsiInspection? Inspection { get; set; }

    [MaxLength(500)]
    public string FilePath { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }
}
