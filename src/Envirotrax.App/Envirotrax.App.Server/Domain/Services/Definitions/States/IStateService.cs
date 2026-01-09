using Envirotrax.App.Server.Data.Models.States;
using Envirotrax.App.Server.Domain.DataTransferObjects.States;

namespace Envirotrax.App.Server.Domain.Services.Definitions.States
{
    public interface IStateService :  IService<State, StateDto>
    {
    }
}
