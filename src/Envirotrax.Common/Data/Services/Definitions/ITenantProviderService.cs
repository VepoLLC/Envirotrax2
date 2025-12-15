

using System.Security.Claims;

namespace Envirotrax.Common.Data.Services.Definitions
{
    public interface ITenantProvidersService
    {
        int WaterSupplierId { get; }
        int ParentWaterSupplierId { get; }

        string Domain { get; }

        int UserId { get; }

        int ContractorId { get; }

        int ParentContractorId { get; }

        void SetWaterSupplierId(int supplierId);
        void SetWaterSupplier(ClaimsPrincipal principal, int supplierId);

        void SetParentWaterSupplier(ClaimsPrincipal principal, int supplierId);

        void SetDomain(ClaimsPrincipal principal, string domain);
    }
}