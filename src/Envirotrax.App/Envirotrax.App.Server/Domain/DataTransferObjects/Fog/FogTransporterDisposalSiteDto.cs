using Envirotrax.App.Server.Domain.DataTransferObjects.Professionals;
using Envirotrax.Common.Domain.DataTransferObjects;

namespace Envirotrax.App.Server.Domain.DataTransferObjects.Fog;

public class FogTransporterDisposalSiteDto : IDto
{
    public int Id { get; set; }

    public int DisposalSiteId { get; set; }

    public ReferencedProfessionalDto? Professional { get; set; }

    public bool IsActive { get; set; }
}
