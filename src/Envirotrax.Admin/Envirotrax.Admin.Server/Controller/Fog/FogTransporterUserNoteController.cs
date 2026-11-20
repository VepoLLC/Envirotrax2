using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/transporters/{professionalId}/users/{userId}/notes")]
public class FogTransporterUserNoteController : ProfessionalUserNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogTransporter;

    public FogTransporterUserNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
