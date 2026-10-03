
namespace Envirotrax.App.Server.Domain.DataTransferObjects.PublicSearch;

public class PublicSearchWaterSuppliersDto
{
    public IReadOnlyList<PublicSearchWaterSupplierDto> Suppliers { get; set; } = [];

    public int? SelectedWaterSupplierId { get; set; }
}

public class PublicSearchWaterSupplierDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;
}
