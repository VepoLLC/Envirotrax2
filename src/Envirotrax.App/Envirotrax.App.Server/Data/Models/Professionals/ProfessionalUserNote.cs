using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class ProfessionalUserNote : TenantModel<WaterSupplier>, ICreateAuditableModel<AppUser>, IUpdateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    public int UserId { get; set; }

    public ProfessionalUser? ProfessionalUser { get; set; }

    public ProfessionalType ProfessionalType { get; set; }

    [Required]
    public string NoteText { get; set; } = null!;

    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? UpdatedById { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
}

public class ProfessionalUserNoteConfiguration : IEntityTypeConfiguration<ProfessionalUserNote>
{
    public void Configure(EntityTypeBuilder<ProfessionalUserNote> builder)
    {
        builder.HasOne(note => note.ProfessionalUser)
            .WithMany()
            .HasForeignKey(note => new { note.ProfessionalId, note.UserId });

        builder.HasIndex(note => new { note.ProfessionalId, note.UserId, note.ProfessionalType });
    }
}
