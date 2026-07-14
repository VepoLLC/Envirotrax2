using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Backflow;

[Table("BackflowOutOfServiceRequests")]
public class BackflowOutOfServiceRequest : TenantModel<WaterSupplier>, IProfessionalModel
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int? BpatId { get; set; }
    public ProfessionalUser? Bpat { get; set; }

    public int TestId { get; set; }
    public BackflowTest? Test { get; set; }

    public OutOfServiceType Type { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public int? ReplacementAssemblyTestId { get; set; }
    public BackflowTest? ReplacementAssemblyTest { get; set; }

    public DateTime? OutOfServiceDate { get; set; }
    public DateTime? ClearedDate { get; set; }
}

public class BackflowOutOfServiceRequestConfiguration : IEntityTypeConfiguration<BackflowOutOfServiceRequest>
{
    public void Configure(EntityTypeBuilder<BackflowOutOfServiceRequest> builder)
    {
        builder.HasOne(r => r.Bpat)
            .WithMany()
            .HasForeignKey(r => new { r.ProfessionalId, r.BpatId })
            .HasPrincipalKey(pu => new { pu.ProfessionalId, pu.UserId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
