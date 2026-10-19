using Envirotrax.App.Server.Data.Models.Professionals.Licenses;
using Envirotrax.App.Server.Domain.DataTransferObjects.Sites;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Professionals;

public interface ISubmissionWaterSupplierService
{
    Task<int> ResolveAsync(SiteDto? site, int? requestedWaterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken);

    Task EnsureCanSubmitAsync(int waterSupplierId, ProfessionalType professionalType, CancellationToken cancellationToken);
}
