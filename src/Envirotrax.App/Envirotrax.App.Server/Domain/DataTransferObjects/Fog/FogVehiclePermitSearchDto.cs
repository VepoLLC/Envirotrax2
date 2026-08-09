using Envirotrax.App.Server.Data.Models.Fog;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogVehiclePermitSearchDto : IDto
{
    public int Id { get; set; }

    public int TransporterId { get; set; }
    public string TransporterCompanyName { get; set; } = null!;
    public string? TransporterAddress { get; set; }
    public string? TransporterCity { get; set; }
    public string? TransporterState { get; set; }
    public string? TransporterZip { get; set; }
    public string? TransporterPhoneNumber { get; set; }
    public string? TransporterFaxNumber { get; set; }
    public string? TransporterEmailAddress { get; set; }

    public string LicensePlateNumber { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public int ManufacturedYear { get; set; }
    public double Capacity { get; set; }
    public FogVehicleCapacityType CapacityType { get; set; }
    public string StickerNumber { get; set; } = null!;

    public bool HasPermit { get; set; }
    public string? PermitNumber { get; set; }
    public DateTime? InspectionDueDate { get; set; }
    public bool? IsActive { get; set; }

    public FogVehicleInspectionDueStatus InspectionDueStatus { get; set; }
}
