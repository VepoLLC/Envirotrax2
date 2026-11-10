using Envirotrax.Admin.Server.Domain.DataTransferObjects.Professionals.Licenses;
using Envirotrax.Admin.Server.Domain.Services.Definitions.Professionals;
using Microsoft.AspNetCore.Mvc;

namespace Envirotrax.Admin.Server.Controllers.Fog;

[Route("api/fog/inspectors/{professionalId}/notes")]
public class FogInspectorNoteController : ProfessionalNoteController
{
    protected override ProfessionalType ProfessionalType => ProfessionalType.FogInspector;

    public FogInspectorNoteController(IProfessionalNoteService noteService)
        : base(noteService)
    {
    }
}
