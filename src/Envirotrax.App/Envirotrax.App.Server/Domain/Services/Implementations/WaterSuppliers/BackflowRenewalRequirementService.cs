using AutoMapper;
using Envirotrax.App.Server.Data.Models.WaterSuppliers;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.WaterSuppliers;
using Envirotrax.App.Server.Domain.Services.Definitions.WaterSuppliers;

namespace Envirotrax.App.Server.Domain.Services.Implementations.WaterSuppliers;

public class BackflowRenewalRequirementService : Service<BackflowRenewalRequirement, BackflowRenewalRequirementDto>, IBackflowRenewalRequirementService
{
    public BackflowRenewalRequirementService(IMapper mapper, IBackflowRenewalRequirementRepository repository)
        : base(mapper, repository)
    {
    }
}
