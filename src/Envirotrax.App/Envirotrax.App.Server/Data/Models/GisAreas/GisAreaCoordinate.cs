
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

    // Which area this vertex belongs to: 0 is the outer edge, 1 and up are holes cut out of it
    // Vertices of one ring keep their drawing order through Id, so both are needed to restore it.
    public int PolygonIndex { get; set; }

    public double Latitude { get; set; }
    public double Longitude { get; set; }
}