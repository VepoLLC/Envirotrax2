using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Sites;

[Table("SiteSchedules")]
public class SiteSchedule : IProfessionalModel, ICreateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int UserId { get; set; }
    public ProfessionalUser? User { get; set; }

    public ProfessionalType ProfessionalType { get; set; }

    public int SiteId { get; set; }
    public Site? Site { get; set; }

    public DateTime ScheduleDate { get; set; }

    // Audit
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
}

public class SiteScheduleConfiguration : IEntityTypeConfiguration<SiteSchedule>
{
    public void Configure(EntityTypeBuilder<SiteSchedule> builder)
    {
        builder.HasOne(schedule => schedule.User)
            .WithMany()
            .HasForeignKey(schedule => new { schedule.ProfessionalId, schedule.UserId })
            .HasPrincipalKey(user => new { user.ProfessionalId, user.UserId })
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(schedule => new { schedule.SiteId, schedule.ProfessionalId, schedule.UserId, schedule.ProfessionalType })
            .IsUnique();
    }
}
