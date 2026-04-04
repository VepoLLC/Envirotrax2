
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Data.Services.Definitions;

namespace Envirotrax.App.Server.Data.Repositories.Implementations.Professionals;

public class ProfessionalInsuranceRepository : Repository<ProfessionalInsurance>, IProfessionalInsuranceRepository
{
    public ProfessionalInsuranceRepository(IDbContextSelector dbContextSelector)
        : base(dbContextSelector)
    {
    }

    protected override void UpdateEntity(ProfessionalInsurance model)
    {
        base.UpdateEntity(model);

        // You cannot update the file path.
        DbContext.Entry(model).Property(i => i.FilePath).IsModified = false;
    }
}