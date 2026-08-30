using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogVehiclePermitSearchDto : IDto
{
    public int Id { get; set; }

    public ProfessionalDto? Professional { get; set; }

    public string LicensePlateNumber { get; set; } = null!;
    public string Manufacturer { get; set; } = null!;
    public int ManufacturedYear { get; set; }
    public double Capacity { get; set; }
    public FogVehicleCapacityType CapacityType { get; set; }
    public string StickerNumber { get; set; } = null!;

    public FogVehiclePermitDto? Permit { get; set; }

    public FogVehicleInspectionDueStatus InspectionDueStatus { get; set; }
}
