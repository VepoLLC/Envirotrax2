using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface IInsuranceValidationService
{
    Task<InsuranceValidationDto> ValidateAsync(int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken);

    /// <summary>Throws AppValidationException unless the policy is Valid or NotRequired.</summary>
    Task EnsureValidAsync(int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken);
}
