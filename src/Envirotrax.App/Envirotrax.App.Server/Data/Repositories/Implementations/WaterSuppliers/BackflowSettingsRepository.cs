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
                ShowRainSensor = settings.ShowRainSensor,
                ShowOSSF = settings.ShowOSSF,
                ShowPermitNumber = settings.ShowPermitNumber
            };

        return await query.SingleOrDefaultAsync(cancellationToken);
    }
}
