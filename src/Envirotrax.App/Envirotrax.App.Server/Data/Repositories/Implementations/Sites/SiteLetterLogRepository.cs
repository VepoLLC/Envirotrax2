using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Sites;

public class SiteLetterLogRepository : Repository<SiteLetterLog>, ISiteLetterLogRepository
{
    public SiteLetterLogRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    public async Task AddRangeAsync(List<SiteLetterLog> letterLogs)
    {
        Entity.AddRange(letterLogs);

        await DbContext.SaveChangesAsync();
    }

    public async Task<List<DateTime>> GetProcessDatesAsync(CancellationToken cancellationToken)
    {
        return await Entity
            .Select(letterLog => letterLog.ProcessDate)
            .Distinct()
            .OrderByDescending(processDate => processDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LetterFileDto>> GetFilesAsync(DateTime processDate, CancellationToken cancellationToken)
    {
        return await Entity
            .Where(letterLog => letterLog.ProcessDate == processDate && letterLog.Page != SiteLetterLog.EmailPage)
            .GroupBy(letterLog => letterLog.FileName)
            .OrderBy(group => group.Key)
            .Select(group => new LetterFileDto
            {
                FileName = group.Key,
                LetterCount = group.Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LetterEmailDto>> GetEmailsAsync(DateTime processDate, CancellationToken cancellationToken)
    {
        return await Entity
            .Where(letterLog => letterLog.ProcessDate == processDate && letterLog.Page == SiteLetterLog.EmailPage)
            .OrderBy(letterLog => letterLog.FileName)
            .Select(letterLog => new LetterEmailDto
            {
                FileName = letterLog.FileName,
                AccountNumber = letterLog.Site!.AccountNumber,
                StreetNumber = letterLog.Site.StreetNumber,
                StreetName = letterLog.Site.StreetName
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<string?> GetFilePathAsync(DateTime processDate, string fileName, CancellationToken cancellationToken)
    {
        return await Entity
            .Where(letterLog => letterLog.ProcessDate == processDate && letterLog.FileName == fileName)
            .Select(letterLog => letterLog.FilePath)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<string>> GetFilePathsAsync(DateTime processDate, CancellationToken cancellationToken)
    {
        return await Entity
            .Where(letterLog => letterLog.ProcessDate == processDate)
            .Select(letterLog => letterLog.FilePath)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<int> DeleteByProcessDateAsync(DateTime processDate)
    {
        return await Entity
            .Where(letterLog => letterLog.ProcessDate == processDate)
            .ExecuteDeleteAsync();
    }

    // IgnoreQueryFilters: the tenant filter matches the supplier of the letter's site, not the supplier that
    // generated the file. Scoping is restored from the generating supplier's ID, which prefixes every file it generates.
    public async Task<List<SiteLetterLogFile>> GetGeneratedPdfFilesAsync(int generatingSupplierId, CancellationToken cancellationToken)
    {
        var fileNamePattern = GetGeneratedFileNamePattern(generatingSupplierId);

        return await Entity
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(letterLog => letterLog.Page != SiteLetterLog.EmailPage
                && EF.Functions.Like(letterLog.FileName, fileNamePattern))
            .GroupBy(letterLog => new { letterLog.ProcessDate, letterLog.FileName, letterLog.FilePath })
            .Select(group => new SiteLetterLogFile
            {
                ProcessDate = group.Key.ProcessDate,
                FileName = group.Key.FileName,
                FilePath = group.Key.FilePath,
                LetterCount = group.Count()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<string?> GetGeneratedPdfFilePathAsync(int generatingSupplierId, DateTime processDate, string fileName, CancellationToken cancellationToken)
    {
        var fileNamePattern = GetGeneratedFileNamePattern(generatingSupplierId);

        return await Entity
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(letterLog => letterLog.ProcessDate == processDate
                && letterLog.FileName == fileName
                && letterLog.Page != SiteLetterLog.EmailPage
                && EF.Functions.Like(letterLog.FileName, fileNamePattern))
            .Select(letterLog => letterLog.FilePath)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static string GetGeneratedFileNamePattern(int generatingSupplierId)
    {
        return $"{generatingSupplierId} - %";
    }
}
