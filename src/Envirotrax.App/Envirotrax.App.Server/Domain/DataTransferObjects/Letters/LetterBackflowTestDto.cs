namespace Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

public class LetterBackflowTestDto
{
    public int Id { get; set; }
    public int SiteId { get; set; }

    public string? DeviceType { get; set; }
    public string? Manufacturer { get; set; }
    public string? Model { get; set; }
    public string? Size { get; set; }
    public string? SerialNumber { get; set; }
    public string? LocationDescription { get; set; }
    public string? HazardType { get; set; }

    public string? BpatCompanyName { get; set; }
    public string? BpatWorkNumber { get; set; }

    public string? TransactionId { get; set; }
    public DateTime? TestDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
}
