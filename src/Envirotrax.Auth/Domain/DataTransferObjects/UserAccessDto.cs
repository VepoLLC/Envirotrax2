
namespace Envirotrax.Auth.Domain.DataTransferObjects;

public class UserAccessDto
{
    public int? WaterSupplierId { get; set; }

    /// <summary>
    /// User may belong to a parent water supplier and request access to a child water supplier. In that case, this property value will be the child's ID
    /// </summary>
    public int? WaterSupplierIdRequested { get; set; }

    public int? ProfessionalId { get; set; }

    /// <summary>
    /// User may belong to a parent professional and request access to a child professional. in that case, this property value will be the child's ID
    /// </summary>
    public int? ProfessionalIdRequested { get; set; }
}