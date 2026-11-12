using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Letters;
using Envirotrax.App.Server.Data.Repositories.Definitions.Sites;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions;
using Envirotrax.App.Server.Domain.Services.Definitions.Letters;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Templates.Emails.Letters;
using Envirotrax.Common.Configuration;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.Extensions.Options;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Letters;

public class LetterGeneratorService : ILetterGeneratorService
{
    private const int LettersPerFile = 1000;
    private const int LettersPerChunk = 100;
    private const int OptInCodeLength = 6;
    private const int OptInCodeValidDays = 30;
    private const int UnknownAssembliesWaitDays = 15;
    private const int MaxAssemblyTextLength = 37;

    private const string BackflowFilePrefix = "BF";
    private const string CsiFilePrefix = "CSI";
    private const string LettersFolder = "letters";
    private const string EmailsFolder = "Emails";
    private const string PdfTemplateName = "Letters.Letters";
    private const string PageSiteIdsScript = "Array.from(document.querySelectorAll('.letter-page'), page => Number(page.dataset.siteId))";
    private const string EmailTemplateName = "Letters.Letter";
    private const string EmailSubject = "Important Notice from Your Water Supplier – Action May Be Required";

    private const string DefaultFontFamily = "Arial";
    private const int DefaultFontSize = 12;
    private const string DateFormat = "M/d/yyyy";

    private const string AllValue = "All";
    private const string FireSystemHazardType = "Fire System";
    private const string ImportTransactionId = "import";
    private const string ManualTransactionId = "manual";

    private const string ResidentialDefaultName = "Current Resident";
    private const string CommercialDefaultName = "Owner Or Owner's Representative";
    private const string ResidentWord = "resident";
    private const string ResidentSuffix = " or Current Resident";

    private static readonly TimeZoneInfo CentralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
    private static readonly Regex StyleTagRegex = new(@"(\[/?[biu]\])", RegexOptions.IgnoreCase);
    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();

    private readonly ILetterRepository _letterRepository;
    private readonly ISiteLetterLogRepository _siteLetterLogRepository;
    private readonly IWaterSupplierService _waterSupplierService;
    private readonly IBackflowRenewalRequirementService _renewalRequirementService;
    private readonly IPdfTemplateService _pdfTemplateService;
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmailService _emailService;
    private readonly IHtmlTemplateService _htmlTemplateService;
    private readonly PdfTemplateOptions _pdfTemplateOptions;

    public LetterGeneratorService(
        ILetterRepository letterRepository,
        ISiteLetterLogRepository siteLetterLogRepository,
        IWaterSupplierService waterSupplierService,
        IBackflowRenewalRequirementService renewalRequirementService,
        IPdfTemplateService pdfTemplateService,
        IFileStorageService fileStorageService,
        IEmailService emailService,
        IHtmlTemplateService htmlTemplateService,
        IOptions<PdfTemplateOptions> pdfTemplateOptions)
    {
        _letterRepository = letterRepository;
        _siteLetterLogRepository = siteLetterLogRepository;
        _waterSupplierService = waterSupplierService;
        _renewalRequirementService = renewalRequirementService;
        _pdfTemplateService = pdfTemplateService;
        _fileStorageService = fileStorageService;
        _emailService = emailService;
        _htmlTemplateService = htmlTemplateService;
        _pdfTemplateOptions = pdfTemplateOptions.Value;
    }

    public async Task<GeneratedLettersDto?> GenerateAsync(int waterSupplierId, GenerateLettersDto generateLetters)
    {
        var details = await _waterSupplierService.GetDetailsAsync(waterSupplierId, CancellationToken.None);

        if (details == null)
        {
            return null;
        }

        var renewalRequirements = await _renewalRequirementService.GetAllByWaterSupplierIdAsync(waterSupplierId, CancellationToken.None);
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CentralTimeZone);

        var context = new GenerationContext
        {
            Details = details,
            RenewalRequirements = renewalRequirements.ToList(),
            Now = now,
            ProcessDate = GetProcessDate(generateLetters, now),
            OptInCodeExpirationDate = DateTime.UtcNow.AddDays(OptInCodeValidDays)
        };

        await GenerateBackflowLettersAsync(context);
        await GenerateCsiLettersAsync(context);

        await SaveOptInCodesAsync(context);
        await SaveUnknownAssemblyLettersSentAsync(context);

        await SendLetterEmailsAsync(context);
        await AddPdfFilesAsync(context);

