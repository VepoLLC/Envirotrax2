
using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Professionals.Licenses;

public class ProfessionalUserLicense : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int UserId { get; set; }
    public AppUser? User { get; set; }

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

public class ProfessionalUserConfiguration : IEntityTypeConfiguration<ProfessionalUserLicense>
{
    public void Configure(EntityTypeBuilder<ProfessionalUserLicense> builder)
    {
        builder.HasOne<ProfessionalUser>()
            .WithMany()
            .HasForeignKey(license => new { license.ProfessionalId, license.UserId });
    }
}

public enum ProfessionalType
{
    Contractor,
    PlanChecker,
    Bpat,
    Inspector,
    CsiInspector,
    FogTransporter,
    FogInspector,
    ComponentTester,
}
