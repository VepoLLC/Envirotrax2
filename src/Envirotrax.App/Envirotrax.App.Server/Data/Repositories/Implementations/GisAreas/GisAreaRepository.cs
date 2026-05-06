using Envirotrax.App.Server.Data.Models.GisAreas;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.GisAreas;
using Envirotrax.App.Server.Data.Services.Definitions;
using Envirotrax.Common.Data.Services.Definitions;
using Microsoft.EntityFrameworkCore;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.GisAreas;

public class GisAreaRepository : Repository<GisArea>, IGisAreaRepository
{
    private readonly ITenantProvidersService _tenantProvider;

    public GisAreaRepository(IDbContextSelector dbContextSelector, ITenantProvidersService tenantProvider)
        : base(dbContextSelector)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void UpdateEntity(GisArea model)
    {
        base.UpdateEntity(model);

        var entry = DbContext.Entry(model);

        entry.Property(a => a.MinLongitude).IsModified = false;
        entry.Property(a => a.MinLatitude).IsModified = false;
        entry.Property(a => a.MaxLongitude).IsModified = false;
        entry.Property(a => a.MaxLatitude).IsModified = false;
    }

    public async Task<DefaultGisMapView> GetDefaultMapViewAsync(CancellationToken cancellationToken)
    {
        return await DbContext
            .WaterSuppliers
            .Where(s => s.Id == _tenantProvider.WaterSupplierId)
            .Select(s => new DefaultGisMapView
            {
                GisCenterLatitude = s.GisCenterLatitude,
                GisCenterLongitude = s.GisCenterLongitude,
                GisCenterZoom = s.GisCenterZoom
            }).SingleAsync(cancellationToken);
    }

    public async Task<DefaultGisMapView> UpdateDefaultMapViewAsync(DefaultGisMapView mapView)
    {
        var supplier = new WaterSupplier
        {
            Id = _tenantProvider.WaterSupplierId
        };

        DbContext.WaterSuppliers.Attach(supplier);

        supplier.GisCenterLatitude = mapView.GisCenterLatitude;
        supplier.GisCenterLongitude = mapView.GisCenterLongitude;
        supplier.GisCenterZoom = mapView.GisCenterZoom;

        await DbContext.SaveChangesAsync();

        return mapView;
    }

    public async Task UpdateBoundsAsync(int areaId, double minLongitude, double minLatitude, double maxLongitude, double maxLatitude)
    {
        await DbContext
            .GisAreas
            .Where(a => a.Id == areaId)
            .ExecuteUpdateAsync(set => set
                .SetProperty(a => a.MinLongitude, minLongitude)
                .SetProperty(a => a.MinLatitude, minLatitude)
                .SetProperty(a => a.MaxLongitude, maxLongitude)
                .SetProperty(a => a.MaxLatitude, maxLatitude)
            );
    }
}
