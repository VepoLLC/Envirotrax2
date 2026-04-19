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
}
