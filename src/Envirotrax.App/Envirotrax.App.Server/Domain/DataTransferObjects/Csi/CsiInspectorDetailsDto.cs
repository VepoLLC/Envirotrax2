using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals.Licenses;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Csi;

public class CsiInspectorDetailsDto
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

    public List<ProfessionalWaterSupplierDto> WaterSuppliers { get; set; } = new();

    public List<ProfessionalUserDto> SubAccounts { get; set; } = new();

    public List<ProfessionalUserLicenseDto> Licenses { get; set; } = new();
}
