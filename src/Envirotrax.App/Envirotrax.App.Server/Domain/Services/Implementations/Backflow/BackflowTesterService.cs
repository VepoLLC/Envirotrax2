using AutoMapper;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Backflow;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Backflow;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Backflow
{
    public class BackflowTesterService : Service<Professional, ProfessionalDto>, IBackflowTesterService
    {
        public BackflowTesterService(IMapper mapper, IBackflowTesterRepository repository)
            : base(mapper, repository)
        {
        }
    }
}
