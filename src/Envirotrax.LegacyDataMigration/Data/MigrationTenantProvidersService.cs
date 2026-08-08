
using System.Security.Claims;
using Envirotrax.Common.Data.Services.Definitions;

namespace Envirotrax.LegacyDataMigration.Data;

public class MigrationTenantProvidersService : ITenantProvidersService
{
    public int WaterSupplierId => 0;
    public int ParentWaterSupplierId => 0;
    public string Domain => string.Empty;
    public int UserId => 0;
    public int ProfessionalId => 0;
    public int ParentProfessionalId => 0;

    public void SetWaterSupplierId(int supplierId)
    {
    }

    public void SetWaterSupplier(ClaimsPrincipal principal, int supplierId)
    {
    }

    public void SetParentWaterSupplier(ClaimsPrincipal principal, int supplierId)
    {
    }

    public void SetProfessional(ClaimsPrincipal principal, int professionalId)
    {
    }

    public void SetDomain(ClaimsPrincipal principal, string domain)
    {
    }

    public bool HasScope(string scopeToCheck) => false;

    public bool HasAnyScopes(params string[] scopesToCheck) => false;
}
