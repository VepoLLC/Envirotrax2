using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

public class BackflowTestDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public string? AccountNumber { get; set; }

    public DateTime CreatedTime { get; set; }

    public DateTime? TestDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public int TestResult { get; set; }

    public bool IsCurrent { get; set; }

    public bool OutOfService { get; set; }

    public bool Disapproved { get; set; }

    public bool Rejected { get; set; }

    public string? TransactionId { get; set; }

    public string? SerialNumber { get; set; }

    public string? SerialNumber2 { get; set; }

    public string? Manufacturer { get; set; }

    public string? Model { get; set; }

    public string? Size { get; set; }

    public string? DeviceType { get; set; }

    public string? HazardType { get; set; }

    public string? LocationDescription { get; set; }

    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }

    public string? BpatCompanyName { get; set; }

    public string? BpatContactName { get; set; }

    public string? BpatLicenseNumber { get; set; }
}
