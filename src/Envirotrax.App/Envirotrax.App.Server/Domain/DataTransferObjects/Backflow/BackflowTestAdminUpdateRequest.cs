using System.ComponentModel.DataAnnotations;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestAdminUpdateRequest
{
    public bool IsCurrent { get; set; }
    public bool OutOfService { get; set; }
    public bool Disapproved { get; set; }
    public bool Rejected { get; set; }
    public bool NeedsValidation { get; set; }

    [StringLength(255)]
    public string? ValidationNotes { get; set; }

    public bool ForceRenewal { get; set; }
    public int ForceRenewalYears { get; set; }
    public int BackflowScheduleMonth { get; set; }
    public bool RenewalRequired { get; set; }

    public DateTime? TestDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    [StringLength(100)]
    public string? TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public decimal Amount { get; set; }
    public decimal AmountShare { get; set; }

    public int PropertyType { get; set; }

    [StringLength(100)]
    public string? PropertyBusinessName { get; set; }

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
    public string? DeviceType { get; set; }

    [StringLength(100)]
    public string? Manufacturer { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [StringLength(50)]
    public string? Size { get; set; }

    [StringLength(100)]
    public string? SerialNumber { get; set; }

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

    public BackflowTestResult TestResult { get; set; }

    [StringLength(50)]
    public string? JobNumber { get; set; }

    public BackflowReasonForTest ReasonForTest { get; set; }

    [StringLength(100)]
    public string? ReplacementAssembly { get; set; }

    public bool ProperlyInstalled { get; set; }
    public bool NonPotable { get; set; }

    public DateTime? InitialTestDate { get; set; }

    public decimal? InitCV1HeldPSID { get; set; }
    public bool InitCV1ClosedTight { get; set; }
    public bool InitCV1Leaked { get; set; }

    public decimal? InitCV2HeldPSID { get; set; }
    public bool InitCV2ClosedTight { get; set; }
    public bool InitCV2Leaked { get; set; }

    public decimal? InitRVOpenedPSID { get; set; }
    public bool InitRVDidNotOpen { get; set; }

    public decimal? InitBCHeldPSID { get; set; }
    public bool InitBCClosedTight { get; set; }
    public bool InitBCLeaked { get; set; }

    public decimal? InitPvbAirInletOpenedPSID { get; set; }
    public bool InitPvbAirInletDidNotOpen { get; set; }
    public bool InitPvbAirInletFullyOpened { get; set; }

    public decimal? InitPvbCVHeldPSID { get; set; }
    public bool InitPvbCVLeaked { get; set; }

    public decimal? InitCV1HeldPSID2 { get; set; }
    public bool InitCV1ClosedTight2 { get; set; }
    public bool InitCV1Leaked2 { get; set; }

    public decimal? InitCV2HeldPSID2 { get; set; }
    public bool InitCV2ClosedTight2 { get; set; }
    public bool InitCV2Leaked2 { get; set; }

    public decimal? InitRVOpenedPSID2 { get; set; }
    public bool InitRVDidNotOpen2 { get; set; }

    [StringLength(100)]
    public string? RepairCV1Details { get; set; }

    [StringLength(100)]
    public string? RepairCV2Details { get; set; }

    [StringLength(100)]
    public string? RepairRVDetails { get; set; }

    [StringLength(100)]
    public string? RepairBCDetails { get; set; }

    [StringLength(100)]
    public string? RepairCV1Details2 { get; set; }

    [StringLength(100)]
    public string? RepairCV2Details2 { get; set; }

    [StringLength(100)]
    public string? RepairRVDetails2 { get; set; }

    [StringLength(100)]
    public string? RepairPvbAirInletDetails { get; set; }

    [StringLength(100)]
    public string? RepairPvbCVDetails { get; set; }

    public DateTime? RepairTestDate { get; set; }

    public decimal? FinalCV1HeldPSID { get; set; }
    public bool FinalCV1ClosedTight { get; set; }

    public decimal? FinalCV2HeldPSID { get; set; }
    public bool FinalCV2ClosedTight { get; set; }

    public decimal? FinalRVOpenedPSID { get; set; }

    public decimal? FinalBCHeldPSID { get; set; }
    public bool FinalBCClosedTight { get; set; }

    public decimal? FinalPvbAirInletOpenedPSID { get; set; }
    public bool FinalPvbAirInletFullyOpened { get; set; }

    public decimal? FinalPvbCVHeldPSID { get; set; }

    public decimal? FinalCV1HeldPSID2 { get; set; }
    public bool FinalCV1ClosedTight2 { get; set; }

    public decimal? FinalCV2HeldPSID2 { get; set; }
    public bool FinalCV2ClosedTight2 { get; set; }

    public decimal? FinalRVOpenedPSID2 { get; set; }

    [StringLength(50)]
    public string? MeterNumber { get; set; }

    public bool MeterRegisters { get; set; }
    public decimal? MeterReadingBefore { get; set; }
    public decimal? MeterReadingAfter { get; set; }

    public bool AirGapValid { get; set; }

    public bool Ossf { get; set; }
    public bool RainFreezeSensorInstalled { get; set; }
    public bool RainFreezeSensorWorkingProperly { get; set; }

    [StringLength(50)]
    public string? PermitNumber { get; set; }

    public string? Comments { get; set; }
}
