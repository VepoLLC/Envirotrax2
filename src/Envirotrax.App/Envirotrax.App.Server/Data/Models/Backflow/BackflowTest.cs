using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Models.Backflow;

[Table("BackflowTests")]
public class BackflowTest : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public int Id { get; set; }

    public int? SiteId { get; set; }
    public Site? Site { get; set; }

    public DateTime CreationDate { get; set; }

    [StringLength(50)]
    public string? SubmissionId { get; set; }

    [StringLength(50)]
    public string? JobNumber { get; set; }

    // BPAT (Backflow Prevention Assembly Tester)
    [StringLength(50)]
    public string? MasterBpatId { get; set; }

    [StringLength(50)]
    public string? BpatId { get; set; }

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

    [StringLength(50)]
    public string? BpatState { get; set; }

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

    [StringLength(50)]
    public string? PropertyState { get; set; }

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

    [StringLength(50)]
    public string? MailingState { get; set; }

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

    [StringLength(200)]
    public string? LocationDescription { get; set; }

    [StringLength(100)]
    public string? HazardType { get; set; }

    [StringLength(200)]
    public string? HazardTypeOtherDescription { get; set; }

    // Test info
    public BackflowReasonForTest ReasonForTest { get; set; }

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

    [StringLength(100)]
    public string? ApprovedBy { get; set; }

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

    [StringLength(100)]
    public string? RejectedBy { get; set; }

    public DateTime? RejectedDate { get; set; }

    public string? RejectedReason { get; set; }

    // Validation flags
    public bool NeedsValidation { get; set; }
    public bool ValidationNewSite { get; set; }
    public bool ValidationSiteInformationChanged { get; set; }
    public bool ValidationUnknownSerialNumber { get; set; }
    public bool ValidationDeviceInformationChanged { get; set; }
}
