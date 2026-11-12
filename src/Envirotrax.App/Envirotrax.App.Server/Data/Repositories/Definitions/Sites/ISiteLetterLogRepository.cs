using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;

namespace Envirotrax.App.Server.Data.Repositories.Definitions.Sites;

public interface ISiteLetterLogRepository : IRepository<SiteLetterLog>
{
    Task AddRangeAsync(List<SiteLetterLog> letterLogs);

    Task<List<DateTime>> GetProcessDatesAsync(CancellationToken cancellationToken);

    Task<List<LetterFileDto>> GetFilesAsync(DateTime processDate, CancellationToken cancellationToken);

    Task<List<LetterEmailDto>> GetEmailsAsync(DateTime processDate, CancellationToken cancellationToken);

    Task<string?> GetFilePathAsync(DateTime processDate, string fileName, CancellationToken cancellationToken);

    Task<List<string>> GetFilePathsAsync(DateTime processDate, CancellationToken cancellationToken);

    Task<int> DeleteByProcessDateAsync(DateTime processDate);
}
