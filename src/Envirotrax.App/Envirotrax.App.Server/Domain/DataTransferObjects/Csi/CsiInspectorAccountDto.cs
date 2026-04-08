using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectorAccountDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? CompanyEmail { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public ReferencedStateDto? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }
    public DateTime CreatedTime { get; set; }
}
