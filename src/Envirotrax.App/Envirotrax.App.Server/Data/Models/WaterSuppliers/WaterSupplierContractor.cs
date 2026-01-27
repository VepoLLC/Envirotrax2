
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class WaterSupplierProfessional : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int ProfessionalId { get; set; }
    public Professional? Professional { get; set; }
}