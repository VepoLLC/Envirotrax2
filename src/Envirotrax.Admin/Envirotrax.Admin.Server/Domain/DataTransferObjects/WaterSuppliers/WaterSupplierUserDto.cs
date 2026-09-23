
namespace Envirotrax.Admin.Server.Domain.DataTransferObjects.WaterSuppliers;

public class WaterSupplierUserDto
{
    public string ContactName { get; set; } = null!;

    public string EmailAddress { get; set; } = null!;

    public string? CellNumber { get; set; }

    public IEnumerable<ReferencedRoleDto> Roles { get; set; } = [];
}

public class ReferencedRoleDto
{
    public string? Name { get; set; }
}
