

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class MySupplierHierarchyDto
{
    public ReferencedWaterSupplierDto? AdminAccount { get; set; }

    public IEnumerable<WaterSupplierHierarchyDto> Hierarchy { get; set; } = null!;
}

public class WaterSupplierHierarchyDto
{
    public string? GroupLetter { get; set; }

    public IEnumerable<WaterSupplierHierarchyChildDto> WaterSuppliers { get; set; } = [];
}

public class WaterSupplierHierarchyChildDto
{
    public ReferencedWaterSupplierDto WaterSupplier { get; set; } = null!;

    public IEnumerable<WaterSupplierHierarchyDto> Children { get; set; } = [];
}