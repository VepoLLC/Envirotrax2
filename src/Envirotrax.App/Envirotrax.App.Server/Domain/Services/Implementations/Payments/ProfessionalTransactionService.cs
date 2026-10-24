using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Payments;
using Envirotrax.App.Server.Domain.Services.Definitions.Payments;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Payments;

public class ProfessionalTransactionService : Service<ProfessionalTransaction, ProfessionalTransactionDto>, IProfessionalTransactionService
{
    public ProfessionalTransactionService(IMapper mapper, IProfessionalTransactionRepository repository)
        : base(mapper, repository)
    {
    }
}
