
using System.ComponentModel.DataAnnotations;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

public class WaterSupplierHierarchyDto
{
    public string? GroupLetter { get; set; }

    [Required]
    public ReferencedWaterSupplierDto WaterSupplier { get; set; } = null!;

    public IEnumerable<ReferencedWaterSupplierDto> Children { get; set; } = [];
}
