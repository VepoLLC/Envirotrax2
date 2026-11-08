using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/transporters/{professionalId}/notes")]
public class FogTransporterNoteController : ProfessionalNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogTransporter;

    public FogTransporterNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
