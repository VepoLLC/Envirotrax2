using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Domain.Services.Defintions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.WaterSuppliers;

public class GeneralSettingsRepository : TenantSettingsRepository<GeneralSettings>, IGeneralSettingsRepository
{
    private readonly IAuthService _authService;

    public GeneralSettingsRepository(IDbContextSelector dbContextSelector, IAuthService authService)
        : base(dbContextSelector)
    {
        _authService = authService;
    }

    // Only for a water supplier the professional is registered with, as BackflowSettingsRepository's
    // GetTestingSettingsAsync does.
    public Task<GeneralSettings?> GetForProfessionalAsync(int waterSupplierId, CancellationToken cancellationToken)
    {
        var professionalId = _authService.ProfessionalId;

        var query =
            from settings in Entity
            join registration in DbContext.ProfessionalWaterSuppliers
                on settings.WaterSupplierId equals registration.WaterSupplierId
            where settings.WaterSupplierId == waterSupplierId
                && registration.ProfessionalId == professionalId
            select settings;

        return query
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
    }

    // One query for any number of water suppliers, returning those with RedactMailingInfo turned on.
    public async Task<HashSet<int>> GetRedactingWaterSupplierIdsAsync(IReadOnlyCollection<int> waterSupplierIds, CancellationToken cancellationToken)
    {
        var redactingSupplierIds = await Entity
            .Where(settings => waterSupplierIds.Contains(settings.WaterSupplierId) && settings.RedactMailingInfo)
            .Select(settings => settings.WaterSupplierId)
            .ToListAsync(cancellationToken);

        return redactingSupplierIds.ToHashSet();
    }

    protected override void CopyValues(GeneralSettings source, GeneralSettings target, SettingsSection section)
    {
        if (section != SettingsSection.General)
        {
            throw new ArgumentOutOfRangeException(nameof(section), section, null);
        }

        source.WaterSupplierId = target.WaterSupplierId;

        DbContext.Entry(target).CurrentValues.SetValues(source);
    }
}
