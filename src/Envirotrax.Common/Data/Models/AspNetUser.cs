
using Envirotrax.Common.Data.Attributes;

namespace Envirotrax.Common.Data.Models
{
    [ReadOnlyModel]
    [ExcludedModel]
    public class AspNetUserBase : IAspNetUserBase
    {
        public int Id { get; set; }

        public string? NormalizedEmail { get; set; } = null!;
    }

    public interface IAspNetUserBase
    {
        int Id { get; set; }

        string? NormalizedEmail { get; set; }
    }
}