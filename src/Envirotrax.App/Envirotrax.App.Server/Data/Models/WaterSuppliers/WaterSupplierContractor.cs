
using Envirotrax.App.Server.Data.Models.Contractors;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers;

public class WaterSupplierContractor : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public int ContractorId { get; set; }
    public Contractor? Contractor { get; set; }
}