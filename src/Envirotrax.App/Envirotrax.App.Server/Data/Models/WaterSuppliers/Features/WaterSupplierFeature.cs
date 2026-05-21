
using Envirotrax.Common;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.WaterSuppliers.Features;

public class WaterSupplierFeature : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(false)]
    public FeatureType FeatureId { get; set; }
    public Feature? Feature { get; set; }
}