using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Letters;

public interface ILetterRepository
{
    Task<List<LetterBackflowTestDto>> GetRenewalRequirementTestsAsync(int waterSupplierId, BackflowRenewalRequirementDto requirement, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken);

    Task<List<LetterBackflowTestDto>> GetForcedRenewalTestsAsync(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken);

    Task<List<LetterSiteDto>> GetSitesAsync(List<int> siteIds, CancellationToken cancellationToken);

    Task<List<LetterSiteDto>> GetUnknownAssembliesSitesAsync(int waterSupplierId, DateTime lastModifiedBefore, CancellationToken cancellationToken);

    Task<List<LetterSiteDto>> GetCsiSitesAsync(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken);

    Task UpdateOptInCodeAsync(int siteId, string optInCodeHash, DateTime optInCodeExpirationDate);

    Task IncrementUnknownAssemblyLettersSentAsync(List<int> siteIds);
}
