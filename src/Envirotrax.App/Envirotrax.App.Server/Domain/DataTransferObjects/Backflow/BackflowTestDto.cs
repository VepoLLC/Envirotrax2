using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestDto : IDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public ReferencedSiteDto? Site { get; set; }

    [StringLength(50)]
    public string? SubmissionId { get; set; }

    [StringLength(50)]
    public string? JobNumber { get; set; }

    // BPAT
    public ReferencedProfessionalDto? Professional { get; set; }
    public ReferencedProfessionalUserDto? Bpat { get; set; }

    [StringLength(50)]
    public string? BpatLicenseNumber { get; set; }

    public DateTime? BpatLicenseExpiration { get; set; }

    [StringLength(100)]
    public string? BpatCompanyName { get; set; }

    [StringLength(100)]
    public string? BpatContactName { get; set; }

    public ReferencedStateDto? BpatState { get; set; }

    [StringLength(200)]
    public string? BpatAddress { get; set; }

    [StringLength(50)]
    public string? BpatCity { get; set; }

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

    public ReferencedStateDto? PropertyState { get; set; }

    [StringLength(20)]
    public string? PropertyZip { get; set; }

    // Mailing
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

    public ReferencedStateDto? MailingState { get; set; }

    [StringLength(20)]
    public string? MailingZip { get; set; }

    [StringLength(50)]
    public string? MailingPhoneNumber { get; set; }

    [StringLength(100)]
    public string? MailingEmailAddress { get; set; }

    // Device
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

    // Bypass assembly identity (DCD/DCD2/RPPD/RPPD2 only)
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
    [StringLength(100)]
    public string? GaugeManufacturer { get; set; }

    [StringLength(100)]
    public string? GaugeModel { get; set; }

    [StringLength(50)]
    public string? GaugeSerialNumber { get; set; }

    public DateTime? GaugeLastCalibrationDate { get; set; }

    public bool GaugeNonPotable { get; set; }

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

    // Status
    public string? Comments { get; set; }
    public bool IsCurrent { get; set; }
    public bool Disapproved { get; set; }
    public bool OutOfService { get; set; }
    public DateTime? OutOfServiceDate { get; set; }

    public DateTime? ApprovalDate { get; set; }
    public ReferencedWaterSupplierUserDto? ApprovedBy { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public decimal Amount { get; set; }
    public decimal AmountShare { get; set; }

    public bool Rejected { get; set; }
    public ReferencedWaterSupplierUserDto? RejectedBy { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectedReason { get; set; }

    public bool NeedsValidation { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public AppUserDto? UpdatedBy { get; set; }
}
