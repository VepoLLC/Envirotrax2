using Envirotrax.Common;

namespace Envirotrax.Auth.Data.Repositories.Defintions;

public interface IFeatureRepository
{
    Task<IEnumerable<FeatureType>> GetAllAsync(int? supplierId, int? professionalId);
}