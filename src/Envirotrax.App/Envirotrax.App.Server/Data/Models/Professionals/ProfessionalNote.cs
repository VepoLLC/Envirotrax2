using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Professionals;

public class ProfessionalNote : TenantModel<WaterSupplier>, ICreateAuditableModel<AppUser>, IUpdateAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

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

public class ProfessionalNoteConfiguration : IEntityTypeConfiguration<ProfessionalNote>
{
    public void Configure(EntityTypeBuilder<ProfessionalNote> builder)
    {
        builder.HasIndex(note => new { note.ProfessionalId, note.ProfessionalType });
    }
}
