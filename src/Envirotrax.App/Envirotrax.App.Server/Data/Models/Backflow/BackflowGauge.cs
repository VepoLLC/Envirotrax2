
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Backflow;

[Table("BackflowGauges")]
public class BackflowGauge : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    [Required]
    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = null!;

    [Required]
    [StringLength(50)]
    public string SerialNumber { get; set; } = null!;

    public DateTime? LastCalibrationDate { get; set; }

    public bool IsPortable { get; set; }

    public bool IsManaged { get; set; }

    public string? FilePath { get; set; }

    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
