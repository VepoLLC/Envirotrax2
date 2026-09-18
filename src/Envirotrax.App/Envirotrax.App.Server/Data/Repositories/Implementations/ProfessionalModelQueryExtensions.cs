using Envirotrax.App.Server.Data.DbContexts;
using Envirotrax.App.Server.Data.Models.Professionals;

namespace Envirotrax.App.Server.Data.Repositories.Implementations;

public static class ProfessionalModelQueryExtensions
{
    /// <summary>
    /// Scopes an IProfessionalModel query to the current water supplier. These entities carry no
    /// WaterSupplierId, so EF applies no tenant filter; ProfessionalWaterSupplier does, and joining
    /// through it borrows that filter.
    /// </summary>
    public static IQueryable<TModel> ScopedToWaterSupplier<TModel>(this IQueryable<TModel> query, TenantDbContext dbContext)
        where TModel : IProfessionalModel
    {
        return query.Where(model => dbContext.ProfessionalWaterSuppliers.Any(pws => pws.ProfessionalId == model.ProfessionalId));
    }
}
