

using System.Security.Claims;

namespace Envirotrax.Common.Data.Services.Definitions
{
    public interface ITenantProvidersService
    {
        int WaterSupplierId { get; }
        int ParentWaterSupplierId { get; }

        string Domain { get; }

        int UserId { get; }

        int ProfessionalId { get; }

        int ParentProfessionalId { get; }

        void SetWaterSupplierId(int supplierId);
        void SetWaterSupplier(ClaimsPrincipal principal, int supplierId);

        void SetParentWaterSupplier(ClaimsPrincipal principal, int supplierId);

        void SetProfessional(ClaimsPrincipal principal, int professionalId);

        void SetDomain(ClaimsPrincipal principal, string domain);
    }
}