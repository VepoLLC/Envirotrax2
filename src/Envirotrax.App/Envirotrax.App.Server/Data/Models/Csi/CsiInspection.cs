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

namespace Envirotrax.App.Server.Data.Models.Csi;

[Table("CsiInspections")]
public class CsiInspection : TenantModel<WaterSupplier>, IAuditableModel<AppUser>
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

    [StringLength(20)]
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

    [StringLength(20)]
    public string? MailingZip { get; set; }

    [StringLength(50)]
    public string? MailingPhoneNumber { get; set; }

    [StringLength(100)]
    public string? MailingEmailAddress { get; set; }

    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }

    public int UserId { get; set; }
    public ProfessionalUser? Inspector { get; set; }

    [StringLength(50)]
    public string? InspectorLicenseNumber { get; set; }

    [StringLength(50)]
    public string? InspectorLicenseType { get; set; }

    [StringLength(100)]
    public string? InspectorCompanyName { get; set; }

    [StringLength(100)]
    public string? InspectorJobTitle { get; set; }

    [StringLength(100)]
    public string? InspectorContactName { get; set; }

    [StringLength(200)]
    public string? InspectorAddress { get; set; }

    [StringLength(50)]
    public string? InspectorCity { get; set; }

    [StringLength(50)]
    public string? InspectorState { get; set; }

    [StringLength(20)]
    public string? InspectorZip { get; set; }

    [StringLength(50)]
    public string? InspectorWorkNumber { get; set; }

    [StringLength(50)]
    public string? InspectorCellNumber { get; set; }

    [StringLength(50)]
    public string? InspectorFaxNumber { get; set; }

    // Inspection criteria
    public CsiInspectionReason ReasonForInspection { get; set; }

    public bool Compliance1 { get; set; }
    public bool Compliance2 { get; set; }
    public bool Compliance3 { get; set; }
    public bool Compliance4 { get; set; }
    public bool Compliance5 { get; set; }
    public bool Compliance6 { get; set; }

    // Material - service line
    public bool MaterialServiceLineLead { get; set; }
    public bool MaterialServiceLineCopper { get; set; }
    public bool MaterialServiceLinePVC { get; set; }
    public bool MaterialServiceLineOther { get; set; }

    [StringLength(200)]
    public string? MaterialServiceLineOtherDescription { get; set; }

    // Material - solder
    public bool MaterialSolderLead { get; set; }
    public bool MaterialSolderLeadFree { get; set; }
    public bool MaterialSolderSolventWeld { get; set; }
    public bool MaterialSolderOther { get; set; }

    [StringLength(200)]
    public string? MaterialSolderOtherDescription { get; set; }

    // Approval
    public bool Disapproved { get; set; }
    public string? DisapprovedReason { get; set; }

    // Additional infrastructure (AI) flags
    public bool AiOssf { get; set; }
    public bool AiWaterWell { get; set; }
    public bool AiFireSystem { get; set; }
    public bool AiFireSystem2 { get; set; }
    public bool AiGreaseTrap { get; set; }
    public bool AiSandGrit { get; set; }
    public bool AiReclaimedWater { get; set; }
    public bool AiIrrigationSystem { get; set; }
    public bool AiIrrigationSystem2 { get; set; }
    public bool AiHasDomesticPremisesIsolation { get; set; }
    public bool AiRequiresDomesticPremisesIsolation { get; set; }

    public string? Comments { get; set; }

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

public class CsiInspectionConfiguration : IEntityTypeConfiguration<CsiInspection>
{
    public void Configure(EntityTypeBuilder<CsiInspection> builder)
    {
        builder.HasOne(i=> i.Inspector)
            .WithMany()
            .HasForeignKey(i => new { i.ProfessionalId, i.UserId })
            .OnDelete(DeleteBehavior.Restrict);
    }
}
