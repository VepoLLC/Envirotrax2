
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.Common.Data.Models
{
    public class TenantModel<TTennant> : ITenantModel
        where TTennant : TenantBase
    {
        [AppPrimaryKey(false, CompositeKeyOrder = int.MinValue, IsShadowKey = true)]
        public int WaterSupplierId { get; set; }
        public TTennant? WaterSupplier { get; set; }
    }
}