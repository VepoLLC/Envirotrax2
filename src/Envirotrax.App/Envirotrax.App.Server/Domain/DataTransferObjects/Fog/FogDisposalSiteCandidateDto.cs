using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogDisposalSiteCandidateDto : IDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public string County { get; set; } = null!;
    public PhysicalType PhysicalType { get; set; }
    public bool IsActive { get; set; }
}
