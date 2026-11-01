using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using Envirotrax.App.Server.Data.Models.Backflow;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Sites;
using Envirotrax.App.Server.Data.Repositories.Definitions.Letters;
using Envirotrax.App.Server.Domain.DataTransferObjects.Letters;
using Envirotrax.App.Server.Domain.DataTransferObjects.Lookup;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.Letters;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Templates.Emails.Letters;
using Envirotrax.Common.Domain.DataTransferObjects;
using Envirotrax.Common.Domain.Services.Defintions;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Letters;

public class LetterGeneratorService : ILetterGeneratorService
{
    private const int LettersPerFile = 1000;
    private const int OptInCodeLength = 6;
    private const int OptInCodeValidDays = 30;
    private const int UnknownAssembliesWaitDays = 15;
    private const int MaxAssemblyTextLength = 37;

    private const string BackflowFilePrefix = "BF";
    private const string CsiFilePrefix = "CSI";
    private const string PdfTemplateName = "Letters.Letters";
    private const string EmailTemplateName = "Letters.Letter";
    private const string EmailSubject = "Important Notice from Your Water Supplier – Action May Be Required";
    private const string OptInUrl = "https://www.envirotrax.com/renewal_opt_in.aspx?id=";

    private const string DefaultFontFamily = "Arial";
    private const int DefaultFontSize = 12;
    private const string DateFormat = "M/d/yyyy";

    private const string FireSystemHazardType = "Fire System";
    private const string ImportTransactionId = "import";
    private const string ManualTransactionId = "manual";

    private const string ResidentialDefaultName = "Current Resident";
    private const string CommercialDefaultName = "Owner Or Owner's Representative";
    private const string ResidentWord = "resident";
    private const string ResidentSuffix = " or Current Resident";

    private static readonly TimeZoneInfo CentralTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
    private static readonly Regex StyleTagRegex = new(@"(\[/?[biu]\])", RegexOptions.IgnoreCase);

    private readonly ILetterRepository _letterRepository;
    private readonly IWaterSupplierService _waterSupplierService;
    private readonly IBackflowRenewalRequirementService _renewalRequirementService;
    private readonly IPdfTemplateService _pdfTemplateService;
    private readonly IEmailService _emailService;

    public LetterGeneratorService(
        ILetterRepository letterRepository,
        IWaterSupplierService waterSupplierService,
        IBackflowRenewalRequirementService renewalRequirementService,
        IPdfTemplateService pdfTemplateService,
        IEmailService emailService)
    {
        _letterRepository = letterRepository;
        _waterSupplierService = waterSupplierService;
        _renewalRequirementService = renewalRequirementService;
        _pdfTemplateService = pdfTemplateService;
        _emailService = emailService;
    }

    public async Task<GeneratedLettersDto?> GenerateAsync(int waterSupplierId)
    {
        var details = await _waterSupplierService.GetDetailsAsync(waterSupplierId, CancellationToken.None);

        if (details == null)
        {
            return null;
        }

        var renewalRequirements = await _renewalRequirementService.GetAllByWaterSupplierIdAsync(waterSupplierId, CancellationToken.None);

        var context = new GenerationContext
        {
            Details = details,
            RenewalRequirements = renewalRequirements.ToList(),
            Now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, CentralTimeZone)
        };

        await GenerateBackflowLettersAsync(context);
        await GenerateCsiLettersAsync(context);

        var result = new GeneratedLettersDto
        {
            Counts = context.Counts,
            EmailCount = context.EmailCount
        };

        if (context.Files.Count > 0)
        {
            result.FileName = $"{details.WaterSupplier.Id} - {details.WaterSupplier.Name}.zip";
            result.FileContent = CreateZip(context.Files);
        }

        return result;
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

        return await GenerateLettersAsync(context, content, sites, fileTypeName, true);
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

        return await GenerateLettersAsync(context, content, sites, fileTypeName, true);
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

        return await GenerateLettersAsync(context, content, sites, fileTypeName, isPrinted);
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

        var letterCount = await GenerateLettersAsync(context, content, sites, fileTypeName, true);

        if (sites.Count > 0)
        {
            var siteIds = new List<int>();

            foreach (var site in sites)
            {
                siteIds.Add(site.Id);
            }

            await _letterRepository.IncrementUnknownAssemblyLettersSentAsync(siteIds);
        }

        return letterCount;
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

        return await GenerateLettersAsync(context, content, sites, fileTypeName, true);
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

        return await GenerateLettersAsync(context, content, sites, fileTypeName, true);
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

