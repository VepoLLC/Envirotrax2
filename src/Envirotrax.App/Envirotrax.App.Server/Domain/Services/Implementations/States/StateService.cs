using AutoMapper;
using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Data.Repositories.Definitions.States;
using Envirotrax.App.Server.Data.Repositories.Definitions.WaterSuppliers;
using Envirotrax.App.Server.Domain.DataTransferObjects.States;
using Envirotrax.App.Server.Domain.Services.Definitions.States;

namespace Envirotrax.App.Server.Domain.Services.Implementations.States
{
    public class StateService : Service<State, StateDto>, IStateService
    {
        public StateService(IMapper mapper, IStateRepository repository) : base(mapper, repository)
        {
        }
    }
}