        return new GeneratedLettersDto
        {
            ProcessDate = context.ProcessDate,
            Counts = context.Counts,
            EmailCount = context.Emails.Count
        };
    }

    private static DateTime GetProcessDate(GenerateLettersDto generateLetters, DateTime now)
    {
        var processDate = now;

        if (generateLetters.ProcessDate != null)
        {
            processDate = generateLetters.ProcessDate.Value;
        }

        return new DateTime(processDate.Year, processDate.Month, processDate.Day, processDate.Hour, processDate.Minute, processDate.Second);
    }

    private async Task GenerateBackflowLettersAsync(GenerationContext context)
    {
        var settings = context.Details.BackflowSettings;

        if (settings.ExpiringNotice1 != BackflowExpiringType.Off)
        {
            context.Counts.ExpiringNotice1 = await GenerateExpiringLettersAsync(context, 1, settings.ExpiringNotice1);
        }

        if (settings.ExpiringNotice2 != BackflowExpiringType.Off)
        {
            context.Counts.ExpiringNotice2 = await GenerateExpiringLettersAsync(context, 2, settings.ExpiringNotice2);
        }

        if (settings.ExpiredNotice1 != BackflowExpiredType.Off)
        {
            context.Counts.ExpiredNotice1 = await GenerateExpiredLettersAsync(context, 1, settings.ExpiredNotice1);
        }

        if (settings.ExpiredNotice2 != BackflowExpiredType.Off)
        {
            context.Counts.ExpiredNotice2 = await GenerateExpiredLettersAsync(context, 2, settings.ExpiredNotice2);
        }

        if (settings.BackflowNonCompliant1 != BackflowNonCompliantType.Off)
        {
            context.Counts.BackflowNonCompliant1 = await GenerateBackflowNonCompliantLettersAsync(context, 1, settings.BackflowNonCompliant1, settings.NonCompliantPrint1);
        }

        if (settings.BackflowNonCompliant2 != BackflowNonCompliantType.Off)
        {
            context.Counts.BackflowNonCompliant2 = await GenerateBackflowNonCompliantLettersAsync(context, 2, settings.BackflowNonCompliant2, settings.NonCompliantPrint2);
        }

        if (settings.UnknownAssembliesNotice)
        {
            context.Counts.UnknownAssembliesNotice = await GenerateUnknownAssembliesLettersAsync(context);
        }
    }

    private async Task GenerateCsiLettersAsync(GenerationContext context)
    {
        var settings = context.Details.CsiSettings;

        if (settings.ImpendingNotice1 != CsiImpendingType.Off)
        {
            context.Counts.ImpendingNotice1 = await GenerateImpendingLettersAsync(context, 1, settings.ImpendingNotice1);
        }

        if (settings.ImpendingNotice2 != CsiImpendingType.Off)
        {
            context.Counts.ImpendingNotice2 = await GenerateImpendingLettersAsync(context, 2, settings.ImpendingNotice2);
        }

        if (settings.PastDueNotice1 != CsiPastDueType.Off)
        {
            context.Counts.PastDueNotice1 = await GeneratePastDueLettersAsync(context, 1, settings.PastDueNotice1);
        }

        if (settings.PastDueNotice2 != CsiPastDueType.Off)
        {
            context.Counts.PastDueNotice2 = await GeneratePastDueLettersAsync(context, 2, settings.PastDueNotice2);
        }

        if (settings.NonCompliant1 != CsiNonCompliantType.Off)
        {
            context.Counts.CsiNonCompliant1 = await GenerateCsiNonCompliantLettersAsync(context, 1, settings.NonCompliant1, settings.NonCompliantPrint1);
        }

        if (settings.NonCompliant2 != CsiNonCompliantType.Off)
        {
            context.Counts.CsiNonCompliant2 = await GenerateCsiNonCompliantLettersAsync(context, 2, settings.NonCompliant2, settings.NonCompliantPrint2);
        }
    }

    private async Task<int> GenerateExpiringLettersAsync(GenerationContext context, int noticeNumber, BackflowExpiringType expiringType)
    {
        var settings = context.Details.BackflowSettings;
        var period = LetterPeriod.ForExpiring(context.Now, expiringType);

        var seguinText1 = string.Empty;
        var seguinText2 = string.Empty;

        if (expiringType == BackflowExpiringType.TwoMonths)
        {
            seguinText1 = SeguinLetterTexts.ExpiringInTwoMonths1;
            seguinText2 = SeguinLetterTexts.ExpiringInTwoMonths2;
        }
        else if (expiringType == BackflowExpiringType.NextMonth)
        {
            seguinText1 = SeguinLetterTexts.ExpiringNextMonth1;
            seguinText2 = SeguinLetterTexts.ExpiringNextMonth2;
        }

        var message = ReplaceTokens(context, settings.ExpiringMessage);
        message = ReplaceSeguinTokens(context, message, seguinText1, seguinText2);

        var content = CreateContent(context, settings.NoticeBodyFont, settings.NoticeBodyFontSize);
        content.Title = settings.ExpiringTitle;
        content.MessageLines = ParseMessageLines(message);
        content.HeaderBackgroundColor = settings.ExpiringLettersBackgroundColor;
        content.HeaderForegroundColor = settings.ExpiringLettersForegroundColor;
        content.HeaderBorderColor = settings.ExpiringLettersBorderColor;
        content.ShowAssemblies = true;

        var sites = await GetBackflowLetterSitesAsync(context, period);
        var fileTypeName = $"{BackflowFilePrefix} - Expiring{noticeNumber} {period.Name}";

        return AddLetters(context, content, sites, fileTypeName, true);
    }

    private async Task<int> GenerateExpiredLettersAsync(GenerationContext context, int noticeNumber, BackflowExpiredType expiredType)
    {
        var settings = context.Details.BackflowSettings;
        var period = LetterPeriod.ForExpired(context.Now, expiredType);

        var seguinText1 = string.Empty;
        var seguinText2 = string.Empty;

        if (expiredType == BackflowExpiredType.ThisMonth)
        {
            seguinText1 = SeguinLetterTexts.ExpiredThisMonth1;
            seguinText2 = SeguinLetterTexts.ExpiredThisMonth2;
        }

        var message = ReplaceTokens(context, settings.ExpiredMessage);
        message = ReplaceSeguinTokens(context, message, seguinText1, seguinText2);

        var content = CreateContent(context, settings.NoticeBodyFont, settings.NoticeBodyFontSize);
        content.Title = settings.ExpiredTitle;
        content.MessageLines = ParseMessageLines(message);
        content.HeaderBackgroundColor = settings.ExpiredLettersBackgroundColor;
        content.HeaderForegroundColor = settings.ExpiredLettersForegroundColor;
        content.HeaderBorderColor = settings.ExpiredLettersBorderColor;
        content.ShowAssemblies = true;

        var sites = await GetBackflowLetterSitesAsync(context, period);
        var fileTypeName = $"{BackflowFilePrefix} - Expired{noticeNumber} {period.Name}";

        return AddLetters(context, content, sites, fileTypeName, true);
    }

    private async Task<int> GenerateBackflowNonCompliantLettersAsync(GenerationContext context, int noticeNumber, BackflowNonCompliantType nonCompliantType, bool isPrinted)
    {
        var settings = context.Details.BackflowSettings;
        var period = LetterPeriod.ForNonCompliant(context.Now, nonCompliantType);

        var message = ReplaceTokens(context, settings.NonCompliantMessage);
        message = ReplaceSeguinTokens(context, message, string.Empty, string.Empty);

        var content = CreateContent(context, settings.NoticeBodyFont, settings.NoticeBodyFontSize);
        content.Title = settings.NonCompliantTitle;
        content.MessageLines = ParseMessageLines(message);
        content.HeaderBackgroundColor = settings.NonCompliantLettersBackgroundColor;
        content.HeaderForegroundColor = settings.NonCompliantLettersForegroundColor;
        content.HeaderBorderColor = settings.NonCompliantLettersBorderColor;
        content.ShowAssemblies = true;

        var sites = await GetBackflowLetterSitesAsync(context, period);
        var fileTypeName = $"{BackflowFilePrefix} - Non-Compliant{noticeNumber} {period.Name}";

        return AddLetters(context, content, sites, fileTypeName, isPrinted);
    }

    private async Task<int> GenerateUnknownAssembliesLettersAsync(GenerationContext context)
    {
        var settings = context.Details.BackflowSettings;

        var message = ReplaceTokens(context, settings.UnknownAssembliesMessage);
        message = ReplaceSeguinTokens(context, message, string.Empty, string.Empty);

        var content = CreateContent(context, settings.NoticeBodyFont, settings.NoticeBodyFontSize);
        content.Title = settings.UnknownAssembliesTitle;
        content.MessageLines = ParseMessageLines(message);
        content.HeaderBackgroundColor = settings.UnknownAssembliesLettersBackgroundColor;
        content.HeaderForegroundColor = settings.UnknownAssembliesLettersForegroundColor;
        content.HeaderBorderColor = settings.UnknownAssembliesLettersBorderColor;

        var lastModifiedBefore = DateTime.UtcNow.AddDays(-UnknownAssembliesWaitDays);
        var sites = await _letterRepository.GetUnknownAssembliesSitesAsync(context.Details.WaterSupplier.Id, lastModifiedBefore, CancellationToken.None);
        var fileTypeName = $"{BackflowFilePrefix} - Unknown Assemblies";

        foreach (var site in sites)
        {
            context.UnknownAssemblySiteIds.Add(site.Id);
        }

        return AddLetters(context, content, sites, fileTypeName, true);
    }

    private async Task<int> GenerateImpendingLettersAsync(GenerationContext context, int noticeNumber, CsiImpendingType impendingType)
    {
        var settings = context.Details.CsiSettings;
        var period = LetterPeriod.ForImpending(context.Now, impendingType);

        var content = CreateContent(context, settings.NoticeBodyFont, settings.NoticeBodyFontSize);
        content.Title = settings.ImpendingTitle;
        content.MessageLines = ParseMessageLines(ReplaceTokens(context, settings.ImpendingMessage));
        content.HeaderBackgroundColor = settings.ImpendingLettersBackgroundColor;
        content.HeaderForegroundColor = settings.ImpendingLettersForegroundColor;
        content.HeaderBorderColor = settings.ImpendingLettersBorderColor;

        var sites = await _letterRepository.GetCsiSitesAsync(context.Details.WaterSupplier.Id, period.Start, period.End, CancellationToken.None);
        var fileTypeName = $"{CsiFilePrefix} - Impending{noticeNumber} {period.Name}";

        return AddLetters(context, content, sites, fileTypeName, true);
    }

    private async Task<int> GeneratePastDueLettersAsync(GenerationContext context, int noticeNumber, CsiPastDueType pastDueType)
    {
        var settings = context.Details.CsiSettings;
        var period = LetterPeriod.ForPastDue(context.Now, pastDueType);

        var content = CreateContent(context, settings.NoticeBodyFont, settings.NoticeBodyFontSize);
        content.Title = settings.PastDueTitle;
        content.MessageLines = ParseMessageLines(ReplaceTokens(context, settings.PastDueMessage));
        content.HeaderBackgroundColor = settings.PastDueLettersBackgroundColor;
        content.HeaderForegroundColor = settings.PastDueLettersForegroundColor;
        content.HeaderBorderColor = settings.PastDueLettersBorderColor;

        var sites = await _letterRepository.GetCsiSitesAsync(context.Details.WaterSupplier.Id, period.Start, period.End, CancellationToken.None);
        var fileTypeName = $"{CsiFilePrefix} - Past Due{noticeNumber} {period.Name}";

        return AddLetters(context, content, sites, fileTypeName, true);
    }

    private async Task<int> GenerateCsiNonCompliantLettersAsync(GenerationContext context, int noticeNumber, CsiNonCompliantType nonCompliantType, bool isPrinted)
    {
        var settings = context.Details.CsiSettings;
        var period = LetterPeriod.ForNonCompliant(context.Now, nonCompliantType);

        var content = CreateContent(context, settings.NoticeBodyFont, settings.NoticeBodyFontSize);
        content.Title = settings.NonCompliantTitle;
        content.MessageLines = ParseMessageLines(ReplaceTokens(context, settings.NonCompliantMessage));
        content.HeaderBackgroundColor = settings.NonCompliantLettersBackgroundColor;
        content.HeaderForegroundColor = settings.NonCompliantLettersForegroundColor;
        content.HeaderBorderColor = settings.NonCompliantLettersBorderColor;

        var sites = await _letterRepository.GetCsiSitesAsync(context.Details.WaterSupplier.Id, period.Start, period.End, CancellationToken.None);
        var fileTypeName = $"{CsiFilePrefix} - Non-Compliant{noticeNumber} {period.Name}";

        return AddLetters(context, content, sites, fileTypeName, isPrinted);
    }

    private async Task<List<LetterSiteDto>> GetBackflowLetterSitesAsync(GenerationContext context, LetterPeriod period)
    {
        var waterSupplierId = context.Details.WaterSupplier.Id;
        var periodTests = await _letterRepository.GetBackflowTestsAsync(waterSupplierId, period.Start, period.End, CancellationToken.None);

        var tests = new List<LetterBackflowTestDto>();
        var testIds = new HashSet<int>();

        foreach (var requirement in context.RenewalRequirements)
        {
            foreach (var test in periodTests)
            {
                if (DoesTestMatchRequirement(test, requirement) && testIds.Add(test.Id))
                {
                    tests.Add(test);
                }
            }
        }

        foreach (var test in periodTests)
        {
            if (test.ForceRenewal && testIds.Add(test.Id))
            {
                tests.Add(test);
            }
        }

        if (tests.Count == 0)
        {
            return [];
        }

        var sites = await _letterRepository.GetBackflowSitesAsync(waterSupplierId, period.Start, period.End, CancellationToken.None);
        var sitesById = sites.ToDictionary(site => site.Id);

        var orderedSites = new List<LetterSiteDto>();

        foreach (var test in tests)
        {
            if (!sitesById.TryGetValue(test.SiteId, out var site))
            {
                continue;
            }

            if (site.BackflowTests.Count == 0)
            {
                orderedSites.Add(site);
            }

            site.BackflowTests.Add(test);
        }

        return orderedSites;
    }

    private static bool DoesTestMatchRequirement(LetterBackflowTestDto test, BackflowRenewalRequirementDto requirement)
    {
        if (test.SitePropertyType != requirement.PropertyType)
        {
            return false;
        }

        if (!IsAllValue(requirement.DeviceType) && !string.Equals(test.DeviceType, requirement.DeviceType, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!IsAllValue(requirement.HazardType) && !string.Equals(test.HazardType, requirement.HazardType, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (requirement.HasSiteOssf && !test.SiteHasOnSiteSewageFacility)
        {
            return false;
        }

        if (requirement.AuxWaterSupply && !test.SiteHasAuxWaterSupply)
        {
            return false;
        }

        return true;
    }

    private static bool IsAllValue(string? value)
    {
        return string.IsNullOrEmpty(value) || string.Equals(value, AllValue, StringComparison.OrdinalIgnoreCase);
    }

    private static int AddLetters(GenerationContext context, LetterContentDto content, List<LetterSiteDto> sites, string fileTypeName, bool includeInDownload)
    {
        var printedLetters = new List<LetterDto>();

        foreach (var site in sites)
        {
            var emailAddresses = GetEmailAddresses(site);

            if (emailAddresses.Count == 0 && !includeInDownload)
            {
                continue;
            }

            var optInCode = GetOptInCode(context, site.Id);
            var letter = BuildLetter(context, site, optInCode);

            if (emailAddresses.Count > 0)
            {
                var emailFileName = $"{context.Details.WaterSupplier.Id} - {fileTypeName} - {site.Id}.html";

                context.Emails.Add(new LetterEmail(emailAddresses, content, letter, emailFileName));
            }
            else
            {
                printedLetters.Add(letter);
            }
        }

        AddPdfDocuments(context, content, printedLetters, fileTypeName);

        return sites.Count;
    }

    private static OptInCode GetOptInCode(GenerationContext context, int siteId)
    {
        if (context.OptInCodes.TryGetValue(siteId, out var existingCode))
        {
            return existingCode;
        }

        var maxValue = (int)Math.Pow(10, OptInCodeLength);
        var code = RandomNumberGenerator.GetInt32(maxValue).ToString($"D{OptInCodeLength}");
        var codeHash = BCrypt.Net.BCrypt.HashPassword(code);

        var optInCode = new OptInCode(code, codeHash);

        context.OptInCodes.Add(siteId, optInCode);

        return optInCode;
    }

    private static void AddPdfDocuments(GenerationContext context, LetterContentDto content, List<LetterDto> letters, string fileTypeName)
    {
        var fileNumber = 0;

        for (var index = 0; index < letters.Count; index += LettersPerFile)
        {
            fileNumber++;

            var letterCount = Math.Min(LettersPerFile, letters.Count - index);

            var document = new LetterDocumentDto
            {
                Content = content,
                Letters = letters.GetRange(index, letterCount)
            };

            var fileName = $"{context.Details.WaterSupplier.Id} - {fileTypeName} ({fileNumber}).pdf";

            context.PdfDocuments.Add(new LetterPdfDocument(fileName, document));
        }
    }

    private async Task SaveOptInCodesAsync(GenerationContext context)
    {
        if (context.OptInCodes.Count == 0)
        {
            return;
        }

        var optInCodes = new List<LetterOptInCodeDto>();

        foreach (var siteCode in context.OptInCodes)
        {
            optInCodes.Add(new LetterOptInCodeDto
            {
                SiteId = siteCode.Key,
                CodeHash = siteCode.Value.CodeHash
            });
        }

        await _letterRepository.UpdateOptInCodesAsync(optInCodes, context.OptInCodeExpirationDate);
    }

    private async Task SaveUnknownAssemblyLettersSentAsync(GenerationContext context)
    {
        if (context.UnknownAssemblySiteIds.Count == 0)
        {
            return;
        }

        await _letterRepository.IncrementUnknownAssemblyLettersSentAsync(context.UnknownAssemblySiteIds);
    }

    private async Task SendLetterEmailsAsync(GenerationContext context)
    {
        if (context.Emails.Count == 0)
        {
            return;
        }

        var folderPath = $"{GetFolderPath(context)}/{EmailsFolder}";
        var letterLogs = new List<SiteLetterLog>();

        foreach (var email in context.Emails)
        {
            letterLogs.Add(CreateEmailLetterLog(context, email, folderPath));
        }

        await _siteLetterLogRepository.AddRangeAsync(letterLogs);

        for (var index = 0; index < context.Emails.Count; index++)
        {
            var email = context.Emails[index];

            var templateData = new LetterEmailVm
            {
                BaseUrl = _pdfTemplateOptions.BaseUrl,
                Content = email.Content,
                Letter = email.Letter
            };

            var html = await _htmlTemplateService.ParseEmailAsync(EmailTemplateName, templateData);

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(html));

            await _fileStorageService.UploadAsync(letterLogs[index].FilePath, stream);

            await _emailService.SendAsync(new EmailDto<LetterEmailVm>
            {
                Recipients = email.Recipients,
                Subject = EmailSubject,
                TemplateId = EmailTemplateName,
                TemplateData = templateData
            });
        }
    }

    private static SiteLetterLog CreateEmailLetterLog(GenerationContext context, LetterEmail email, string folderPath)
    {
        return new SiteLetterLog
        {
            WaterSupplierId = email.Letter.WaterSupplierId,
            SiteId = email.Letter.SiteId,
            ProcessDate = context.ProcessDate,
            FileName = email.FileName,
            Page = SiteLetterLog.EmailPage,
            FilePath = $"{folderPath}/{email.FileName}"
        };
    }

    private async Task AddPdfFilesAsync(GenerationContext context)
    {
        if (context.PdfDocuments.Count == 0)
        {
            return;
        }

        var documents = new List<List<LetterDocumentDto>>();

        foreach (var pdfDocument in context.PdfDocuments)
        {
            documents.Add(SplitIntoChunks(pdfDocument.Document));
        }

        var folderPath = GetFolderPath(context);
        var index = 0;

        await foreach (var pdf in _pdfTemplateService.GenerateManyAsync<LetterDocumentDto, int[]>(PdfTemplateName, documents, PageSiteIdsScript))
        {
            var pdfDocument = context.PdfDocuments[index];
            var filePath = $"{folderPath}/{pdfDocument.FileName}";

            var letterLogs = CreateLetterLogs(context, pdfDocument, pdf.ScriptResults, filePath);

            await _siteLetterLogRepository.AddRangeAsync(letterLogs);

            using var stream = new MemoryStream(pdf.Content);

            await _fileStorageService.UploadAsync(filePath, stream);

            index++;
        }
    }

    private static List<LetterDocumentDto> SplitIntoChunks(LetterDocumentDto document)
    {
        var chunks = new List<LetterDocumentDto>();

        for (var index = 0; index < document.Letters.Count; index += LettersPerChunk)
        {
            var letterCount = Math.Min(LettersPerChunk, document.Letters.Count - index);

            chunks.Add(new LetterDocumentDto
            {
                Content = document.Content,
                Letters = document.Letters.GetRange(index, letterCount)
            });
        }

        return chunks;
    }

    private static string GetFolderPath(GenerationContext context)
    {
        var waterSupplier = context.Details.WaterSupplier;
        var processId = LetterProcessId.FromProcessDate(context.ProcessDate);
        var waterSupplierFolder = RemoveInvalidFileNameChars($"{waterSupplier.Id} - {waterSupplier.Name}");

        return $"{LettersFolder}/{processId}/{waterSupplierFolder}";
    }

    private static string RemoveInvalidFileNameChars(string name)
    {
        var result = new StringBuilder();

        foreach (var character in name)
        {
            if (!InvalidFileNameChars.Contains(character))
            {
                result.Append(character);
            }
        }

        return result.ToString();
    }

    private static List<SiteLetterLog> CreateLetterLogs(GenerationContext context, LetterPdfDocument pdfDocument, List<int[]> chunkPageSiteIds, string filePath)
    {
        var firstPages = new Dictionary<int, int>();
        var pageNumber = 0;

        foreach (var pageSiteIds in chunkPageSiteIds)
        {
            foreach (var siteId in pageSiteIds)
            {
                pageNumber++;

                firstPages.TryAdd(siteId, pageNumber);
            }
        }

        var letterLogs = new List<SiteLetterLog>();

        foreach (var letter in pdfDocument.Document.Letters)
        {
            letterLogs.Add(new SiteLetterLog
            {
                WaterSupplierId = letter.WaterSupplierId,
                SiteId = letter.SiteId,
                ProcessDate = context.ProcessDate,
                FileName = pdfDocument.FileName,
                Page = firstPages[letter.SiteId],
                FilePath = filePath
            });
        }

        return letterLogs;
    }

    private static List<string> GetEmailAddresses(LetterSiteDto site)
    {
        var emailAddresses = new List<string>();

        if (site.RenewalOptInType != RenewalOptInType.OptedIn || string.IsNullOrWhiteSpace(site.MailingEmailAddress))
        {
            return emailAddresses;
        }

        var parts = site.MailingEmailAddress.Split([';', ','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var part in parts)
        {
            emailAddresses.Add(part);
        }

        return emailAddresses;
    }

    private static LetterContentDto CreateContent(GenerationContext context, string? fontFamily, int? fontSize)
    {
        var waterSupplier = context.Details.WaterSupplier;

        var content = new LetterContentDto
        {
            FontFamily = DefaultFontFamily,
            FontSize = DefaultFontSize,
            LetterDate = FormatDate(context.Now),
            ReturnCompanyName = waterSupplier.LetterCompanyName,
            ReturnAddress = waterSupplier.LetterAddress,
            ReturnCityStateZip = $"{waterSupplier.LetterCity}, {GetStateCode(waterSupplier.LetterState)}  {waterSupplier.LetterZipCode}",
            ContactCompanyName = waterSupplier.LetterContactCompanyName,
            ContactName = waterSupplier.LetterContactContactName,
            ContactPhoneNumber = waterSupplier.LetterContactPhoneNumber,
            ContactFaxNumber = waterSupplier.LetterContactFaxNumber,
            ContactEmailAddress = waterSupplier.LetterContactEmailAddress,
            SignatureName = waterSupplier.ContactName
        };

        if (!string.IsNullOrWhiteSpace(waterSupplier.LetterContactAddress))
        {
            content.ContactAddress = waterSupplier.LetterContactAddress;
            content.ContactCityStateZip = $"{waterSupplier.LetterContactCity}, {GetStateCode(waterSupplier.LetterContactState)} {waterSupplier.LetterContactZipCode}";
        }

        content.HasMainContact = !string.IsNullOrWhiteSpace(content.ContactCompanyName)
            || !string.IsNullOrWhiteSpace(content.ContactName)
            || !string.IsNullOrWhiteSpace(content.ContactAddress);

        if (!string.IsNullOrWhiteSpace(fontFamily))
        {
            content.FontFamily = fontFamily;
        }

        if (fontSize != null && fontSize.Value > 0)
        {
            content.FontSize = fontSize.Value;
        }

        return content;
    }

    private static string ReplaceTokens(GenerationContext context, string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return string.Empty;
        }

        var waterSupplier = context.Details.WaterSupplier;
        var now = context.Now;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);

        var result = message;

        result = result.Replace("##WsID##", waterSupplier.Id.ToString());
        result = result.Replace("##WsName##", waterSupplier.Name);
        result = result.Replace("##WsAddress##", waterSupplier.Address);
        result = result.Replace("##WsCity##", waterSupplier.City);
        result = result.Replace("##WsState##", GetStateCode(waterSupplier.State));
        result = result.Replace("##WsZip##", waterSupplier.ZipCode);
        result = result.Replace("##WsPhoneNumber##", waterSupplier.PhoneNumber);
        result = result.Replace("##WsEmail##", waterSupplier.EmailAddress);

        result = result.Replace("##CurrentMonth##", now.Month.ToString());
        result = result.Replace("##CurrentDay##", now.Day.ToString());
        result = result.Replace("##CurrentYear##", now.Year.ToString());
        result = result.Replace("##EndOfMonth##", FormatDate(monthEnd));
        result = result.Replace("##TodayPlus30Days##", FormatDate(now.AddDays(30)));
        result = result.Replace("##TodayPlus60Days##", FormatDate(now.AddDays(60)));
        result = result.Replace("##TodayPlus90Days##", FormatDate(now.AddDays(90)));
        result = result.Replace("##TodayPlus120Days##", FormatDate(now.AddDays(120)));
        result = result.Replace("##TodayPlus180Days##", FormatDate(now.AddDays(180)));
        result = result.Replace("##15thOfCurrentMonth##", FormatDate(monthStart.AddDays(14)));
        result = result.Replace("##20thOfCurrentMonth##", FormatDate(monthStart.AddDays(19)));

        return result;
    }

    private static string ReplaceSeguinTokens(GenerationContext context, string message, string customText1, string customText2)
    {
        if (context.Details.WaterSupplier.Id != SeguinLetterTexts.WaterSupplierId)
        {
            return message;
        }

        var result = message.Replace(SeguinLetterTexts.Custom1Token, customText1);
        result = result.Replace(SeguinLetterTexts.Custom2Token, customText2);

        return result;
    }

    private static List<LetterMessageLineDto> ParseMessageLines(string message)
    {
        var lines = new List<LetterMessageLineDto>();

        var isBold = false;
        var isItalic = false;
        var isUnderline = false;

        var normalizedMessage = message.Trim().Replace("\r", string.Empty).Replace("\t", "     ");

        foreach (var rawLine in normalizedMessage.Split('\n'))
        {
            var line = new LetterMessageLineDto();

            foreach (var part in StyleTagRegex.Split(rawLine.Trim()))
            {
                var tag = part.ToLowerInvariant();

                if (tag == "[b]")
                {
                    isBold = true;
                }
                else if (tag == "[/b]")
                {
                    isBold = false;
                }
                else if (tag == "[i]")
                {
                    isItalic = true;
                }
                else if (tag == "[/i]")
                {
                    isItalic = false;
                }
                else if (tag == "[u]")
                {
                    isUnderline = true;
                }
                else if (tag == "[/u]")
                {
                    isUnderline = false;
                }
                else if (part.Length > 0)
                {
                    line.Runs.Add(new LetterMessageRunDto
                    {
                        Text = part,
                        IsBold = isBold,
                        IsItalic = isItalic,
                        IsUnderline = isUnderline
                    });
                }
            }

            lines.Add(line);
        }

        return lines;
    }

    private static LetterDto BuildLetter(GenerationContext context, LetterSiteDto site, OptInCode optInCode)
    {
        var letter = new LetterDto
        {
            SiteId = site.Id,
            WaterSupplierId = site.WaterSupplierId,
            AccountNumber = GetDisplayAccountNumber(context, site),
            OptInCode = optInCode.Code,
            OptInExpirationDate = FormatDate(context.OptInCodeExpirationDate)
        };

        letter.MailingLines.AddRange(GetMailingNameLines(site));
        letter.MailingLines.Add(FormatStreetAddress(site.MailingStreetNumber, site.MailingStreetName, site.MailingNumber));
        letter.MailingLines.Add($"{site.MailingCity}, {site.MailingStateCode}  {site.MailingZipCode}");

        if (!string.IsNullOrWhiteSpace(site.BusinessName))
        {
            letter.PropertyLines.Add(site.BusinessName);
        }

        letter.PropertyLines.Add(FormatStreetAddress(site.StreetNumber, site.StreetName, site.PropertyNumber));
        letter.PropertyLines.Add($"{site.City}, {site.StateCode}  {site.ZipCode}");

        foreach (var test in site.BackflowTests)
        {
            letter.Assemblies.Add(BuildAssembly(test));
        }

        return letter;
    }

    private static List<string> GetMailingNameLines(LetterSiteDto site)
    {
        var lines = new List<string>();

        if (!string.IsNullOrWhiteSpace(site.MailingCompanyName))
        {
            lines.Add(site.MailingCompanyName);

            if (!string.IsNullOrWhiteSpace(site.MailingContactName))
            {
                lines.Add(site.MailingContactName);
            }

            return lines;
        }

        var isResidential = site.PropertyType == PropertyType.Residential;

        if (string.IsNullOrWhiteSpace(site.MailingContactName))
        {
            if (isResidential)
            {
                lines.Add(ResidentialDefaultName);
            }
            else
            {
                lines.Add(CommercialDefaultName);
            }

            return lines;
        }

        var contactName = site.MailingContactName;

        if (isResidential && !contactName.Contains(ResidentWord, StringComparison.OrdinalIgnoreCase))
        {
            contactName += ResidentSuffix;
        }

        lines.Add(contactName);

        return lines;
    }

    private static LetterAssemblyDto BuildAssembly(LetterBackflowTestDto test)
    {
        var assembly = new LetterAssemblyDto
        {
            Description = $"{test.Manufacturer} {test.Model} {test.Size} - {test.DeviceType}",
            SerialNumber = test.SerialNumber,
            LocationDescription = Shorten(test.LocationDescription),
            IsFireSystem = test.HazardType == FireSystemHazardType,
            TesterCompanyName = Shorten(test.BpatCompanyName),
            TesterPhoneNumber = Shorten(test.BpatWorkNumber),
            ExpirationDate = FormatDate(test.ExpirationDate)
        };

        if (IsTestDateShown(test.TransactionId))
        {
            assembly.TestDate = FormatDate(test.TestDate);
        }

        return assembly;
    }

    private static bool IsTestDateShown(string? transactionId)
    {
        if (string.Equals(transactionId, ImportTransactionId, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (string.Equals(transactionId, ManualTransactionId, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }

    private static string GetDisplayAccountNumber(GenerationContext context, LetterSiteDto site)
    {
        if (context.Details.GeneralSettings.UseWsAccountNumbersOnLetters && !string.IsNullOrWhiteSpace(site.WaterSupplierAccountNumber))
        {
            return site.WaterSupplierAccountNumber;
        }

        return site.AccountNumber;
    }

    private static string FormatStreetAddress(string? streetNumber, string? streetName, string? unitNumber)
    {
        var address = $"{streetNumber} {streetName}".Trim();

        if (!string.IsNullOrWhiteSpace(unitNumber))
        {
            address += $" #{unitNumber}";
        }

        return address;
    }

    private static string? Shorten(string? text)
    {
        if (text == null || text.Length <= MaxAssemblyTextLength)
        {
            return text;
        }

        return text.Substring(0, MaxAssemblyTextLength) + "...";
    }

    private static string? GetStateCode(ReferencedStateDto? state)
    {
        if (state == null)
        {
            return null;
        }

        return state.Code;
    }

    private static string FormatDate(DateTime date)
    {
        return date.ToString(DateFormat, CultureInfo.InvariantCulture);
    }

    private static string? FormatDate(DateTime? date)
    {
        if (date == null)
        {
            return null;
        }

        return FormatDate(date.Value);
    }

    private class GenerationContext
    {
        public WaterSupplierDetailsDto Details { get; set; } = null!;
        public List<BackflowRenewalRequirementDto> RenewalRequirements { get; set; } = [];
        public DateTime Now { get; set; }
        public DateTime ProcessDate { get; set; }
        public DateTime OptInCodeExpirationDate { get; set; }

        public LetterCountsDto Counts { get; } = new();

        public Dictionary<int, OptInCode> OptInCodes { get; } = new();
        public List<int> UnknownAssemblySiteIds { get; } = [];

        public List<LetterEmail> Emails { get; } = [];
        public List<LetterPdfDocument> PdfDocuments { get; } = [];
    }

    private record OptInCode(string Code, string CodeHash);

    private record LetterEmail(List<string> Recipients, LetterContentDto Content, LetterDto Letter, string FileName);

    private record LetterPdfDocument(string FileName, LetterDocumentDto Document);
}
