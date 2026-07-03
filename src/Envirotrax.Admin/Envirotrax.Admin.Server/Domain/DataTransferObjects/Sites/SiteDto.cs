
using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Sites;

public class SiteDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public string? AccountNumber { get; set; }

    public string? BusinessName { get; set; }

    public int PropertyType { get; set; }

    public string? StreetNumber { get; set; }

    public string? StreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? City { get; set; }

    public StateDto? State { get; set; }

    public bool Active { get; set; }

    public bool OutOfArea { get; set; }

    public bool IsFeeExempt { get; set; }
}
