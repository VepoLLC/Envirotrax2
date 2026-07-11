using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Fog;
using Envirotrax.App.Server.Data.Repositories.Definitions.Fog;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.App.Server.Domain.DataTransferObjects.Fog;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Fog;

public class FogVehicleRepository : Repository<FogVehicle>, IFogVehicleRepository
{
    private readonly TenantDbContext _context;

    public FogVehicleRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
        _context = dbContextSelector.Current;
    }

    public async Task<IReadOnlyList<FogLookupItemDto>> GetAsOptionsAsync(CancellationToken ct)
    {
        var rows = await _context.FogVehicles
            .Where(v => v.DeletedTime == null)
            .Select(v => new { v.Id, v.ManufacturedYear, v.Manufacturer, v.LicensePlateNumber })
            .OrderBy(v => v.ManufacturedYear)
            .ThenBy(v => v.Manufacturer)
            .ThenBy(v => v.LicensePlateNumber)
            .ToListAsync(ct);

        return [.. rows.Select(v =>
            new FogLookupItemDto(v.Id, $"{v.ManufacturedYear} {v.Manufacturer} {v.LicensePlateNumber}".Trim()))];
    }
}
