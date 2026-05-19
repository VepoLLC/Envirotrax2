using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Models.Users;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Envirotrax.App.Server.Data.Models.Backflow;

[Table("BackflowTests")]
public class BackflowTest : TenantModel<WaterSupplier>, IAuditableModel<AppUser>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int? SiteId { get; set; }
    public Site? Site { get; set; }

    [StringLength(50)]
    public string? SubmissionId { get; set; }

    [StringLength(50)]
    public string? JobNumber { get; set; }

    // BPAT (Backflow Prevention Assembly Tester)
    public int? ProfessionalId { get; set; }
    public int? BpatId { get; set; }
    public ProfessionalUser? Bpat { get; set; }

    [StringLength(50)]
    public string? BpatLicenseNumber { get; set; }

    public DateTime? BpatLicenseExpiration { get; set; }

    [StringLength(100)]
    public string? BpatCompanyName { get; set; }

    [StringLength(100)]
    public string? BpatContactName { get; set; }

    [StringLength(200)]
    public string? BpatAddress { get; set; }

    [StringLength(50)]
    public string? BpatCity { get; set; }

    public int? BpatStateId { get; set; }
    public State? BpatState { get; set; }

    [StringLength(20)]
    public string? BpatZip { get; set; }

    [StringLength(50)]
    public string? BpatWorkNumber { get; set; }

    [StringLength(50)]
    public string? BpatCellNumber { get; set; }

    // Property / Site fields
    [StringLength(50)]
    public string? AccountNumber { get; set; }

    [StringLength(100)]
    public string? PropertyBusinessName { get; set; }

    public int PropertyType { get; set; }

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

    // Mailing address
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

    // Device info
    [StringLength(50)]
    public string? DeviceType { get; set; }

    [StringLength(100)]
    public string? Manufacturer { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [StringLength(50)]
    public string? Size { get; set; }

    [StringLength(100)]
    public string? SerialNumber { get; set; }

    public bool UnknownSerialNumber { get; set; }

    [StringLength(100)]
    public string? Manufacturer2 { get; set; }

    [StringLength(100)]
    public string? Model2 { get; set; }

    [StringLength(50)]
    public string? Size2 { get; set; }

    [StringLength(100)]
    public string? SerialNumber2 { get; set; }

    [StringLength(200)]
    public string? LocationDescription { get; set; }

    [StringLength(100)]
    public string? HazardType { get; set; }

    [StringLength(200)]
    public string? HazardTypeOtherDescription { get; set; }

    // Test info
    public BackflowReasonForTest ReasonForTest { get; set; }

    [StringLength(100)]
    public string? ReplacementAssembly { get; set; }

    public DateTime? InstallationDate { get; set; }
    public DateTime? TestDate { get; set; }
    public DateTime? InitialTestDate { get; set; }
    public DateTime? RepairTestDate { get; set; }
    public DateTime? FinalTestDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public BackflowTestResult TestResult { get; set; }

    public bool ProperlyInstalled { get; set; }
    public bool NonPotable { get; set; }

    // Gauge
    [StringLength(100)]
    public string? GaugeManufacturer { get; set; }

    [StringLength(100)]
    public string? GaugeModel { get; set; }

    [StringLength(50)]
    public string? GaugeSerialNumber { get; set; }

    public DateTime? GaugeLastCalibrationDate { get; set; }
    public bool GaugeNonPotable { get; set; }

    // Optional fields
    [StringLength(50)]
    public string? MeterNumber { get; set; }

    [Precision(5, 2)]
    public decimal? MeterReadingBefore { get; set; }
    public bool MeterRegisters { get; set; }
    [Precision(5, 2)]
    public decimal? MeterReadingAfter { get; set; }

    [StringLength(50)]
    public string? PermitNumber { get; set; }

    public bool Ossf { get; set; }

    // Status
    public string? Comments { get; set; }
    public bool IsCurrent { get; set; }
    public bool Disapproved { get; set; }
    public bool OutOfService { get; set; }
    public DateTime? OutOfServiceDate { get; set; }

    // Approval / Payment
    public DateTime? ApprovalDate { get; set; }

    public int? ApprovedById { get; set; }
    public WaterSupplierUser? ApprovedBy { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    [Precision(19, 4)]
    public decimal Amount { get; set; }

    [Precision(19, 4)]
    public decimal AmountShare { get; set; }

    public bool EmailPdf { get; set; }

    // Rejection
    public bool Rejected { get; set; }

    public int? RejectedById { get; set; }
    public WaterSupplierUser? RejectedBy { get; set; }

    public DateTime? RejectedDate { get; set; }

    public string? RejectedReason { get; set; }

    // Validation flags
    public bool NeedsValidation { get; set; }
    public bool ValidationNewSite { get; set; }
    public bool ValidationSiteInformationChanged { get; set; }
    public bool ValidationUnknownSerialNumber { get; set; }
    public bool ValidationDeviceInformationChanged { get; set; }

    // Initial test readings - main assembly
    [Precision(5, 2)]
    public decimal? InitCV1HeldPSID { get; set; }
    public bool InitCV1ClosedTight { get; set; }
    public bool InitCV1Leaked { get; set; }

    [Precision(5, 2)]
    public decimal? InitCV2HeldPSID { get; set; }
    public bool InitCV2ClosedTight { get; set; }
    public bool InitCV2Leaked { get; set; }

    [Precision(5, 2)]
    public decimal? InitRVOpenedPSID { get; set; }
    public bool InitRVDidNotOpen { get; set; }

    [Precision(5, 2)]
    public decimal? InitBCHeldPSID { get; set; }
    public bool InitBCClosedTight { get; set; }
    public bool InitBCLeaked { get; set; }

    [Precision(5, 2)]
    public decimal? InitPvbAirInletOpenedPSID { get; set; }
    public bool InitPvbAirInletDidNotOpen { get; set; }
    public bool InitPvbAirInletFullyOpened { get; set; }

    [Precision(5, 2)]
    public decimal? InitPvbCVHeldPSID { get; set; }
    public bool InitPvbCVLeaked { get; set; }

    // Air gap
    public bool AirGapValid { get; set; }

    // Repairs (stored as comma-separated text, e.g. "Cleaned, Replaced Disc, Replaced Spring")
    [StringLength(200)]
    public string? RepairCV1 { get; set; }

    [StringLength(100)]
    public string? RepairCV1Details { get; set; }

    [StringLength(200)]
    public string? RepairCV2 { get; set; }

    [StringLength(100)]
    public string? RepairCV2Details { get; set; }

    [StringLength(200)]
    public string? RepairRV { get; set; }

    [StringLength(100)]
    public string? RepairRVDetails { get; set; }

    [StringLength(200)]
    public string? RepairBC { get; set; }

    [StringLength(100)]
    public string? RepairBCDetails { get; set; }

    // Final test readings - main assembly
    [Precision(5, 2)]
    public decimal? FinalCV1HeldPSID { get; set; }
    public bool FinalCV1ClosedTight { get; set; }

    [Precision(5, 2)]
    public decimal? FinalCV2HeldPSID { get; set; }
    public bool FinalCV2ClosedTight { get; set; }

    [Precision(5, 2)]
    public decimal? FinalRVOpenedPSID { get; set; }

    [Precision(5, 2)]
    public decimal? FinalBCHeldPSID { get; set; }
    public bool FinalBCClosedTight { get; set; }

    [Precision(5, 2)]
    public decimal? FinalPvbAirInletOpenedPSID { get; set; }
    public bool FinalPvbAirInletFullyOpened { get; set; }

    [Precision(5, 2)]
    public decimal? FinalPvbCVHeldPSID { get; set; }

    // Bypass assembly readings (DCD/RPPD device types only)
    [Precision(5, 2)]
    public decimal? InitCV1HeldPSID2 { get; set; }
    public bool InitCV1ClosedTight2 { get; set; }
    public bool InitCV1Leaked2 { get; set; }

    [Precision(5, 2)]
    public decimal? InitCV2HeldPSID2 { get; set; }
    public bool InitCV2ClosedTight2 { get; set; }
    public bool InitCV2Leaked2 { get; set; }

    [Precision(5, 2)]
    public decimal? InitRVOpenedPSID2 { get; set; }
    public bool InitRVDidNotOpen2 { get; set; }

    [StringLength(200)]
    public string? RepairCV12 { get; set; }

    [StringLength(100)]
    public string? RepairCV1Details2 { get; set; }

    [StringLength(200)]
    public string? RepairCV22 { get; set; }

    [StringLength(100)]
    public string? RepairCV2Details2 { get; set; }

    [StringLength(200)]
    public string? RepairRV2 { get; set; }

    [StringLength(100)]
    public string? RepairRVDetails2 { get; set; }

    [Precision(5, 2)]
    public decimal? FinalCV1HeldPSID2 { get; set; }
    public bool FinalCV1ClosedTight2 { get; set; }
    public bool FinalCV2ClosedTight2 { get; set; }

    [Precision(5, 2)]
    public decimal? FinalRVOpenedPSID2 { get; set; }

    // Audit
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

public class BackflowTestConfiguration : IEntityTypeConfiguration<BackflowTest>
{
    public void Configure(EntityTypeBuilder<BackflowTest> builder)
    {
        builder.HasOne<ProfessionalUser>(bt => bt.Bpat)
            .WithMany()
            .HasForeignKey(bt => new { bt.ProfessionalId, bt.BpatId })
            .HasPrincipalKey(pu => new { pu.ProfessionalId, pu.UserId });
    }
}
