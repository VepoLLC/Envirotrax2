namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

/// <summary>A gauge row on the Test Gauges queue. Gauges always belong to BPATs.</summary>
public class WaterSupplierGaugeDto : IDto
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public string? CompanyName { get; set; }
    public string? CompanyEmail { get; set; }
    public string Manufacturer { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public DateTime? LastCalibrationDate { get; set; }
}
