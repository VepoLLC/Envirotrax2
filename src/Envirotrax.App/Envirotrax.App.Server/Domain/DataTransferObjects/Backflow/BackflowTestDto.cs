using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Users;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestDto : IDto
{
    public int Id { get; set; }

    public ReferencedSiteDto? Site { get; set; }

    [StringLength(50)]
    public string? SubmissionId { get; set; }

    [StringLength(50)]
    public string? JobNumber { get; set; }

    // BPAT
    public int? ProfessionalId { get; set; }
    public int? BpatId { get; set; }

    [StringLength(50)]
    public string? BpatLicenseNumber { get; set; }

    public DateTime? BpatLicenseExpiration { get; set; }

    [StringLength(100)]
    public string? BpatCompanyName { get; set; }

    [StringLength(100)]
    public string? BpatContactName { get; set; }

    public ReferencedStateDto? BpatState { get; set; }

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

    public ReferencedStateDto? MailingState { get; set; }

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

    [StringLength(200)]
    public string? LocationDescription { get; set; }

    [StringLength(100)]
    public string? HazardType { get; set; }

    // Test info
    public BackflowReasonForTest ReasonForTest { get; set; }

    public DateTime? TestDate { get; set; }
    public DateTime? InitialTestDate { get; set; }
    public DateTime? FinalTestDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public BackflowTestResult TestResult { get; set; }

    public bool ProperlyInstalled { get; set; }
    public bool NonPotable { get; set; }
    public bool GaugeNonPotable { get; set; }

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

    public DateTime? ApprovalDate { get; set; }
    public int? ApprovedById { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public decimal Amount { get; set; }
    public decimal AmountShare { get; set; }

    public bool Rejected { get; set; }
    public int? RejectedById { get; set; }
    public DateTime? RejectedDate { get; set; }
    public string? RejectedReason { get; set; }

    public bool NeedsValidation { get; set; }

    // Audit
    public DateTime CreatedTime { get; set; }
    public DateTime? UpdatedTime { get; set; }
    public AppUserDto? UpdatedBy { get; set; }
}
