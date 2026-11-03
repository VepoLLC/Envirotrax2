using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Letters;

public interface ILetterRepository
{
    Task<List<LetterBackflowTestDto>> GetBackflowTestsAsync(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken);

    Task<List<LetterSiteDto>> GetBackflowSitesAsync(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken);

    Task<List<LetterSiteDto>> GetUnknownAssembliesSitesAsync(int waterSupplierId, DateTime lastModifiedBefore, CancellationToken cancellationToken);

    Task<List<LetterSiteDto>> GetCsiSitesAsync(int waterSupplierId, DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken);

    Task UpdateOptInCodesAsync(List<LetterOptInCodeDto> optInCodes, DateTime optInCodeExpirationDate);

    Task IncrementUnknownAssemblyLettersSentAsync(List<int> siteIds);
}
