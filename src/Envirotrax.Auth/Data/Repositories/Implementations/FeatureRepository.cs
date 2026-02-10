
using Envirotrax.Auth.Data;
using Envirotrax.Auth.Data.Repositories.Defintions;
using Envirotrax.Common;
using Microsoft.EntityFrameworkCore;

public class FeatureRepository : IFeatureRepository
{
    private readonly ApplicationDbContext _dbContext;

    public FeatureRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private IEnumerable<FeatureType> ConvertToFeatureTypes(IEnumerable<FeatureProjection> featureProjections)
    {
        var features = new List<FeatureType>();

        if (featureProjections.Any())
        {
            if (featureProjections.Max(f => f.HasWiseGuys))
            {
                features.Add(FeatureType.WiseGuys);
            }

            if (featureProjections.Max(f => f.HasBackflowTesting))
            {
                features.Add(FeatureType.BackflowTesting);
            }

            if (featureProjections.Max(f => f.HasCsiInspection))
            {
                features.Add(FeatureType.CsiInspection);
            }

            if (featureProjections.Max(f => f.HasFogInspection))
            {
                features.Add(FeatureType.FogInspection);
            }

            if (featureProjections.Max(f => f.HasFogTransportation))
            {
                features.Add(FeatureType.FogTransportation);
            }
        }

        return features;
    }

    public async Task<IEnumerable<FeatureType>> GetAllAsync(int? supplierId, int? professionalId)
    {
        var supplierFeatures = _dbContext
            .GeneralSettings
            .Where(settings => settings.WaterSupplierId == supplierId)
            .Select(settings => new FeatureProjection
            {
                HasWiseGuys = settings.WiseGuys,
                HasBackflowTesting = settings.BackflowTesting,
                HasCsiInspection = settings.CsiInspections,
                HasFogInspection = settings.FogProgram,
                HasFogTransportation = settings.FogProgram
            });

        var professionalFeatures = _dbContext
            .Professionals
            .Where(pro => pro.Id == professionalId)
            .Select(pro => new FeatureProjection
            {
                HasWiseGuys = pro.HasWiseGuys,
                HasBackflowTesting = pro.HasBackflowTesting,
                HasCsiInspection = pro.HasCsiInspection,
                HasFogInspection = pro.HasFogInspection,
                HasFogTransportation = pro.HasFogTransportation
            });

        var combinedFeatures = await supplierFeatures
            .Concat(professionalFeatures)
            .ToListAsync();

        return ConvertToFeatureTypes(combinedFeatures);
    }
}

class FeatureProjection
{
    public bool HasWiseGuys { get; set; }
    public bool HasBackflowTesting { get; set; }
    public bool HasCsiInspection { get; set; }
    public bool HasFogInspection { get; set; }
    public bool HasFogTransportation { get; set; }
}