namespace Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;

public class WaterSupplierGaugeDto : IDto
{
    public int Id { get; set; }
    public int ProfessionalId { get; set; }
    public DateTime SubmittedOn { get; set; }
    public string? UserEmail { get; set; }
    public string? CompanyName { get; set; }
    public string Manufacturer { get; set; } = null!;
    public string Model { get; set; } = null!;
    public string SerialNumber { get; set; } = null!;
    public bool IsPortable { get; set; }
}
