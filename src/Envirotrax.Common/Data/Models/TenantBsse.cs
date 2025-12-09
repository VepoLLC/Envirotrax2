
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.Common.Data.Models
{
    [ExcludedModel]
    [ReadOnlyModel]
    public class TenantBase
    {
        [AppPrimaryKey(true)]
        public int Id { get; set; }

        public int? ParentId { get; set; }
    }
}