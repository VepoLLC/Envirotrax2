using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Envirotrax.App.Server.Data.Models.Professionals;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Fog;

[Table("FogInspections")]
public class FogInspection : TenantModel<WaterSupplier>, IAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int SiteId { get; set; }
    public Site? Site { get; set; }

    public DateTime? InspectionDate { get; set; }

    [StringLength(50)]
    public string? SubmissionId { get; set; }

    // Property / Location fields
    [StringLength(100)]
    public string? PropertyBusinessName { get; set; }

    public PropertyType PropertyType { get; set; }

    [StringLength(50)]
    public string? PropertyStreetNumber { get; set; }

    [StringLength(100)]
    public string? PropertyStreetName { get; set; }

    [StringLength(50)]
    public string? PropertyNumber { get; set; }

    [StringLength(50)]
    public string? PropertyCity { get; set; }

    public int? PropertyStateId { get; set; }
    public State? PropertyState { get; set; }

    [StringLength(50)]
    public string? PropertyZip { get; set; }

    // Mailing / Contact fields
    [StringLength(100)]
    public string? MailingCompanyName { get; set; }

    [StringLength(100)]
    public string? MailingContactName { get; set; }

    [StringLength(50)]
    public string? MailingStreetNumber { get; set; }

    [StringLength(100)]
    public string? MailingStreetName { get; set; }

    [StringLength(50)]
    public string? MailingNumber { get; set; }

    [StringLength(50)]
    public string? MailingCity { get; set; }

    public int? MailingStateId { get; set; }
    public State? MailingState { get; set; }

    [StringLength(50)]
    public string? MailingZip { get; set; }

    [StringLength(50)]
    public string? MailingPhoneNumber { get; set; }

    [StringLength(100)]
    public string? MailingEmailAddress { get; set; }

    // Inspector fields
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int InspectorId { get; set; }
    public ProfessionalUser? Inspector { get; set; }

    [StringLength(255)]
    public string? InspectorCompanyName { get; set; }

    [StringLength(100)]
    public string? InspectorJobTitle { get; set; }

    [StringLength(255)]
    public string? InspectorContactName { get; set; }

    [StringLength(255)]
    public string? InspectorAddress { get; set; }

    [StringLength(255)]
    public string? InspectorCity { get; set; }

    [StringLength(255)]
    public string? InspectorState { get; set; }

    [StringLength(255)]
    public string? InspectorZip { get; set; }

    [StringLength(50)]
    public string? InspectorWorkNumber { get; set; }

    [StringLength(50)]
    public string? InspectorCellNumber { get; set; }

    [StringLength(50)]
    public string? InspectorFaxNumber { get; set; }

    // FOG specific fields
    [StringLength(50)]
    public string? FogGeneratorPhoneNumber { get; set; }

    [StringLength(100)]
    public string? FogGeneratorEmailAddress { get; set; }

    public FacilityType FacilityType { get; set; }

    public FogReasonForInspection ReasonForInspection { get; set; }

    // Interceptor fields
    [StringLength(100)]
    public string? InterceptorType { get; set; }

    [StringLength(200)]
    public string? InterceptorOtherDescription { get; set; }

    public int InterceptorCapacity { get; set; }

    public int InterceptorCapacityType { get; set; }

    [StringLength(200)]
    public string? InterceptorLocationDescription { get; set; }

    public double? InterceptorLatitude { get; set; }

    public double? InterceptorLongitude { get; set; }

    public string? InterceptorComments { get; set; }

    // Condition fields
    public bool Maintained { get; set; }
    public bool Accessible { get; set; }
    public bool PastOverflow { get; set; }

    // Chamber fields
    [StringLength(50)]
    public string? InletChamberWettingHeight { get; set; }

    [StringLength(50)]
    public string? InletChamberGreaseBlanket { get; set; }

    [StringLength(50)]
    public string? InletChamberSediments { get; set; }

    [StringLength(50)]
    public string? OutletChamberWettingHeight { get; set; }

    [StringLength(50)]
    public string? OutletChamberGreaseBlanket { get; set; }

    [StringLength(50)]
    public string? OutletChamberSediments { get; set; }

    public bool InletTeeIntact { get; set; }
    public bool OutletTeeIntact { get; set; }
    public bool? InletTeeVisible { get; set; }
    public bool? OutletTeeVisible { get; set; }

    // Sampling fields
    [StringLength(100)]
    public string? SampledFrom { get; set; }

    public bool SamplingPointAccessible { get; set; }
    public bool SamplingPointClean { get; set; }

    // Capacity
    public int InletTotalCapacityPercent { get; set; }
    public int OutletTotalCapacityPercent { get; set; }
    public int TotalCapacityPercent { get; set; }

    public FogInspectionResult InspectionResult { get; set; }

    [StringLength(255)]
    public string? SignatureContactName { get; set; }

    public DateTime? SignatureDate { get; set; }

    public string? Comments { get; set; }

    // Trip ticket fields
    public DateTime? LastTripTicketDate { get; set; }
    public int TripTicketInterval { get; set; }

    // Validation
    public bool NeedsValidation { get; set; }
    public bool ValidationNewSite { get; set; }
    public bool ValidationSiteInformationChanged { get; set; }

    // Payment / transaction
    [StringLength(100)]
    public string? TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    [Precision(19, 4)]
    public decimal Amount { get; set; }

    [Precision(19, 4)]
    public decimal AmountShare { get; set; }

    public bool EmailPdf { get; set; }

    // Audit properties
    public int? CreatedById { get; set; }
    public AppUser? CreatedBy { get; set; }
    public DateTime CreatedTime { get; set; }
    public int? UpdatedById { get; set; }
    public AppUser? UpdatedBy { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public int? DeletedById { get; set; }
    public AppUser? DeletedBy { get; set; }
    public DateTime? DeletedTime { get; set; }
}

public class FogInspectionConfiguration : IEntityTypeConfiguration<FogInspection>
{
    public void Configure(EntityTypeBuilder<FogInspection> builder)
    {
        builder.HasOne(i => i.Inspector)
            .WithMany()
            .HasForeignKey(i => new { i.ProfessionalId, i.InspectorId })
            .HasPrincipalKey(pu => new { pu.ProfessionalId, pu.UserId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
