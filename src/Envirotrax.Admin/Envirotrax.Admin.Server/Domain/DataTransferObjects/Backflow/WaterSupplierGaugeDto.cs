namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

public class WaterSupplierGaugeDto
{
    public int Id { get; set; }

    public int ProfessionalId { get; set; }

    public DateTime SubmittedOn { get; set; }

    public string? UserEmail { get; set; }

    public string? CompanyName { get; set; }

    public string? ContactName { get; set; }

    public string Manufacturer { get; set; } = null!;

    public string Model { get; set; } = null!;

    public string SerialNumber { get; set; } = null!;

    public DateTime? LastCalibrationDate { get; set; }

    public bool IsPortable { get; set; }

    public bool HasFile { get; set; }
}
