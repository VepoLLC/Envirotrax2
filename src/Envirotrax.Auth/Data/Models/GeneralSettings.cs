
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.Auth.Data.Models;

[ReadOnlyModel]
[ExcludedModel]
public class GeneralSettings : TenantModel<WaterSupplier>
{
    public bool WiseGuys { get; set; }
    public bool BackflowTesting { get; set; }
    public bool CsiInspections { get; set; }
    public bool FogProgram { get; set; }
}