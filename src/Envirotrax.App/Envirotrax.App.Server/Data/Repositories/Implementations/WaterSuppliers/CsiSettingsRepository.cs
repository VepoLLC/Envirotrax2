using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class CsiSettingsRepository : TenantSettingsRepository<CsiSettings>, ICsiSettingsRepository
{
    public CsiSettingsRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override void CopyValues(CsiSettings source, CsiSettings target, SettingsSection section)
    {
        switch (section)
        {
            case SettingsSection.Csi:
                CopyCsiSettings(source, target);
                break;

            case SettingsSection.CsiLetterMessage:
                CopyLetterMessageSettings(source, target);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(section), section, null);
        }
    }

    private static void CopyCsiSettings(CsiSettings source, CsiSettings target)
    {
        target.ModificationGracePeriodDays = source.ModificationGracePeriodDays;
        target.NewlyCreatedBackflowTestExpirationDays = source.NewlyCreatedBackflowTestExpirationDays;
        target.RequireInspectionImages = source.RequireInspectionImages;

        target.ImpendingNotice1 = source.ImpendingNotice1;
        target.ImpendingNotice2 = source.ImpendingNotice2;
        target.PastDueNotice1 = source.PastDueNotice1;
        target.PastDueNotice2 = source.PastDueNotice2;
        target.NonCompliant1 = source.NonCompliant1;
        target.NonCompliant2 = source.NonCompliant2;

        target.ImpendingLettersBackgroundColor = source.ImpendingLettersBackgroundColor;
        target.ImpendingLettersForegroundColor = source.ImpendingLettersForegroundColor;
        target.ImpendingLettersBorderColor = source.ImpendingLettersBorderColor;
        target.PastDueLettersBackgroundColor = source.PastDueLettersBackgroundColor;
        target.PastDueLettersForegroundColor = source.PastDueLettersForegroundColor;
        target.PastDueLettersBorderColor = source.PastDueLettersBorderColor;
        target.NonCompliantLettersBackgroundColor = source.NonCompliantLettersBackgroundColor;
        target.NonCompliantLettersForegroundColor = source.NonCompliantLettersForegroundColor;
        target.NonCompliantLettersBorderColor = source.NonCompliantLettersBorderColor;
    }

    private static void CopyLetterMessageSettings(CsiSettings source, CsiSettings target)
    {
        target.NoticeBodyFont = source.NoticeBodyFont;
        target.NoticeBodyFontSize = source.NoticeBodyFontSize;

        target.ImpendingTitle = source.ImpendingTitle;
        target.ImpendingMessage = source.ImpendingMessage;
        target.PastDueTitle = source.PastDueTitle;
        target.PastDueMessage = source.PastDueMessage;
        target.NonCompliantTitle = source.NonCompliantTitle;
        target.NonCompliantMessage = source.NonCompliantMessage;
    }
}