        return await GenerateLettersAsync(context, content, sites, fileTypeName, isPrinted);
    }

    private async Task<List<LetterSiteDto>> GetBackflowLetterSitesAsync(GenerationContext context, LetterPeriod period)
    {
        var waterSupplierId = context.Details.WaterSupplier.Id;

        var tests = new List<LetterBackflowTestDto>();
        var testIds = new HashSet<int>();

        foreach (var requirement in context.RenewalRequirements)
        {
            var requirementTests = await _letterRepository.GetRenewalRequirementTestsAsync(waterSupplierId, requirement, period.Start, period.End, CancellationToken.None);

            AddNewTests(tests, testIds, requirementTests);
        }

        var forcedRenewalTests = await _letterRepository.GetForcedRenewalTestsAsync(waterSupplierId, period.Start, period.End, CancellationToken.None);

        AddNewTests(tests, testIds, forcedRenewalTests);

        var siteIds = new List<int>();
        var addedSiteIds = new HashSet<int>();

        foreach (var test in tests)
        {
            if (addedSiteIds.Add(test.SiteId))
            {
                siteIds.Add(test.SiteId);
            }
        }

        if (siteIds.Count == 0)
        {
            return [];
        }

        var sites = await _letterRepository.GetSitesAsync(siteIds, CancellationToken.None);
        var sitesById = sites.ToDictionary(site => site.Id);

        foreach (var test in tests)
        {
            sitesById[test.SiteId].BackflowTests.Add(test);
        }

        var orderedSites = new List<LetterSiteDto>();

        foreach (var siteId in siteIds)
        {
            orderedSites.Add(sitesById[siteId]);
        }

        return orderedSites;
    }

    private static void AddNewTests(List<LetterBackflowTestDto> tests, HashSet<int> testIds, List<LetterBackflowTestDto> newTests)
    {
        foreach (var test in newTests)
        {
            if (testIds.Add(test.Id))
            {
                tests.Add(test);
            }
        }
    }

    private async Task<int> GenerateLettersAsync(GenerationContext context, LetterContentDto content, List<LetterSiteDto> sites, string fileTypeName, bool includeInDownload)
    {
        var printedLetters = new List<LetterDto>();

        foreach (var site in sites)
        {
            var optInCode = await GetOptInCodeAsync(context, site.Id);
            var letter = BuildLetter(context, site, optInCode);
            var emailAddresses = GetEmailAddresses(site);

            if (emailAddresses.Count > 0)
            {
                await SendLetterEmailAsync(content, letter, emailAddresses);

                context.EmailCount++;
            }
            else
            {
                printedLetters.Add(letter);
            }
        }

        if (includeInDownload)
        {
            await AddPdfFilesAsync(context, content, printedLetters, fileTypeName);
        }

        return sites.Count;
    }

    private async Task<OptInCode> GetOptInCodeAsync(GenerationContext context, int siteId)
    {
        if (context.OptInCodes.TryGetValue(siteId, out var existingCode))
        {
            return existingCode;
        }

        var maxValue = (int)Math.Pow(10, OptInCodeLength);
        var code = RandomNumberGenerator.GetInt32(maxValue).ToString($"D{OptInCodeLength}");
        var expirationDate = DateTime.UtcNow.AddDays(OptInCodeValidDays);
        var codeHash = BCrypt.Net.BCrypt.HashPassword(code);

        await _letterRepository.UpdateOptInCodeAsync(siteId, codeHash, expirationDate);

        var optInCode = new OptInCode(code, expirationDate);

        context.OptInCodes.Add(siteId, optInCode);

        return optInCode;
    }

    private async Task AddPdfFilesAsync(GenerationContext context, LetterContentDto content, List<LetterDto> letters, string fileTypeName)
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

            var pdf = await _pdfTemplateService.GenerateAsync(PdfTemplateName, document);
            var fileName = $"{context.Details.WaterSupplier.Id} - {fileTypeName} ({fileNumber}).pdf";

            context.Files.Add(new LetterFile(fileName, pdf));
        }
    }

    private async Task SendLetterEmailAsync(LetterContentDto content, LetterDto letter, List<string> emailAddresses)
    {
        await _emailService.SendAsync(new EmailDto<LetterEmailVm>
        {
            Recipients = emailAddresses,
            Subject = EmailSubject,
            TemplateId = EmailTemplateName,
            TemplateData = new LetterEmailVm
            {
                Content = content,
                Letter = letter
            }
        });
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
            AccountNumber = GetDisplayAccountNumber(context, site),
            OptInUrl = OptInUrl + site.Id,
            OptInCode = optInCode.Code,
            OptInExpirationDate = FormatDate(optInCode.ExpirationDate)
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

    private static byte[] CreateZip(List<LetterFile> files)
    {
        using var stream = new MemoryStream();

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true))
        {
            foreach (var file in files)
            {
                var entry = archive.CreateEntry(file.Name);

                using var entryStream = entry.Open();

                entryStream.Write(file.Content);
            }
        }

        return stream.ToArray();
    }

    private class GenerationContext
    {
        public WaterSupplierDetailsDto Details { get; set; } = null!;
        public List<BackflowRenewalRequirementDto> RenewalRequirements { get; set; } = [];
        public DateTime Now { get; set; }

        public LetterCountsDto Counts { get; } = new();
        public int EmailCount { get; set; }

        public Dictionary<int, OptInCode> OptInCodes { get; } = new();
        public List<LetterFile> Files { get; } = [];
    }

    private record OptInCode(string Code, DateTime ExpirationDate);

    private record LetterFile(string Name, byte[] Content);
}
