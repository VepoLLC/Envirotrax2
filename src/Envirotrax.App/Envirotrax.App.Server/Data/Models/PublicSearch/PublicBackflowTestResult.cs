
namespace Envirotrax.App.Server.Data.Models.PublicSearch;

public class PublicBackflowTestResult
{
    public int Id { get; set; }

    public DateTime? TestDate { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public bool RenewalRequired { get; set; }

    public string? Manufacturer { get; set; }

    public string? Model { get; set; }

    public string? Size { get; set; }

    public string? DeviceType { get; set; }

    public string? SerialNumber { get; set; }

    public string? HazardType { get; set; }

    public string? HazardTypeOtherDescription { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public string? PropertyState { get; set; }

    public string? PropertyZip { get; set; }
}
