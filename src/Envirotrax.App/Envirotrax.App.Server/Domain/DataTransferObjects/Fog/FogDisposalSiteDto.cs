using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogDisposalSiteDto : IDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public ReferencedStateDto? State { get; set; }
    public string? ZipCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string County { get; set; } = null!;
    public string TceqRegion { get; set; } = null!;
    public string RegistrationNumber { get; set; } = null!;
    public string PermitNumber { get; set; } = null!;
    public PhysicalType PhysicalType { get; set; }
    public string? LocationDescription { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
