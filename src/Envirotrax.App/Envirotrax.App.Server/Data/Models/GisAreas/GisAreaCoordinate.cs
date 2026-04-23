
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.Common.Data.Attributes;
using Envirotrax.Common.Data.Models;

namespace Envirotrax.App.Server.Data.Models.GisAreas;

public class GisAreaCoordinate : TenantModel<WaterSupplier>
{
    [AppPrimaryKey(true)]
    public long Id { get; set; }

    public int AreaId { get; set; }
    public GisArea? Area { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}