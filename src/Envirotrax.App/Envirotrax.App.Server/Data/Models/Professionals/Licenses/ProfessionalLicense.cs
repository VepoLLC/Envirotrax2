using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Logs;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.Professionals.Licenses;

[RecordLogged(RecordLogTableNames.ProfessionalLicenses, WaterSupplierSource = RecordLogIdSource.Ambient, ProfessionalSource = RecordLogIdSource.Entity)]
public class ProfessionalLicense : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public ProfessionalType ProfessionalType { get; set; }

    public int LicenseTypeId { get; set; }
    public ProfessionalLicenseType? LicenseType { get; set; }

    [Required]
    [StringLength(50)]
    public string LicenseNumber { get; set; } = null!;

    public DateTime? ExpirationDate { get; set; }

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}
