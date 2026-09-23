using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Repositories.Implementations;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogSettingsRepository : Repository<GeneralSettings>, IFogSettingsRepository
{
    private readonly IAuthService _authService;

    public FogSettingsRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    public async Task<ProfessionalFogSettingsDto?> GetSettingsAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var professionalId = _authService.ProfessionalId;

        var query =
            from settings in Entity
            join registration in DbContext.ProfessionalWaterSuppliers
                on settings.WaterSupplierId equals registration.WaterSupplierId
            where settings.WaterSupplierId == waterSupplierId
                && registration.ProfessionalId == professionalId
            select new ProfessionalFogSettingsDto
            {
                FogTransportersRequireInsurance = settings.FogTransportersRequireInsurance,
                FogTransportersRequireInsuranceAmount = settings.FogTransportersRequireInsuranceAmount,
                FogVehiclesRequirePermit = settings.FogVehiclesRequirePermit,
                FogVehiclesRequireInspection = settings.FogVehiclesRequireInspection
            };

        return await query.SingleOrDefaultAsync(cancellationToken);
    }
}
