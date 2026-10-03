using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Backflow;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class BackflowSettingsRepository : TenantSettingsRepository<BackflowSettings>, IBackflowSettingsRepository
{
    private readonly IAuthService _authService;

    public BackflowSettingsRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    public async Task<BackflowTestingSettingsDto?> GetTestingSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var professionalId = _authService.ProfessionalId;

        var query =
            from settings in Entity
            join registration in DbContext.ProfessionalWaterSuppliers
                on settings.WaterSupplierId equals registration.WaterSupplierId
            where settings.WaterSupplierId == waterSupplierId
                && registration.ProfessionalId == professionalId
            select new BackflowTestingSettingsDto
            {
                ShowWaterMeterNumber = settings.ShowWaterMeterNumber,
                ShowRainSensor = settings.ShowRainSensor,
                ShowOSSF = settings.ShowOSSF,
                ShowPermitNumber = settings.ShowPermitNumber
            };

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<BackflowTestingSettingsDto?> GetTestingSettingsByWaterSupplierAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var query =
            from settings in Entity
            where settings.WaterSupplierId == waterSupplierId
            select new BackflowTestingSettingsDto
            {
                ShowWaterMeterNumber = settings.ShowWaterMeterNumber,
                ShowRainSensor = settings.ShowRainSensor,
                ShowOSSF = settings.ShowOSSF,
                ShowPermitNumber = settings.ShowPermitNumber
            };

        return await query.SingleOrDefaultAsync(cancellationToken);
    }

    protected override void CopyValues(BackflowSettings source, BackflowSettings target, SettingsSection section)
    {
        switch (section)
        {
            case SettingsSection.Backflow:
                CopyBackflowSettings(source, target);
                break;

            case SettingsSection.BackflowLetterMessage:
                CopyLetterMessageSettings(source, target);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(section), section, null);
        }
    }

    private static void CopyBackflowSettings(BackflowSettings source, BackflowSettings target)
    {
        target.TestingMethod = source.TestingMethod;
        target.GracePeriodDays = source.GracePeriodDays;
        target.AdjustBackflowCreepingDates = source.AdjustBackflowCreepingDates;
        target.NewInstallationsRequireApproval = source.NewInstallationsRequireApproval;
        target.ReplacementsRequireApproval = source.ReplacementsRequireApproval;
        target.DetectorAssembliesRequireMeterReading = source.DetectorAssembliesRequireMeterReading;
        target.OutOfServiceRequiresApproval = source.OutOfServiceRequiresApproval;
        target.OutOfServiceType = source.OutOfServiceType;
        target.RequireBackflowTestImages = source.RequireBackflowTestImages;

        target.ExpiringNotice1 = source.ExpiringNotice1;
        target.ExpiringNotice2 = source.ExpiringNotice2;
        target.ExpiredNotice1 = source.ExpiredNotice1;
        target.ExpiredNotice2 = source.ExpiredNotice2;
        target.BackflowNonCompliant1 = source.BackflowNonCompliant1;
        target.BackflowNonCompliant2 = source.BackflowNonCompliant2;

        target.ShowWaterMeterNumber = source.ShowWaterMeterNumber;
        target.ShowRainSensor = source.ShowRainSensor;
        target.ShowOSSF = source.ShowOSSF;
        target.ShowPermitNumber = source.ShowPermitNumber;

        target.ExpiringLettersBackgroundColor = source.ExpiringLettersBackgroundColor;
        target.ExpiringLettersForegroundColor = source.ExpiringLettersForegroundColor;
        target.ExpiringLettersBorderColor = source.ExpiringLettersBorderColor;
        target.ExpiredLettersBackgroundColor = source.ExpiredLettersBackgroundColor;
        target.ExpiredLettersForegroundColor = source.ExpiredLettersForegroundColor;
        target.ExpiredLettersBorderColor = source.ExpiredLettersBorderColor;
        target.NonCompliantLettersBackgroundColor = source.NonCompliantLettersBackgroundColor;
        target.NonCompliantLettersForegroundColor = source.NonCompliantLettersForegroundColor;
        target.NonCompliantLettersBorderColor = source.NonCompliantLettersBorderColor;
    }

    private static void CopyLetterMessageSettings(BackflowSettings source, BackflowSettings target)
    {
        target.NoticeBodyFont = source.NoticeBodyFont;
        target.NoticeBodyFontSize = source.NoticeBodyFontSize;

        target.ExpiringTitle = source.ExpiringTitle;
        target.ExpiringMessage = source.ExpiringMessage;
        target.ExpiredTitle = source.ExpiredTitle;
        target.ExpiredMessage = source.ExpiredMessage;
        target.NonCompliantTitle = source.NonCompliantTitle;
        target.NonCompliantMessage = source.NonCompliantMessage;
    }
}
