
using Envirotrax.Common;
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.Auth.Data.Models;

[ReadOnlyModel]
[ExcludedModel]
public class WaterSupplierFeature
{
    [AppPrimaryKey(false)]
    public int WaterSupplierId { get; set; }

    [AppPrimaryKey(false)]
    public FeatureType FeatureId { get; set; }
}