namespace Envirotrax.App.Server.Data.Models.Fog;

public class FogDisposalSiteCandidate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public string County { get; set; } = null!;
    public PhysicalType PhysicalType { get; set; }
    public bool IsActive { get; set; }
}
