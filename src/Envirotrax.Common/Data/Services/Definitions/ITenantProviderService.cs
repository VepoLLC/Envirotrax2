

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

        /// <summary>
        /// Remote address of the current request, or null outside a request (background jobs, seeding).
        /// </summary>
        string? IpAddress { get; }

        void SetWaterSupplierId(int supplierId);
        void SetWaterSupplier(ClaimsPrincipal principal, int supplierId);

        void SetParentWaterSupplier(ClaimsPrincipal principal, int supplierId);

        void SetProfessional(ClaimsPrincipal principal, int professionalId);

        void SetDomain(ClaimsPrincipal principal, string domain);

        bool HasScope(string scopeToCheck);
        bool HasAnyScopes(params string[] scopesToCheck);
    }
}