using AutoMapper;
using Envirotrax.App.Server.Data.Models.Csi;
using Envirotrax.App.Server.Data.Repositories.Definitions.Csi;
using Envirotrax.App.Server.Domain.DataTransferObjects.Csi;
using Envirotrax.App.Server.Domain.Services.Definitions.Csi;

namespace Envirotrax.App.Server.Domain.Services.Implementations.Csi;

public class CsiInspectionService : Service<CsiInspection, CsiInspectionDto>, ICsiInspectionService
{
    public CsiInspectionService(IMapper mapper, ICsiInspectionRepository repository)
        : base(mapper, repository)
    {
    }
}
