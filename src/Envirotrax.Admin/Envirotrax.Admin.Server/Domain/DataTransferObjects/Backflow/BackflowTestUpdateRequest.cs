using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestUpdateRequest
{
    public bool IsCurrent { get; set; }
    public bool OutOfService { get; set; }
    public bool Disapproved { get; set; }
    public bool Rejected { get; set; }
    public bool NeedsValidation { get; set; }
    public string? ValidationNotes { get; set; }
    public bool ForceRenewal { get; set; }
    public int ForceRenewalYears { get; set; }
    public int BackflowScheduleMonth { get; set; }
    public bool RenewalRequired { get; set; }

    public DateTime? TestDate { get; set; }
    public DateTime? ExpirationDate { get; set; }

    public string? TransactionId { get; set; }
    public DateTime? TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public decimal AmountShare { get; set; }

    public int PropertyType { get; set; }
    public string? PropertyBusinessName { get; set; }
    public string? PropertyStreetNumber { get; set; }
    public string? PropertyStreetName { get; set; }
    public string? PropertyNumber { get; set; }
    public string? PropertyCity { get; set; }
    public StateDto? PropertyState { get; set; }
    public string? PropertyZip { get; set; }

    public string? MailingCompanyName { get; set; }
    public string? MailingContactName { get; set; }
    public string? MailingStreetNumber { get; set; }
    public string? MailingStreetName { get; set; }
    public string? MailingNumber { get; set; }
    public string? MailingCity { get; set; }
    public StateDto? MailingState { get; set; }
    public string? MailingZip { get; set; }

    public string? DeviceType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Size { get; set; }
    public string? SerialNumber { get; set; }
    public string? Manufacturer2 { get; set; }
    public string? Model2 { get; set; }
    public string? Size2 { get; set; }
    public string? SerialNumber2 { get; set; }
    public string? LocationDescription { get; set; }
    public string? HazardType { get; set; }
    public string? HazardTypeOtherDescription { get; set; }

    public int TestResult { get; set; }
    public string? JobNumber { get; set; }
    public int ReasonForTest { get; set; }
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

    public string? RepairCV1Details { get; set; }
    public string? RepairCV2Details { get; set; }
    public string? RepairRVDetails { get; set; }
    public string? RepairBCDetails { get; set; }
    public string? RepairCV1Details2 { get; set; }
    public string? RepairCV2Details2 { get; set; }
    public string? RepairRVDetails2 { get; set; }
    public string? RepairPvbAirInletDetails { get; set; }
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

    public string? MeterNumber { get; set; }
    public bool MeterRegisters { get; set; }
    public decimal? MeterReadingBefore { get; set; }
    public decimal? MeterReadingAfter { get; set; }

    public bool AirGapValid { get; set; }

    public bool Ossf { get; set; }
    public bool RainFreezeSensorInstalled { get; set; }
    public bool RainFreezeSensorWorkingProperly { get; set; }
    public string? PermitNumber { get; set; }

    public string? Comments { get; set; }
}
