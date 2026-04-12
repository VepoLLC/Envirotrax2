using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;

namespace Envirotrax.App.Server.Domain.Services.Definitions.Fog
{
    public interface IFogInspectorService : IService<Professional, ProfessionalDto>
    {
    }
}
