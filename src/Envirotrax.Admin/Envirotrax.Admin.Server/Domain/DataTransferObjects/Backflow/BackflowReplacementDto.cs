using Envirotrax.Admin.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.Backflow;

public class BackflowReplacementDto
{
    public int Id { get; set; }

    public ReferencedWaterSupplierDto? WaterSupplier { get; set; }

    public ReferencedBackflowSiteDto? Site { get; set; }

    public bool ValidationReplacementOnHold { get; set; }

    public bool ValidationReplacementCleared { get; set; }

    public string? ReplacementAssembly { get; set; }

    public string? DeviceType { get; set; }

    public string? Manufacturer { get; set; }

    public string? Model { get; set; }

    public string? Size { get; set; }

    public string? SerialNumber { get; set; }

    public int PropertyType { get; set; }

    public string? PropertyBusinessName { get; set; }

    public string? PropertyStreetNumber { get; set; }

    public string? PropertyStreetName { get; set; }

    public string? PropertyNumber { get; set; }

    public string? PropertyCity { get; set; }

    public StateDto? PropertyState { get; set; }

    public string? PropertyZip { get; set; }
}
