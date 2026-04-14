
using Envirotrax.App.Server.Data.Models.WaterSuppliers.Features;
using Envirotrax.Common;

namespace Envirotrax.App.Server.Data.SeedData;

public static class FeatureSeedData
{
    public static IReadOnlyList<Feature> Features =>
    [
        new()
        {
            Id = FeatureType.BackflowTesting
        }
    ];
}