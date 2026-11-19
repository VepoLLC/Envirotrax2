
using Envirotrax.App.Server.Data.Models.Logs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Backflow;

[Table("BackflowGauges")]
[RecordLogged(RecordLogTableNames.BackflowGauges, ProfessionalSource = RecordLogIdSource.Entity)]
public class BackflowGauge : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    // Null means the gauge belongs to the company and every tester can use it. Otherwise only tests
    // submitted for this user can, like a sub-account's own gauge in V1.
    public int? ProfessionalUserId { get; set; }
    public ProfessionalUser? ProfessionalUser { get; set; }

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

public class BackflowGaugeConfiguration : IEntityTypeConfiguration<BackflowGauge>
{
    public void Configure(EntityTypeBuilder<BackflowGauge> builder)
    {
        builder.HasOne(gauge => gauge.ProfessionalUser)
            .WithMany()
            .HasForeignKey(gauge => new { gauge.ProfessionalId, gauge.ProfessionalUserId })
            .HasPrincipalKey(user => new { user.ProfessionalId, user.UserId });
    }
}
