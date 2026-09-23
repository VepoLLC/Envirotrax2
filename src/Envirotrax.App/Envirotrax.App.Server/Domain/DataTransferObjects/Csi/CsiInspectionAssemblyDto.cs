using Envirotrax.App.Server.Data.Models.Backflow;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectionAssemblyDto : IDto
{
    public int Id { get; set; }

    public int InspectionId { get; set; }

    public int? TestId { get; set; }

    public bool VisuallyIdentified { get; set; }

    public string? DeviceType { get; set; }

    public string? AssemblyDescription { get; set; }

    public string? SerialNumber { get; set; }

    public string? AssemblyDescription2 { get; set; }

    public string? SerialNumber2 { get; set; }

    public string? HazardType { get; set; }

    public string? HazardTypeOtherDescription { get; set; }

    public string? LocationDescription { get; set; }

    public bool IsCurrent { get; set; }

    public BackflowTestResult TestResult { get; set; }

    public bool OutOfService { get; set; }

    public DateTime? TestDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? TransactionId { get; set; }

    public bool Disapproved { get; set; }

    public bool Rejected { get; set; }
}
