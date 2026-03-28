using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Models.Professionals;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi
{
    public class CsiInspectorService : Service<Professional, ProfessionalDto>, ICsiInspectorService
    {
        public CsiInspectorService(IMapper mapper, ICsiInspectorRepository repository)
            : base(mapper, repository)
        {
        }
    }
}
